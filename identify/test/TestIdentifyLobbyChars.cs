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
	public class TestIdentifyLobbyChars : TestBase
	{
        // const
        private const bool USE_SCREEN = true;
        private const bool SAVE_UNKNOWN = false;

		public static void Main(string[] args)
		{
            Log.SetLevel(Log.Level.FINEST);
            Iterator<Image> screenIter;
            if (USE_SCREEN)
            {
                // wait
                Log.Info("waiting ...");
                Thread.Sleep(5000);

                // full screen
                Console.WriteLine("## scanning for lobby ##");
                Image fullScreen = new ScreenImageIterator(new Win32Control()).next();
                Point offset = PatternLocator.locateLobby(fullScreen);
                Console.WriteLine("lobby found at x=" + offset.X + " y=" + offset.Y);

                // desk
                LobbyLayout layout = new LobbyLayout();
                screenIter = new ScreenImageIterator(new Win32Control(), new Rectangle(offset.X + layout.TableList.X, offset.Y + layout.TableList.Y, layout.TableList.Width, layout.TableList.Height));
            }
            else
            {
                screenIter = new MockOneImageIterator(toImage(new Bitmap("test/lobby1.bmp")));
            }

			// screen + reduce + invert
            Iterator<Image> reduceIter = new ReduceColorIterator(screenIter, new ColorReducers.LobbyChars());
            Iterator<Image> replaceIter = new ReplaceColorIterator(reduceIter, Color.White, Color.Transparent);
			
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
			List<CharPattern> patterns = CharReader.readCharsFromResourcesLobby();
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
                    if (SAVE_UNKNOWN)
                    {
                        Console.WriteLine("Saving image");
                        saveBitmap("lobby", count++, toBitmap(ex.image));
                    }
                }
                catch (Exception)
                {
                }
                return "?";
			}
		}
	}
}
