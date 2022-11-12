using System;
using HeadsetUtils;
using log4net;

namespace HeadsetUtils
{
    internal class Program
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(nameof(Program));
        static void Main(string[] args)
        {
            log.Info($"Running {nameof(HeadsetUtils)}...");
            IConnectionEventSource source = new LogFileConnectionEventSource("HS70 Pro");
            source.OnConnected += OnConnected;
            source.OnDisconnected += OnDisconnected;
            Console.ReadLine();
        }

        public static void OnConnected()
        {
            log.Info("OnConnected Invoked");
        }

        public static void OnDisconnected()
        {
            log.Info("OnDisconnected Invoked");
        }

    }
}

