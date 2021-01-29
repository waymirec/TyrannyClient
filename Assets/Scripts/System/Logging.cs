using NLog;
using UnityEngine;

namespace System
{
    public class Logging : Singleton<Logging>
    {
        public Logging()
        {
            Initialize();
        }

        private void Initialize()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "/Users/waymirec/tyranny_client.log" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;

            var logger = LogManager.GetCurrentClassLogger();
            Debug.Log("Logging initialized.");
            logger.Info("Logging started.");
        }
    }
}