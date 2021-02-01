using System;
using Events;
using Network;
using NLog;
using Tyranny.Networking;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private NetworkEventManager _networkEvents;
        private WorldClient _worldClient;

        protected override void OnAwake()
        {
            _networkEvents = Registry.Get<NetworkEventManager>();
        }
        private void OnEnable()
        {
            _networkEvents.OnLoginAuth += OnLoginAuth;
            _networkEvents.OnWorldAuth += OnWorldAuth;
        }

        private void OnDisable()
        {
            _networkEvents.OnLoginAuth -= OnLoginAuth;
            _networkEvents.OnWorldAuth -= OnWorldAuth;
        }

        private void OnLoginAuth(object source, NetworkLoginAuthEventArgs args)
        {
            if(_worldClient != null && _worldClient.Connected) _worldClient.Close();
            _worldClient = new WorldClient {Host = args.WorldHost, Port = args.WorldPort};
            _worldClient.Connect(args.Username, args.Token);
        }
        
        private void OnWorldAuth(object source, NetworkWorldAuthEventArgs args)
        {
            Logger.Debug($"Got World Auth. Loading World Scene....");
            SceneManager.LoadScene("WorldScene");
            var readyPacket = new PacketWriter<GameOpcode>(GameOpcode.GameReady);
            Logger.Debug("Sending READY.");
            _worldClient.Send(readyPacket);
        }
    }
}