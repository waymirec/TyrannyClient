using System;
using Events;

namespace Managers
{
    public class NetworkEventManager : Singleton<NetworkEventManager>
    {
        public event EventHandler<NetworkPingPongArgs> OnPing;
        public void FireEvent_OnPing(object source, NetworkPingPongArgs args) =>
            OnPing?.Invoke(source, args);

        public event EventHandler<NetworkPingPongArgs> OnPong;
        public void FireEvent_OnPong(object source, NetworkPingPongArgs args) =>
            OnPong?.Invoke(source, args);

        public event EventHandler<NetworkLoginAuthEventArgs> OnLoginAuth;
        public void FireEvent_OnLoginAuth(object source, NetworkLoginAuthEventArgs args) =>
            OnLoginAuth?.Invoke(source, args);
        
        public event EventHandler<NetworkWorldAuthEventArgs> OnWorldAuth;
        public void FireEvent_OnWorldAuth(object source, NetworkWorldAuthEventArgs args) => 
            OnWorldAuth?.Invoke(source, args);
        
        public event EventHandler<EventArgs> OnWorldConnect;
        public void FireEvent_OnWorldConnect(object source) => 
            OnWorldConnect?.Invoke(source, null);

        public event EventHandler<EventArgs> OnWorldConnectFailed;
        public void FireEvent_OnWorldConnectFailed(object source) => 
            OnWorldConnectFailed?.Invoke(source, null);
        
        public event EventHandler<EventArgs> OnWorldDisconnect;
        public void FireEvent_OnWorldDisconnect(object source) => 
            OnWorldDisconnect?.Invoke(source, null);

        public event EventHandler<NetworkEnterWorldEventArgs> OnEnterWorld;
        public void FireEvent_OnEnterWorld(object source, NetworkEnterWorldEventArgs args) => 
            OnEnterWorld?.Invoke(source, args);
        
        public event EventHandler<NetworkSpawnWorldEntityEventArgs> OnSpawnWorldEntity;
        public void FireEvent_OnSpawnWorldEntity(object source, NetworkSpawnWorldEntityEventArgs args) =>
            OnSpawnWorldEntity?.Invoke(source, args);

        public event EventHandler<NetworkDestroyWorldEntityEventArgs> OnDestroyWorldEntity;
        public void FireEvent_OnDestroyWorldEntity(object source, NetworkDestroyWorldEntityEventArgs args) =>
            OnDestroyWorldEntity?.Invoke(source, args);

        public event EventHandler<NetworkMoveWorldEntityArgs> OnMoveWorldEntity;
        public void FireEvent_OnMoveWorldEntity(object source, NetworkMoveWorldEntityArgs args) =>
            OnMoveWorldEntity?.Invoke(source, args);
    }
}