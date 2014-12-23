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
	public class TestIdentifyBets : TestBase
	{
		public static void Main(string[] args)
		{
			// screen + reduce + invert
			Iterator<Image> screenIter = new MockOneImageIterator(toImage(new Bitmap("test/bet.png")));
			//Iterator<Image> screenIter = new ScreenImageIterator(new Rectangle(100, 400, 80, 30));			
			Iterator<Image> reduceIter = new ReduceColorIterator(screenIter, new ColorReducers.Bets());
			Iterator<Image> invertIter = new InvertColorIterator(reduceIter, Color.White, Color.Black);
			Iterator<Image> replaceIter = new ReplaceColorIterator(invertIter, Color.White, Color.Transparent);
			
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
			List<CharPattern> patterns = CharReader.readCharsFromResourcesBets();
			CharIdentifier identifier = new CharIdentifier(patterns);
			
			// identify
			int count = 0;
			while(cropIter.hasNext()) 
			{
				List<Image> line = cropIter.next()[0];
                // chars
				String textLine = "";
				foreach(Image chars in line) 
				{
					List<Image> combos = CharDecomposer.decompose(chars);
					foreach(Image chr in combos)
					{
						Image character = ImageCropper.crop(chr);
						textLine += identifyChars(identifier, character, ref count);
					}						
				}
                
                // original
                Console.WriteLine(textLine);
                
                // check for digit
                if(!containsDigit(textLine))
                {
                    textLine = "no bet";
                }
                else 
                {                
                    // format
                   textLine = textLine.Replace("?", "").Replace("$", "").Trim();
                }
                
                Console.WriteLine(textLine);
			}
		}
        
        private static bool containsDigit(string text)
        {
            foreach(char chr in text.ToCharArray())
            {
                if(chr == '0') return true;
                if(chr == '1') return true;
                if(chr == '2') return true;
                if(chr == '3') return true;
                if(chr == '4') return true;
                if(chr == '5') return true;
                if(chr == '6') return true;
                if(chr == '7') return true;
                if(chr == '8') return true;
            }
            return false;
        }
		
		public static String identifyChars(CharIdentifier identifier, Image image, ref int count)
		{
			try
			{
				return identifier.identifyChars(image);
			}
			catch(UnknownCharException ex)
			{
				Console.WriteLine("Saving image");
				saveBitmap("bet", count++, toBitmap(ex.image));
				return "?";
			}
		}
	}
}
