using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
namespace HeadsetUtils
{
    public class StartupManager
    {
        private string startupKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        public void SetRunAtStartup(string programName, string programPath, bool enabled)
        {
            using var reg = Registry.CurrentUser;
            using var key = reg.OpenSubKey(startupKey, true);
            if (key == null)
                throw new Exception($"Failed to open key {startupKey}");

            if (enabled)
            {
                key.SetValue(programName, programPath);
            }
            else
            {
                key.DeleteValue(startupKey);
            }
        }
    }
}
