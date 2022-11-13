using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace HeadsetUtils
{
    static internal class Configuration
    {
        public static int GetInt(string key) => int.Parse(ConfigurationManager.AppSettings[key]);
        public static string GetString(string key) => ConfigurationManager.AppSettings[key];
        public static bool GetBool(string key) => bool.Parse(ConfigurationManager.AppSettings[key]);
    }
}
