using System;
using System.Diagnostics;

namespace PokerBot
{
	
	public class DeltaImageAnalyzer : Iterator<Image>
	{
		private readonly Image background;
		private readonly Iterator<Image> iterator;
		
		public DeltaImageAnalyzer(Image background,
				Iterator<Image> iterator) {
			this.background = background;
			this.iterator = iterator;
		}
		
		public bool hasNext() {
			return iterator.hasNext();
		}
		
		public Image next() {
			while(true) {
				Image current = iterator.next();
				if (!areImagesEqual(background, current)) {
					return diff(background, current);
				}
			}
		}	
		
		private bool areImagesEqual(Image img1, Image img2) {
			int[] timg1 = img1.pixels;
			int[] timg2 = img2.pixels;
			for (int i = 0; i < timg1.Length; i++) {
				if (timg1[i] != timg2[i]) {
					return false;
				}
			}
			return true;
		}
		
		private Image diff(Image background, Image current) {

			// check
			Debug.Assert(background.width != current.width || background.height != current.height);
			
			// diffs
			int width = background.width;
			int height = background.height;
			int[] pixels = new int[width*height];
			
			// for each pixels
			for(int i = 0; i < pixels.Length; i++) {
				int pixelB = background.pixels[i];
				int pixelC = current.pixels[i];
				if(pixelB == pixelC) {
					pixels[i] = Image.EmptyPixel;
				} else {
					pixels[i] = pixelC;
				}
			}
			
			// symbol
			return new Image(pixels, width, height);
		}
	}
}
