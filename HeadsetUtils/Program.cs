using System;
using HeadsetUtils;
using log4net;

namespace HeadsetUtils
{
    internal class Program
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(nameof(Program));
        private static IDefaultAudioDeviceManager defaultAudioDeviceManager;

        private static string headsetName;
        private static string speakersName;
        static void Main(string[] args)
        {
            log.Info($"Running {nameof(HeadsetUtils)}...");
            headsetName = Configuration.GetString("HeadsetName");
            speakersName = Configuration.GetString("SpeakersName");
            log.Debug($"HeadsetName: {headsetName}, SpeakersName:{speakersName}");
            IConnectionEventSource source = new LogFileConnectionEventSource(headsetName);
            defaultAudioDeviceManager = new PowershellDefaultAudioDeviceManager();
            source.OnConnected += OnConnected;
            source.OnDisconnected += OnDisconnected;
            Task.Delay(-1).Wait();
        }

        public static void OnConnected()
        {
            log.Info("OnConnected Invoked");
            try
            {
                defaultAudioDeviceManager.SetAsDefaultPlaybackDevice(headsetName);
            } catch (Exception ex)
            {
                log.Error($"Failed setting {headsetName} as default playback device, erorr:", ex);
            }
        }

        public static void OnDisconnected()
        {
            log.Info("OnDisconnected Invoked");
            try
            {
                defaultAudioDeviceManager.SetAsDefaultPlaybackDevice(speakersName);
            }
            catch (Exception ex)
            {
                log.Error($"Failed setting {speakersName} as default playback device, erorr:", ex);
            }
        }

    }
}

