using NLog;
using System;
using Managers;
using Events;
using Network;
using Tyranny.Networking;
using UnityEngine;

namespace Handlers
{
    public class WorldHandlers
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [GamePacketHandler(GameOpcode.Hello)]
        public static void HandleHello(PacketReader<GameOpcode> packetIn, WorldClient worldClient)
        {
            var guid = new Guid(packetIn.ReadBytes(16));
            worldClient.Id = guid;
            
            Registry.Get<NetworkEventManager>().FireEvent_OnWorldAuth(worldClient, new NetworkWorldAuthEventArgs
            {
                Guid = worldClient.Id
            });
        }

        [GamePacketHandler(GameOpcode.Ping)]
        public static void HandlePing(PacketReader<GameOpcode> packetIn, WorldClient worldClient)
        {
            var counter = packetIn.ReadInt32();
            Registry.Get<NetworkEventManager>().FireEvent_OnPing(worldClient, new NetworkPingPongArgs
            {
                Guid = worldClient.Id, Count = counter
            });
            
            var pong = new PacketWriter<GameOpcode>(GameOpcode.Pong);
            pong.Write(counter + 1);
            worldClient.Send(pong);
        }
        
        [GamePacketHandler(GameOpcode.Pong)]
        public static void HandlePong(PacketReader<GameOpcode> packetIn, WorldClient worldClient)
        {
            var counter = packetIn.ReadInt32();
            Registry.Get<NetworkEventManager>().FireEvent_OnPong(worldClient, new NetworkPingPongArgs
            {
                Guid = worldClient.Id, Count = counter
            });
        }
        
        [GamePacketHandler(GameOpcode.EnterWorld)]
        public static void HandleEnterWorld(PacketReader<GameOpcode> packetIn, WorldClient worldClient)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnEnterWorld(worldClient, new NetworkEnterWorldEventArgs
            {
                Guid = worldClient.Id, 
                Position = new Vector3(packetIn.ReadSingle(), packetIn.ReadSingle(), packetIn.ReadSingle())
            });

            worldClient.Send(new PacketWriter<GameOpcode>(GameOpcode.WorldEntityDisco));
        }

        [GamePacketHandler(GameOpcode.SpawnWorldEntity)]
        public static void HandleSpawnWorldEntity(PacketReader<GameOpcode> packetIn, WorldClient worldClient)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnSpawnWorldEntity(worldClient, new NetworkSpawnWorldEntityEventArgs
            {
                Guid = new Guid(packetIn.ReadBytes(16)), 
                Position = new Vector3(packetIn.ReadSingle(), packetIn.ReadSingle(), packetIn.ReadSingle())
            });
        }

        [GamePacketHandler(GameOpcode.DestroyWorldEntity)]
        public static void HandleDestroyWorldEntity(PacketReader<GameOpcode> packetIn, WorldClient worldClient)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnDestroyWorldEntity(worldClient, new NetworkDestroyWorldEntityEventArgs
            {
                Guid = new Guid(packetIn.ReadBytes(16))
            });
        }
        
        [GamePacketHandler(GameOpcode.MoveWorldEntity)]
        public static void HandleMoveWorldEntity(PacketReader<GameOpcode> packetIn, WorldClient worldClient)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnMoveWorldEntity(worldClient, new NetworkMoveWorldEntityArgs
            {
                Guid = new Guid(packetIn.ReadBytes(16)),
                Source = new Vector3(packetIn.ReadSingle(), packetIn.ReadSingle(), packetIn.ReadSingle()),
                Destination = new Vector3(packetIn.ReadSingle(), packetIn.ReadSingle(), packetIn.ReadSingle())
            });
        }
    }
}
