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
	public class TestIdentifyPlayer : TestBase
	{
		public static void Main(string[] args)
		{			
			// screen + reduce + invert
			Iterator<Image> screenIter = new MockIterator(toImage(new Bitmap("player10.png")));
			//Iterator<Image> screenIter = new ScreenImageIterator(new Rectangle(100, 400, 80, 30));			
			Iterator<Image> reduceIter = new ReduceColorIterator(screenIter, new ColorReducers.TextBox());
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
			Iterator<List<List<Image>>> decomposeIter = new DecomposeImageIterator(patitionIter);
			Iterator<List<List<Image>>> cropIter = new CropImageIterator(decomposeIter);
			
			// patterns
			List<CharPattern> patterns = CharReader.readCharsFromResourcesPlayer();
			CharIdentifier identifier = new CharIdentifier(patterns);
			
			// identify
			int count = 0;
			while(cropIter.hasNext()) 
			{
				List<List<Image>> images = cropIter.next();
				foreach(List<Image> line in images)
				{
					foreach(Image chr in line) 
					{
						String chars = identifyChars(identifier, chr, ref count);
						Console.Write(chars);
					}
				    Console.WriteLine();
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
				Console.WriteLine("Saving image");
				saveBitmap("player", count++, toBitmap(ex.image));
				return "?";
			}
		}
	}
}
