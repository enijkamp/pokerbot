using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace PokerBot.App.Gui
{
    public class BotApp
    {
        public static void Main(string[] args)
        {
            Application.Run(new BotForm());
        }
    }
}
