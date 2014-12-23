using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PokerBot
{
    public class TestDeviceControl
    {
        public static void Main(string[] args)
        {
            DeviceControl control = new VirtualBoxControl("Poker Niklas");
            //Thread.Sleep(3000);
            //control.KeyboardSendMinimizeAllWindows();
            Thread.Sleep(5000);
            control.KeyboardSend("cmn");
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
