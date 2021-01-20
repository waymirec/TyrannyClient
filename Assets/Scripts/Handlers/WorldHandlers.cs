using NLog;
using System;
using Tyranny.Client.Events;
using Tyranny.Client.Network;
using Tyranny.Client.System;
using Tyranny.Networking;
using UnityEngine;
using UnityEngine.AI;

namespace Tyranny.Client.Handlers
{
    public class WorldHandlers
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [PacketHandler(TyrannyOpcode.Hello)]
        public static void HandleHello(PacketReader packetIn, WorldClient worldClient)
        {
            var guid = new Guid(packetIn.ReadBytes(16));
            worldClient.Id = guid;

            var worldAuthEventArgs = new WorldAuthEventArgs {Guid = worldClient.Id};
            Registry.Get<EventManager>().FireEvent_OnWorldAuth(worldClient, worldAuthEventArgs);
        }

        [PacketHandler(TyrannyOpcode.Ping)]
        public static void HandlePing(PacketReader packetIn, WorldClient worldClient)
        {
            var counter = packetIn.ReadInt32();

            var pong = new PacketWriter(TyrannyOpcode.Pong);
            pong.Write(counter + 1);
            worldClient.Send(pong);
        }
        
        [PacketHandler(TyrannyOpcode.EnterWorld)]
        public static void HandleEnterWorld(PacketReader packetIn, WorldClient worldClient)
        {
            var x = packetIn.ReadSingle();
            var y = packetIn.ReadSingle();
            var z = packetIn.ReadSingle();

            Logger.Debug($"Enter World: {x:F2}, {y:F2}, {z:F2}");

            var args = new EnterWorldEventArgs
            {
                Guid = worldClient.Id,
                Position = new Vector3(x, y, z)
            };

            Debug.Log($"Creating player object {x},{y},{z}");
            var position = new Vector3(x, y, z);
            var go = GameObject.Instantiate(Resources.Load("PlayerZombie"), position, Quaternion.identity) as GameObject;
            //Camera.main.GetComponent<FollowCamera>().target = go;
        }

        [PacketHandler(TyrannyOpcode.SpawnGo)]
        public static void HandleSpawnGameObject(PacketReader packetIn, WorldClient worldClient)
        {
            var guid = new Guid(packetIn.ReadBytes(16));
            var x = packetIn.ReadSingle();
            var y = packetIn.ReadSingle();
            var z = packetIn.ReadSingle();

            Debug.Log($"Spawning GO at {x},{y},{z}");
            var position = new Vector3(x, y, z);
            var go = GameObject.Instantiate(Resources.Load("PlayerZombie"), position, Quaternion.identity) as GameObject;
            go.name = guid.ToString();
        }

        [PacketHandler(TyrannyOpcode.DestroyGo)]
        public static void HandleDestroyGameObject(PacketReader packetIn, WorldClient worldClient)
        {
            var guid = new Guid(packetIn.ReadBytes(16));
            Debug.Log("Destroying GO: " + guid.ToString());
            GameObject.Destroy(GameObject.Find(guid.ToString()));
        }
        
        [PacketHandler(TyrannyOpcode.Move)]
        public static void HandleMove(PacketReader packetIn, WorldClient worldClient)
        {
            var guid = new Guid(packetIn.ReadBytes(16));
            var srcX = packetIn.ReadSingle();
            var srcY = packetIn.ReadSingle();
            var srcZ = packetIn.ReadSingle();
            var dstX = packetIn.ReadSingle();
            var dstY = packetIn.ReadSingle();
            var dstZ = packetIn.ReadSingle();
            
            var go = GameObject.Find(guid.ToString());
            go.GetComponent<NavMeshAgent>().SetDestination(new Vector3(dstX, dstY, dstZ));
        }
    }
}
