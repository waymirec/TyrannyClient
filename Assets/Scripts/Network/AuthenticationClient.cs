using NLog;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Tyranny.Networking;

namespace Tyranny.Client.Network
{
    public class AuthenticationClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly SHA256 _sha256;
        private readonly TcpClient _tcpClient;
        private readonly string _serverAddress;
        private readonly int _serverPort;

        public AuthenticationClient(string serverAddress, int serverPort)
        {
            _sha256 = SHA256Managed.Create();
            _tcpClient = new TcpClient();
            _serverAddress = serverAddress;
            _serverPort = serverPort;
        }

        public AuthenticationResult Authenticate(string username, string password)
        {
            bool connected = _tcpClient.Connect(_serverAddress, _serverPort);
            if (!connected)
            {
                throw new IOException($"failed to connect to {_serverAddress}:{_serverPort}");
            }

            var challenge = Identify(username);
            var ack = Verify(challenge, password);
            
            return ack == 0
                ? CompleteAuth()
                : new AuthenticationResult((AuthenticationStatus)ack);
        }

        private byte[] Identify(String username)
        {
            // SEND IDENT
            var identPacket = new PacketWriter(TyrannyOpcode.AuthIdent);
            identPacket.Write((short)1); // Major Vsn
            identPacket.Write((short)1); // Minor Vsn
            identPacket.Write((short)1); // Maint Vsn
            identPacket.Write((short)1); // Build Vsn
            identPacket.Write(username);
            _tcpClient.Send(identPacket);

            if (_tcpClient.Read(out PacketReader challengePacket))
            {
                var len = challengePacket.ReadInt16();
                Console.WriteLine($"Challenge Length: {len}");
                var challenge = challengePacket.ReadBytes(len);
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
            var passwordHash = _sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            
            var sha = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            sha.AppendData(challenge);
            sha.AppendData(passwordHash);
            
            var proof = sha.GetHashAndReset();
            Logger.Debug($"Proof: !{BitConverter.ToString(proof).Replace("-", string.Empty)}!");

            var proofLength = BitConverter.GetBytes((short)proof.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(proofLength);

            var proofPacket = new PacketWriter(TyrannyOpcode.AuthProof);
            proofPacket.Write(proofLength);
            proofPacket.Write(proof);
            _tcpClient.Send(proofPacket);
            
            if (_tcpClient.Read(out PacketReader proofAckPacket))
            {
                return proofAckPacket.ReadByte();
            }
            
            throw new IOException("Failed to receive proof ack");
        }

        private AuthenticationResult CompleteAuth()
        {
            var packetOut = new PacketWriter(TyrannyOpcode.AuthProofAckAck);
            packetOut.Write(1);
            _tcpClient.Send(packetOut);

            if (_tcpClient.Read(out PacketReader authCompletePacket))
            {
                var status = authCompletePacket.ReadInt32();
                if (status == 0)
                {
                    var ipValue = BitConverter.ToUInt32(authCompletePacket.ReadBytes(4), 0);
                    var port = authCompletePacket.ReadInt32();
                    var authTokenLen = authCompletePacket.ReadInt16();
                    var authToken = authCompletePacket.ReadBytes(authTokenLen);
                    var ip = new IPAddress(ipValue).ToString();
                    Logger.Debug($"Auth successful: Status={status}, IP={ip}, Port={port}, Token={BitConverter.ToString(authToken).Replace("-", string.Empty)}");
                    return new AuthenticationResult((AuthenticationStatus)status, ip, port, authToken);
                }
                else
                {
                    Logger.Debug($"Auth failed with status {(AuthenticationStatus)status}");
                    return new AuthenticationResult((AuthenticationStatus)status);
                }
            }
                
            throw new IOException("Failed to receive auth complete");
        }        
    }

    public class AuthenticationResult
    {
        public AuthenticationStatus Status { get; }
        public String Ip { get; }
        public int Port { get; }
        public byte[] Token { get; }

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