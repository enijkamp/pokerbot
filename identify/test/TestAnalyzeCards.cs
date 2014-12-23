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
	class TestAnalyzeCards : TestBase
	{
		public static void Main(string[] args)
		{
			ImagesRenderer renderer = newImageRenderer();

            Iterator<Image> imageIter = new ScreenImageIterator(new Win32Control(), new Rectangle(100, 400, 80, 30));
			Image background = imageIter.next();					
			Iterator<Image> deltaIter = new DeltaImageAnalyzer(background, imageIter);			
			IteratorProxy<Image> proxyDeltaIter = new IteratorProxy<Image>(deltaIter);
			proxyDeltaIter.handler += delegate(Image next) 
			{
				setImage(renderer, toBitmap(next));
		    };		

			HashSet<HashImage> hashImages = new HashSet<HashImage>();
			Iterator<List<Image>> patitionIter = new ImageVerticalPartitioner(proxyDeltaIter);
			int count = 0;
			while(patitionIter.hasNext()) {
				List<Image> images = patitionIter.next();
				foreach(Image image in images) {
					if(!hasMinimumDimensions(image)) {
						continue;
					}
					HashImage hash = new HashImage(image);
					if(!hashImages.Contains(hash)) {
						hashImages.Add(hash);
						System.Drawing.Image bitmap = toBitmap(image);
						addImage(renderer, bitmap);
						saveBitmap("card", count++, bitmap);
					}
				}				
			}
		}
		
		public static bool hasMinimumDimensions(Image image) {
			return image.width > 20 && image.height > 40;
		}

	}
}