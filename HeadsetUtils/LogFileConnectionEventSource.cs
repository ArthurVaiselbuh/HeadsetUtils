using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeadsetUtils;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

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
        private string connectedRegexOverride = null;
        private string disconnectedRegexOverride = null;

        public LogFileConnectionEventSource(string deviceName)
        {
            this.deviceName = deviceName;
            this.logsPath = GetLogsMonitoringDirectory();
            log.Info($"Will monitor directory {logsPath} for logs");
            // Unfortunately FileSystemWatcher is not reliable if the file is continuously written to, so have to use a timer..
            var monitoringIntervalMs = Configuration.GetInt("MonitoringIntervalMs");
            connectedRegexOverride = Configuration.GetString("ConnectedRegexOverride");
            disconnectedRegexOverride = Configuration.GetString("DisconnectedRegexOverride");
            if (connectedRegexOverride != null)
                log.Info($"Will try to match {nameof(connectedRegexOverride)} = {connectedRegexOverride}");
            
            if (disconnectedRegexOverride != null)
                log.Info($"Will try to match {nameof(disconnectedRegexOverride)} = {disconnectedRegexOverride}");

            log.Info($"Will monitor changes every {monitoringIntervalMs}ms");
            t = new Timer(RaiseEventIfNeeded, null, monitoringIntervalMs, monitoringIntervalMs);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
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
                        log.Debug("Done reading previous log file");
                    } 
                    catch (Exception ex)
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

        private void ReadNewData()
        {
            log.Debug($"Attempting to read new data from {lastReadFilename}, from position: {lastReadOffset}");
            using var file = File.Open(lastReadFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            file.Seek(lastReadOffset, SeekOrigin.Begin);
            using var reader = new StreamReader(file);
            
            string? line;
            bool? lastConnectionState = null;
            while ((line = reader.ReadLine()) != null)
            {
                log.Verbose($"Read line: {line}");
                var lineMatch = TryMatchLine(line);
                if (lineMatch != null)
                    lastConnectionState = lineMatch;
            }

            lastReadOffset = file.Position;
            log.Verbose($"Updating {nameof(lastReadOffset)} to {lastReadOffset}");

            if (lastConnectionState == null)
                return;

            if (lastConnectionState.Value)
                OnConnected?.Invoke();
            else
                OnDisconnected?.Invoke();
        }

        /// <summary>
        /// Try matching a single line from the log file against the logic of connected/disconnected.
        /// </summary>
        /// <param name="line">The log line</param>
        /// <returns>null if the line is irrelevant. otherwise returns true/false depending on the connection state. </returns>
        private bool? TryMatchLine(string line)
        {
            Match match;
            if (connectedRegexOverride != null)
            {
                match = Regex.Match(line, connectedRegexOverride);
                if (match.Success)
                {
                    log.Info($"Line matched against {nameof(connectedRegexOverride)}");
                    return true;
                }
            }
            
            if (disconnectedRegexOverride != null)
            {
                match = Regex.Match(line, disconnectedRegexOverride);
                if (match.Success)
                {
                    log.Info($"Line matched against {nameof(disconnectedRegexOverride)}");
                    return false;
                }
            }

            match = Regex.Match(line, logLineRegex);
            log.Verbose($"Matches: {match.Length}");
            if (!match.Success)
                return null;

            if (!match.Groups[2].Value.Contains(deviceName, StringComparison.OrdinalIgnoreCase))
                return null;

            log.Info($"Found matching line:{match.Value}");
            if (!bool.TryParse(match.Groups[1].Value, out bool isConnected))
            {
                log.Warn($"Failed parsing {match.Groups[1].Value} as bool, ignoring event");
                return null;
            }

            return isConnected;
        }

        private string GetLogsMonitoringDirectory()
        {
            string[] supportedCueVersions = new[] { "CUE5", "CUE4" };
            var logsDirectoryOverride = Configuration.GetString("LogsDirectoryOverride");
            if (logsDirectoryOverride != null)
            {
                log.Info($"{nameof(logsDirectoryOverride)} is set, will monitor {logsDirectoryOverride}");
                return logsDirectoryOverride;
            }
            foreach (var supportedVersion in supportedCueVersions)
            {
                var logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Corsair", supportedVersion, "logs");
                if (Directory.Exists(logsPath))
                {
                    return logsPath;
                }
            }
            throw new DirectoryNotFoundException($"Failed to find directory for monitoring logs, consider setting {nameof(logsDirectoryOverride)} manually");
        }
    }
}
