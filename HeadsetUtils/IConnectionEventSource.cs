using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadsetUtils
{
    internal interface IConnectionEventSource
    {
        public delegate void ConnectionEventHandler();
        public event ConnectionEventHandler OnConnected;
        public event ConnectionEventHandler OnDisconnected;
    }
}
