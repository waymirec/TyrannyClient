using NLog;
using System;
using System.Collections.Generic;
using Tyranny.Client.Handlers;
using Tyranny.Client.System;
using Tyranny.Networking;

namespace Tyranny.Client.Network
{
    public class WorldClient : Singleton<WorldClient>
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public delegate void PacketHandlerDelegate(PacketReader packetIn, WorldClient worldClient);

        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; private set; }
        public byte[] AuthToken { get; private set; }

        public Guid Id { get { return tcpClient.Id; } set { tcpClient.Id = value; } }
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
            tcpClient.Send(packetOut);
        }

        public void OnConnected(object source, SocketEventArgs args)
        {
            PacketWriter ident = new PacketWriter(TyrannyOpcode.GameIdent);
            ident.Write(Username);
            ident.Write((short)AuthToken.Length);
            ident.Write(AuthToken);
            logger.Debug("Sending ident");
            tcpClient.Send(ident);
        }

        public void OnConnectFailed(object source, SocketEventArgs args)
        {
            logger.Error($"Failed to connect to {Host}:{Port}");
        }

        public void OnDisconnected(object source, SocketEventArgs args)
        {
            logger.Info($"Disconnected from {Host}:{Port}");
        }

        public void OnDataReceived(object source, PacketEventArgs args)
        {
            TyrannyOpcode opcode = args.Packet.Opcode;
            PacketHandlerDelegate handler;
            if (packetHandlers.TryGetValue(opcode, out handler))
            {
                try
                {
                    handler(args.Packet, this);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.ToString());
                }
            }
            else
            {
                logger.Warn($"No handler found for opcode {opcode}");
            }
        }
    }
}