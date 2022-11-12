using System;
using HeadsetUtils;
using log4net;

namespace HeadsetUtils
{
    internal class Program
    {
        private const string HeadsetName = "HS70 Pro";
        private const string SpeakersName = "Speakers";

        private static log4net.ILog log = log4net.LogManager.GetLogger(nameof(Program));
        private static IDefaultAudioDeviceManager defaultAudioDeviceManager;
        static void Main(string[] args)
        {
            log.Info($"Running {nameof(HeadsetUtils)}...");
            IConnectionEventSource source = new LogFileConnectionEventSource(HeadsetName);
            defaultAudioDeviceManager = new PowershellDefaultAudioDeviceManager();
            source.OnConnected += OnConnected;
            source.OnDisconnected += OnDisconnected;
            Console.ReadLine();
        }

        public static void OnConnected()
        {
            log.Info("OnConnected Invoked");
            try
            {
                defaultAudioDeviceManager.SetAsDefaultPlaybackDevice(HeadsetName);
            } catch (Exception ex)
            {
                log.Error($"Failed setting {HeadsetName} as default playback device, erorr:", ex);
            }
        }

        public static void OnDisconnected()
        {
            log.Info("OnDisconnected Invoked");
            try
            {
                defaultAudioDeviceManager.SetAsDefaultPlaybackDevice(SpeakersName);
            }
            catch (Exception ex)
            {
                log.Error($"Failed setting {HeadsetName} as default playback device, erorr:", ex);
            }
        }

    }
}

