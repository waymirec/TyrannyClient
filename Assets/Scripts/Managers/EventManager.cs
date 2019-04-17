using System;
using Tyranny.Client.Events;

namespace Tyranny.Client.System
{
    public class EventManager : Singleton<EventManager>
    {
        public event EventHandler<LoginEventArgs> OnLoggedIn;

        public void FireEvent_OnLoggedIn(object source, LoginEventArgs args) => OnLoggedIn?.Invoke(source, args);
    }
}
