using NLog;
using System;
using System.Collections.Generic;
using CommonLib;
using Handlers;
using Managers;
using Tyranny.Networking;
using UnityEngine.SceneManagement;

namespace Network
{
    public class WorldClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; private set; }
        public byte[] AuthToken { get; private set; }

        public Guid Id
        {
            get => tcpClient.Id;  
            set => tcpClient.Id = value;
        }
        
        public bool Connected => tcpClient?.Connected ?? false;

        private AsyncTcpClient<GameOpcode> tcpClient;
        private delegate void PacketHandlerDelegate(PacketReader<GameOpcode> packetIn, WorldClient worldClient);
        private Dictionary<GameOpcode, PacketHandlerDelegate> packetHandlers;

        public WorldClient()
        {
            packetHandlers = PacketHandlerLoader<GameOpcode>.Load<PacketHandlerDelegate>(typeof(GamePacketHandler), typeof(WorldHandlers));
        }

        private void OnDestroy()
        {
            Close();
        }

        public void Connect(String username, byte[] authToken)
        {
            Username = username;
            AuthToken = authToken;

            Logger.Debug($"Connecting to server {Host}:{Port}...");
            tcpClient = new AsyncTcpClient<GameOpcode>();
            tcpClient.OnConnected += OnConnected;
            tcpClient.OnConnectFailed += OnConnectFailed;
            tcpClient.OnDisconnected += OnDisconnected;
            tcpClient.OnDataReceived += OnDataReceived;
            tcpClient.Connect(Host, Port);

        }

        public void Close()
        {
            try
            {
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Exception trying to close socket.");
            }
        }

        public void Send(PacketWriter<GameOpcode> packetOut)
        {
            Logger.Debug($"Sending packet ({packetOut.Opcode})...");
            tcpClient.Send(packetOut);
        }

        private void OnConnected(object source, TcpSocketEventArgs<GameOpcode> args)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnWorldConnect(this);
            var ident = new PacketWriter<GameOpcode>(GameOpcode.GameIdent);
            ident.Write(Username);
            ident.Write((short)AuthToken.Length);
            ident.Write(AuthToken);
            Logger.Debug("Sending ident");
            tcpClient.Send(ident);
        }

        private void OnConnectFailed(object source, TcpSocketEventArgs<GameOpcode> args)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnWorldConnectFailed(this);
            Logger.Error($"Failed to connect to {Host}:{Port}");
            SceneManager.LoadScene("LoginScene");
        }

        private void OnDisconnected(object source, TcpSocketEventArgs<GameOpcode> args)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnWorldDisconnect(this);
            Logger.Info($"Disconnected from {Host}:{Port}");
            SceneManager.LoadScene("LoginScene");
        }

        private void OnDataReceived(object source, TcpPacketEventArgs<GameOpcode> args)
        {
            var opcode = args.Packet.Opcode;
            if (packetHandlers.TryGetValue(opcode, out PacketHandlerDelegate handler))
            {
                Logger.Debug($"Received opcode: {opcode}. Handler = {handler}");
                try
                {
                    handler(args.Packet, this);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString());
                }
            }
            else
            {
                Logger.Warn($"No handler found for opcode {opcode}");
            }
        }
    }
}