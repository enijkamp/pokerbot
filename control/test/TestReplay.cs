using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PokerBot
{
    public class TestReplay
    {
        public static void Main(string[] args)
        {
            DeviceControl device = new Win32Control();
            Thread.Sleep(5 * 1000);
            Replayer replayer = new Replayer(device);
            replayer.Replay(new ReplayBubbles());
            Thread.Sleep(5 * 1000);
        }
    }
}
