using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PokerBot
{	
	public class TestIdentifyControlsChars : TestBase
	{
		public static void Main(string[] args)
		{
			// screen + reduce + invert
			Iterator<Image> screenIter = new MockOneImageIterator(toImage(new Bitmap("test/control_post_sb.bmp")));
            Iterator<Image> reduceIter = new ReduceColorIterator(screenIter, new ColorReducers.ControlsChars());
            Iterator<Image> invertIter = new InvertColorIterator(reduceIter, Color.WhiteSmoke, Color.Black);
            Iterator<Image> replaceIter = new ReplaceColorIterator(invertIter, Color.WhiteSmoke, Color.Transparent);
			
			// proxy
			ImagesRenderer renderer = newImageRenderer();
			IteratorProxy<Image> proxyIter = new IteratorProxy<Image>(replaceIter);
			ColorReplacer replacer = new ColorReplacer(Color.Transparent, Color.Cyan);
			proxyIter.handler += delegate(Image next) 
			{
				setImage(renderer, toBitmap(replacer.replace(next)));
		    };

			// partition + decompose + crop
			Iterator<List<List<Image>>> patitionIter = new ImageHoriPartitionIterator(proxyIter);
			Iterator<List<List<Image>>> cropIter = new CropImageIterator(patitionIter);
			
			// patterns
			List<CharPattern> patterns = CharReader.readCharsFromResourcesControls();
			CharIdentifier identifier = new CharIdentifier(patterns);
			
			// identify
			int count = 0;
			while(cropIter.hasNext()) 
			{
				List<List<Image>> lines = cropIter.next();
                // line
                foreach (List<Image> line in lines)
                {
                    // chars
                    String textLine = "";
                    foreach (Image chars in line)
                    {
                        List<Image> combos = CharDecomposer.decompose(chars, 0);
                        foreach (Image chr in combos)
                        {
                            Image character = ImageCropper.crop(chr);
                            textLine += identifyChars(identifier, character, ref count);
                        }
                    }
                    Console.WriteLine(textLine);
                }
			}
		}
        		
		public static String identifyChars(CharIdentifier identifier, Image image, ref int count)
		{
			try
			{
				return identifier.identifyChars(image);
			}
			catch(UnknownCharException ex)
			{
                try
                {
                    Console.WriteLine("Saving image");
                    saveBitmap("button", count++, toBitmap(ex.image));
                }
                catch (Exception)
                {
                }
                return "?";
			}
		}
	}
}
