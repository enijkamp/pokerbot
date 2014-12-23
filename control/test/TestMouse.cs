using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace PokerBot
{
    class TestMouse
    {
        public static void Main(string[] args)
        {
            Log.SetLevel(Log.Level.DEBUG);
            DeviceControl device = new VirtualBoxControl("Poker Birgit");
            device.ResetMouse();
           
            Mouse mouse = new HumanMouse(device);            
            while (true)
            {
                mouse.Move(20, 20);
                mouse.Move(500, 500);
                mouse.Move(850, 850);
                mouse.Move(100, 100);
            }
        }
    }
}
