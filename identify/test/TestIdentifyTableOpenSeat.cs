using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Reflection;

namespace PokerBot
{	
	public class TestIdentifyTableOpenSeat : TestBase
	{
        // const
        private const bool USE_SCREEN = false;
        
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
                screen = new MockOneImageIterator(toImage(new Bitmap("test/seatsopen_cropped.bmp")));
            }

            // renderer
            ImagesRenderer renderer = newImageRenderer();

			// identifier
            TableIdentifier tableIdentifier = new TableIdentifier(new TableLayout9());

            // pattern
            Log.Debug("reading seat pattern");
            Stream stream = AssemblyTools.getAssemblyStream("open_seat.png");
            Bitmap bitmap = Bitmap.FromStream(stream) as Bitmap;
            Image pattern = toImage(bitmap);
            pattern = new ColorReducers.SeatOpen().reduceColors(pattern);
            stream.Close();
			
			// loop
			while(screen.hasNext())
			{
				// start
				Console.WriteLine("## iteration -> start ##");
				DateTime start = DateTime.Now;
				
				// table				
				Console.WriteLine("# next table image #");
				Image tableImage = screen.next();

                // reduce
                ColorReducer reducer = new ColorReducers.SeatOpen();

                // rnder
                setImage(renderer, toBitmap(tableImage));				
				
				// identify seats
                List<Point> seats = new List<Point>();
                Log.Fine("scanning lines ...");
                DateTime seatsStart = DateTime.Now;
                foreach (Rectangle seat in new TableLayout9().Seats)
                {
                    bool isOpen = IsOpen(reducer, seat, pattern, tableImage, 5);
                }
                Console.WriteLine("## seat scan -> " + DateTime.Now.Subtract(seatsStart).TotalMilliseconds + " ms ##");
				
				// print
                foreach (Point seat in seats)
                {
                    Console.WriteLine(seat);
                }
				
				// end
				double time = DateTime.Now.Subtract(start).TotalMilliseconds;
				Console.WriteLine("## iteration -> end -> "+time+" ms ##");
			}
		}

        private static bool IsOpen(ColorReducer reducer, Rectangle seat, Image pattern, Image tableImage, int maxScanY)
        {
            Image seatCropped = tableImage.crop(seat.X, seat.X + seat.Width, seat.Y, seat.Y + seat.Height);
            Image seatReduced = reducer.reduceColors(seatCropped);
            for (int y = 0; y < maxScanY; y++)
            {
                Log.FineIf(y % 100 == 0, y + "/" + seatReduced.height);
                for (int x = 0; x < seatReduced.width - pattern.width; x++)
                {
                    Image sub = seatReduced.crop(x, x + pattern.width, y, y + pattern.height);
                    if (ImageTools.match(sub, pattern))
                    {
                        Log.Info("found seat pattern x=" + x + ", y=" + y);
                        return true;
                    }
                }
            }
            return false;
        }
	}
}
