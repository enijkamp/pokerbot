using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace PokerBot
{
    public class TestHook
    {

        public static void Main(string[] Arg)
        {
            new TestHook();
            while (true)
            {
                Thread.Sleep(10);
            }
        }

        public TestHook()
        {
        }

        public void MyKeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine("KeyPress 	- " + e.KeyChar);
        }
    }
}
