using NLog;
using System;
using System.Collections.Generic;
using Tyranny.Client.Handlers;
using Tyranny.Client.System;
using Tyranny.Networking;
using UnityEngine.SceneManagement;

namespace Tyranny.Client.Network
{
    public class WorldClient : Singleton<WorldClient>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public delegate void PacketHandlerDelegate(PacketReader packetIn, WorldClient worldClient);

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

        private AsyncTcpClient tcpClient;
        private Dictionary<TyrannyOpcode, PacketHandlerDelegate> packetHandlers;

        public WorldClient()
        {
            packetHandlers = PacketHandler.Load<PacketHandlerDelegate>(typeof(WorldHandlers));
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
            tcpClient = new AsyncTcpClient();
            tcpClient.OnConnected += OnConnected;
            tcpClient.OnConnectFailed += OnConnectFailed;
            tcpClient.OnDisconnected += OnDisconnected;
            tcpClient.OnDataReceived += OnDataReceived;
            tcpClient.Connect(Host, Port);

        }

        public void Close()
        {
            tcpClient.Close();
        }

        public void Send(PacketWriter packetOut)
        {
            Logger.Debug($"Sending packet ({packetOut.Opcode})...");
            tcpClient.Send(packetOut);
        }

        public void OnConnected(object source, SocketEventArgs args)
        {
            Registry.Get<EventManager>().FireEvent_OnWorldConnect(this);
            PacketWriter ident = new PacketWriter(TyrannyOpcode.GameIdent);
            ident.Write(Username);
            ident.Write((short)AuthToken.Length);
            ident.Write(AuthToken);
            Logger.Debug("Sending ident");
            tcpClient.Send(ident);
        }

        public void OnConnectFailed(object source, SocketEventArgs args)
        {
            Registry.Get<EventManager>().FireEvent_OnWorldConnectFailed(this);
            Logger.Error($"Failed to connect to {Host}:{Port}");
            SceneManager.LoadScene("LoginScene");
        }

        public void OnDisconnected(object source, SocketEventArgs args)
        {
            Registry.Get<EventManager>().FireEvent_OnWorldDisconnect(this);
            Logger.Info($"Disconnected from {Host}:{Port}");
            SceneManager.LoadScene("LoginScene");
        }

        public void OnDataReceived(object source, PacketEventArgs args)
        {
            TyrannyOpcode opcode = args.Packet.Opcode;
            if (packetHandlers.TryGetValue(opcode, out PacketHandlerDelegate handler))
            {
                Logger.Debug($"Received opcode: {opcode}");
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