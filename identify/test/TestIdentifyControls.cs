using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PokerBot
{
    public class TestIdentifyControls : TestBase
	{
        public static void Main(string[] args)
        {
            // screen + reduce + invert
            Iterator<Image> screenIter = new MockOneImageIterator(toImage(new Bitmap("test/table_no_middle_button.png")));
            Iterator<Image> reduceIter = new ReduceColorIterator(screenIter, new ColorPaletteReducer(new Color[] { Color.White, Color.Black }));
 
            // table
            ImagesRenderer renderer1 = newImageRenderer();
            setImage(renderer1, toBitmap(reduceIter.next()));

            // identifier
            TableIdentifier tableIdentifier = new TableIdentifier(new TableLayout9());

            // proxy
            ImagesRenderer renderer2 = newImageRenderer();
            tableIdentifier.RenderImageEvent += delegate(Image image, Point point)
			{
                setImage(renderer2, toBitmap(image));
			};

            // check
            Image check1 = toImage(new Bitmap("test/table_no_middle_button.png"));
            Image check2 = toImage(new Bitmap("test/table_with_mouse_fold.png"));
            Image check3 = toImage(new Bitmap("test/table_seat_is_occupied.png"));
            DateTime start = DateTime.Now;
            bool controls = tableIdentifier.identifyMove(check1);
            double time = DateTime.Now.Subtract(start).TotalMilliseconds;
            Console.WriteLine(controls);
            DateTime start2 = DateTime.Now;
            controls = tableIdentifier.identifyMove(check2);
            Console.WriteLine(controls);
            double time2 = DateTime.Now.Subtract(start2).TotalMilliseconds;
            controls = tableIdentifier.identifyMove(check3);            
            Console.WriteLine(controls);
            Console.WriteLine("took " + time + " ms and with boost " + time2 + " ms");
        }
    }
}
