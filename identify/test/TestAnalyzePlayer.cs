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
	public class TestAnalyzePlayer : TestBase
	{
		public static void Main(string[] args)
		{
			ImagesRenderer renderer = newImageRenderer();

			// screen + reduce + invert
			Iterator<Image> screenIter = new MockIterator(toImage(new Bitmap("player9.png")));
			//Iterator<Image> screenIter = new ScreenImageIterator(new Rectangle(100, 400, 80, 30));
			Iterator<Image> reduceIter = new ReduceColorIterator(screenIter, new ColorReducers.TextBox());
			Iterator<Image> invertIter = new InvertColorIterator(reduceIter, Color.White, Color.Black);
			Iterator<Image> replaceIter = new ReplaceColorIterator(invertIter, Color.White, Color.Transparent);
		
			// proxy
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
			
			// hash
			HashSet<HashImage> hashImages = new HashSet<HashImage>();			
			int count = 0;
			while(cropIter.hasNext()) 
			{
				List<List<Image>> images = cropIter.next();
				foreach(List<Image> line in images)
				{
					foreach(Image image in line) 
					{
						// dimensions
						if(!hasDimensions(image)) continue;
						
						// hash
						HashImage hash = new HashImage(image);
						if(!hashImages.Contains(hash))
						{
							hashImages.Add(hash);
							System.Drawing.Image bitmap = toBitmap(image);
							addImage(renderer, bitmap);
							Console.WriteLine("Saving image " + count++);
							saveBitmap("char", uniqueId(hash), bitmap);
						}
					}	
				}
			}
		}
		
		public static bool hasDimensions(Image image) 
		{
			return image.width < 40 && image.height < 40;
		}

	}
}
