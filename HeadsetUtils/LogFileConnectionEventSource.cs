using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeadsetUtils;
using System.Text.RegularExpressions;

namespace HeadsetUtils
{
    internal class LogFileConnectionEventSource : IConnectionEventSource
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(nameof(LogFileConnectionEventSource));

        private const string logLineRegex = "Process isConnected: ([a-z]+) for ([^\\(]+)";

        public event IConnectionEventSource.ConnectionEventHandler? OnConnected;
        public event IConnectionEventSource.ConnectionEventHandler? OnDisconnected;

        private string logsPath;
        private string deviceName;
        private string lastReadFilename = String.Empty;
        private long lastReadOffset; // offset the last line we read ended in
        private Timer t;

        public LogFileConnectionEventSource(string deviceName)
        {
            this.deviceName = deviceName;
            this.logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Corsair", "CUE4", "logs");
            // Unfortunately FileSystemWatcher is not reliable if the file is continuously written to, so have to use a timer..
            t = new Timer(RaiseEventIfNeeded, null, 1000, 1000);
        }

        private void RaiseEventIfNeeded(object? state)
        {
            log.Debug("Checking if need to raise event");
            var info = new DirectoryInfo(logsPath);
            var lastFile = info.GetFiles().OrderByDescending(fl => fl.CreationTimeUtc).FirstOrDefault();
            if (lastFile == null)
            {
                log.Debug("No log files foud");
                return;
            }

            if (!lastFile.FullName.Equals(lastReadFilename, StringComparison.OrdinalIgnoreCase))
            {
                log.Info($"New log file created:{lastFile.Name}");
                if (lastReadFilename != null && lastReadFilename != String.Empty)
                {
                    log.Info($"Will attempt to finish reading previous log file: {lastReadFilename}");
                    try
                    {
                        ReadNewData();
                    } catch (Exception ex)
                    {
                        log.Warn("Failed reading old file data, error:", ex);
                    }
                }

                ClearCurrentFileData();
                lastReadFilename = lastFile.FullName;
            }
            
            ReadNewData();
        }

        private void ClearCurrentFileData()
        {
            log.Debug("Clearing current file info");
            lastReadFilename = String.Empty;
            lastReadOffset = 0;
        }

        private async void ReadNewData()
        {
            log.Debug($"Attempting to read new data from {lastReadFilename}");
            using var file = File.Open(lastReadFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            file.Seek(lastReadOffset, SeekOrigin.Begin);
            using var reader = new StreamReader(file);
            
            string? line;
            bool? lastConnectionState = null;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var match = Regex.Match(line, logLineRegex);
                if (!match.Success)
                    continue;

                if (!match.Groups[2].Value.Contains(deviceName, StringComparison.OrdinalIgnoreCase))
                    continue;

                log.Info($"Found matching line:{match.Value}");
                if (!bool.TryParse(match.Groups[1].Value, out bool isConnected))
                {
                    log.Warn($"Failed parsing {match.Groups[1].Value} as bool, ignoring event");
                    continue;
                }
                lastConnectionState = isConnected;
            }
            lastReadOffset = file.Position;

            if (lastConnectionState == null)
                return;

            if (lastConnectionState.Value)
                OnConnected?.Invoke();
            else
                OnDisconnected?.Invoke();
        }
    }
}
