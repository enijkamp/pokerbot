using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;

namespace PokerBot
{	
	public class TestIdentifyTable : TestBase
	{
        // const
        private const bool USE_SCREEN = false;
        
		// members
        private static TableRenderer renderer = TableRendererForm.newTableRendererForm();
        
		public static void Main(string[] args)
        {            
            Iterator<Image> screen;
            if(USE_SCREEN)
            {
			    // wait
			    Log.Info("waiting ...");
    			Thread.Sleep(5000);
    			
    			// full screen
    			Console.WriteLine("## scanning for table ##");
    			Image fullScreen = new ScreenImageIterator(new Win32Control()).next();
    			Point offset = PatternLocator.locateTable(fullScreen);
                Console.WriteLine("table found at x=" + offset.X + " y=" + offset.Y);

    			// desk
                screen = new ScreenImageIterator(new Win32Control(), new Rectangle(offset.X, offset.Y, new TableLayout9().Size.Width, new TableLayout9().Size.Height));
    			screen = new WaitDeltaImageIterator(screen);
            }
            else
            {
                screen = new MockOneImageIterator(toImage(new Bitmap("test/table_highlight.png")));
            }

			// identifier
            TableIdentifier tableIdentifier = new TableIdentifier(new TableLayout9());
			tableIdentifier.RenderImageEvent += delegate(Image image, Point point)
			{
                renderer.renderImage(image, point);
			};
			
			// loop
			while(screen.hasNext())
			{
				// start
				Console.WriteLine("## iteration -> start ##");
				DateTime start = DateTime.Now;
				
				// clear
                renderer.clearImages();
				
				// table				
				Console.WriteLine("# next table image #");
				Image tableImage = screen.next();
                renderer.renderImage(tableImage, 0, 0);				
				
				// identify table
                Table table = tableIdentifier.identifyTable(tableImage, TableIdentifier.PlayerInfoEnum.BOTH, -1, 0);			
				
				// render table
				Console.WriteLine("# rendering table");
                renderer.render(table, tableIdentifier.Layout);
				
				// end
				double time = DateTime.Now.Subtract(start).TotalMilliseconds;
				Console.WriteLine("## iteration -> end -> "+time+" ms ##");
			}
		}
	}
}
