using NLog;
using System;
using Tyranny.Client.Events;
using Tyranny.Client.Network;
using Tyranny.Client.System;
using Tyranny.Networking;
using UnityEngine;

namespace Tyranny.Client.Handlers
{
    public class WorldHandlers
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        [PacketHandler(TyrannyOpcode.Hello)]
        public static void HandleHello(PacketReader packetIn, WorldClient worldClient)
        {
            Guid guid = new Guid(packetIn.ReadBytes(16));
            worldClient.Id = guid;

            LoginEventArgs loginEventArgs = new LoginEventArgs();
            loginEventArgs.Guid = worldClient.Id;
            Registry.Get<EventManager>().FireEvent_OnLoggedIn(worldClient, loginEventArgs);
        }

        [PacketHandler(TyrannyOpcode.EnterWorld)]
        public static void HandleEnterWorld(PacketReader packetIn, WorldClient worldClient)
        {
            float x = packetIn.ReadSingle();
            float y = packetIn.ReadSingle();
            float z = packetIn.ReadSingle();

            logger.Debug($"Enter World: {x.ToString("F2")}, {y.ToString("F2")}, {z.ToString("F2")}");

            EnterWorldEventArgs args = new EnterWorldEventArgs();
            args.Guid = worldClient.Id;
            args.Position = new Vector3(x, y, z);

            Debug.Log("Creating player object");
            Vector3 position = new Vector3(x, y, z);
            GameObject go = GameObject.Instantiate(Resources.Load("PlayerObject"), position, Quaternion.identity) as GameObject;
            Camera.main.GetComponent<FollowCamera>().target = go;
        }
    }
}
