namespace Tyranny.Client.System
{
    public class Configuration : Singleton<Configuration>
    {
        public AuthenticationServer AuthenticationServer { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            AuthenticationServer = new AuthenticationServer();
            AuthenticationServer.Address = "192.168.255.128";
            AuthenticationServer.Port = 5554;
        }
    }

    public class AuthenticationServer
    {
        public string Address { get; set; }
        public int Port { get; set; }
    }
}
