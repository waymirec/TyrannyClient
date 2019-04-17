using NLog;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Tyranny.Client.System;
using Tyranny.Networking;
using UnityEngine;

namespace Tyranny.Client.Network
{
    public class AuthenticationClient
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private SHA256 sha256;
        private TcpClient tcpClient;
        private string serverAddress;
        private int serverPort;

        public AuthenticationClient(string serverAddress, int serverPort)
        {
            this.sha256 = SHA256Managed.Create();
            this.tcpClient = new TcpClient();
            this.serverAddress = serverAddress;
            this.serverPort = serverPort;
        }

        public AuthenticationResult authenticate(string username, string password)
        {
            bool connected = tcpClient.Connect(serverAddress, serverPort);
            if (!connected)
            {
                throw new IOException($"failed to connect to {serverAddress}:{serverPort}");
            }

            byte[] challenge = Identify(username);
            byte ack = Verify(challenge, password);
            AuthenticationResult result;
            if (ack == 0)
            {
                result = CompleteAuth();
            }
            else
            {
                result = new AuthenticationResult((AuthenticationStatus)ack);
            }
            return result;
        }

        private byte[] Identify(String username)
        {
            // SEND IDENT
            PacketWriter identPacket = new PacketWriter(TyrannyOpcode.AuthIdent);
            identPacket.Write((short)1); // Major Vsn
            identPacket.Write((short)1); // Minor Vsn
            identPacket.Write((short)1); // Maint Vsn
            identPacket.Write((short)1); // Build Vsn
            identPacket.Write(username);
            tcpClient.Send(identPacket);

            PacketReader challengePacket;
            if (tcpClient.Read(out challengePacket))
            {
                int len = challengePacket.ReadInt16();
                Console.WriteLine($"Challenge Length: {len}");
                byte[] challenge = challengePacket.ReadBytes(len);
                Console.WriteLine($"DATA: {BitConverter.ToString(challenge).Replace("-", "")}");

                return challenge;
            }
            else
            {
                throw new IOException("Failed to receive challenge");
            }
        }

        private byte Verify(byte[] challenge, string password)
        {
            byte[] passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            logger.Debug($"Password Hash: !{BitConverter.ToString(passwordHash).Replace("-", string.Empty)}!");

            IncrementalHash sha = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            sha.AppendData(challenge);
            sha.AppendData(passwordHash);
            byte[] proof = sha.GetHashAndReset();
            logger.Debug($"Proof: !{BitConverter.ToString(proof).Replace("-", string.Empty)}!");

            byte[] proofLength = BitConverter.GetBytes((short)proof.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(proofLength);

            PacketWriter proofPacket = new PacketWriter(TyrannyOpcode.AuthProof);
            proofPacket.Write(proofLength);
            proofPacket.Write(proof);
            tcpClient.Send(proofPacket);

            PacketReader proofAckPacket;
            if (tcpClient.Read(out proofAckPacket))
            {
                byte ack = proofAckPacket.ReadByte();
                logger.Debug($"Got Proof Ack: {ack}");

                return ack;
            }
            else
            {
                throw new IOException("Failed to receive proof ack");
            }
        }

        private AuthenticationResult CompleteAuth()
        {
            PacketWriter packetOut = new PacketWriter(TyrannyOpcode.AuthProofAckAck);
            packetOut.Write(1);
            tcpClient.Send(packetOut);

            PacketReader authCompletePacket;
            if (tcpClient.Read(out authCompletePacket))
            {
                int status = authCompletePacket.ReadInt32();
                if (status == 0)
                {
                    long ipValue = BitConverter.ToUInt32(authCompletePacket.ReadBytes(4), 0);
                    int port = authCompletePacket.ReadInt32();
                    short authTokenLen = authCompletePacket.ReadInt16();
                    byte[] authToken = authCompletePacket.ReadBytes(authTokenLen);
                    string ip = new IPAddress(ipValue).ToString();
                    logger.Debug($"Auth successful: Status={status}, IP={ip}, Port={port}, Token={BitConverter.ToString(authToken).Replace("-", string.Empty)}");
                    return new AuthenticationResult((AuthenticationStatus)status, ip, port, authToken);
                }
                else
                {
                    logger.Debug($"Auth failed with status {(AuthenticationStatus)status}");
                    return new AuthenticationResult((AuthenticationStatus)status);
                }
            }
            else
            {
                throw new IOException("Failed to receive auth complete");
            }
        }        
    }

    public class AuthenticationResult
    {
        public AuthenticationStatus Status { get; private set; }
        public String Ip { get; private set; }
        public int Port { get; private set; }
        public byte[] Token { get; private set; }

        public AuthenticationResult(AuthenticationStatus status)
        {
            Status = status;
        }

        public AuthenticationResult(AuthenticationStatus status, String ip, int port, byte[] token)
        {
            Status = status;
            Ip = ip;
            Port = port;
            Token = token;
        }
    }

    public enum AuthenticationStatus
    {
        Success = 0,
        InvalidCredentials = 1,
        NoServersAvailable = 999
    }
}