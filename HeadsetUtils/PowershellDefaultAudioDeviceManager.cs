using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HeadsetUtils
{
    internal class PowershellDefaultAudioDeviceManager : IDefaultAudioDeviceManager
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(nameof(PowershellDefaultAudioDeviceManager));
        private const string PowershellPath = @"c:\windows\system32\WindowsPowerShell\v1.0\powershell.exe";
        public void SetAsDefaultPlaybackDevice(string name)
        {
            log.Info($"Attempting to set {name} as default audio device");
            // See https://community.spiceworks.com/topic/2292318-select-audio-device-with-powershell for library installation
            var startingInfo = new ProcessStartInfo()
            {
                FileName = PowershellPath,
                Arguments = $"-ExecutionPolicy Bypass -command \"get-audiodevice -list | where Type -eq 'Playback'| where name -Like '*{name}*' | set-audiodevice\"",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using var process = Process.Start(startingInfo);
            process.WaitForExit();
            log.Debug($"Script exited with code {process.ExitCode}");
        }
    }
}
