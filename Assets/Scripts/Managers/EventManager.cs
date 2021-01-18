using System;
using Tyranny.Client.Events;

namespace Tyranny.Client.System
{
    public class EventManager : Singleton<EventManager>
    {
        public event EventHandler<WorldAuthEventArgs> OnWorldAuth;
        public void FireEvent_OnWorldAuth(object source, WorldAuthEventArgs args) => OnWorldAuth?.Invoke(source, args);
        
        public event EventHandler<EventArgs> OnWorldConnect;
        public void FireEvent_OnWorldConnect(object source) => OnWorldConnect?.Invoke(source, null);

        public event EventHandler<EventArgs> OnWorldConnectFailed;
        public void FireEvent_OnWorldConnectFailed(object source) => OnWorldConnectFailed?.Invoke(source, null);
        
        public event EventHandler<EventArgs> OnWorldDisconnect;
        public void FireEvent_OnWorldDisconnect(object source) => OnWorldDisconnect?.Invoke(source, null);

    }
}
