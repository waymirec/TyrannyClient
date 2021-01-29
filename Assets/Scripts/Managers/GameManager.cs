using System;
using Events;
using Network;
using Tyranny.Networking;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private NetworkEventManager _networkEvent;
        private WorldClient _worldClient;
        
        protected override void OnAwake()
        {
            _networkEvent = Registry.Get<NetworkEventManager>();
            _worldClient = new WorldClient();
        }

        private void OnEnable()
        {
            _networkEvent.OnLoginAuth += OnLoginAuth;
            _networkEvent.OnWorldAuth += OnWorldAuth;
        }

        private void OnDisable()
        {
            _networkEvent.OnLoginAuth -= OnLoginAuth;
            _networkEvent.OnWorldAuth -= OnWorldAuth;
        }

        private void OnLoginAuth(object source, NetworkLoginAuthEventArgs args)
        {
            _worldClient?.Close();
            _worldClient = new WorldClient {Host = args.WorldHost, Port = args.WorldPort};
            _worldClient.Connect(args.Username, args.Token);
        }
        
        private void OnWorldAuth(object source, NetworkWorldAuthEventArgs args)
        {
            SceneManager.LoadScene("WorldScene");
            var readyPacket = new PacketWriter(TyrannyOpcode.GameReady);
            _worldClient.Send(readyPacket);
        }
    }
}