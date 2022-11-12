using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadsetUtils
{
    public interface IDefaultAudioDeviceManager
    {
        public void SetAsDefaultPlaybackDevice(string name);
    }
}
