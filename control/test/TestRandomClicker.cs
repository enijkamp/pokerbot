using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PokerBot
{
    public class TestRandomClicker
    {
        public static void Main(string[] args)
        {
            Win32Control control = new Win32Control();
            Mouse mouse = new HumanMouse(control);
            RandomClicker clicker = new RandomClicker(new Point(control.DisplayWidth, 0), mouse);
            clicker.click();
            clicker.click();
        }
    }
}
