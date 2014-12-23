using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace PokerBot
{	
	public class TestIdentifyTaskBar : TestBase
	{

        public static void Main(string[] args)
        {
            Image bar = toImage(new Bitmap("test/taskbar_rows.png"));
            bar = bar.crop(110, 1190, 730, 785);

            DateTime start1 = DateTime.Now;
            Console.WriteLine(PatternLocator.locateTaskBarPrograms(bar, 0, 0).Count);
            Console.WriteLine(DateTime.Now.Subtract(start1).TotalMilliseconds);

            DateTime start2 = DateTime.Now;
            Console.WriteLine(PatternLocator.locateTaskBarPrograms(bar, 0, 0).Count);
            Console.WriteLine(DateTime.Now.Subtract(start2).TotalMilliseconds);

            Console.ReadKey();
        }		
	}
}