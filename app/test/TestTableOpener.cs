using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace PokerBot
{
    public class TestTableOpener
    {
        public static void Main(string[] args)
        {
            Log.SetLevel(Log.Level.FINEST);
            Thread.Sleep(5000);
            Dictionary<string, string> config = BotAppLogic.ReadConfig();
            Win32Control ctrl = new Win32Control();
            TableOpener opener = new TableOpener(ctrl, new HumanMouse(ctrl), new Keyboard(ctrl), new TableIdentifier(new TableLayout9()));
            Settings settings = new Settings();
            settings.AutoLocateTablesNum = 1;
            opener.OpenNewTables(settings, ImageRendererForm.newImageRendererForm());
            Thread.Sleep(3000);
            opener.SitIn();
        }
    }
}
