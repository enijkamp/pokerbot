using System;
using System.Threading;

namespace PokerBot
{
	public class WaitDeltaImageIterator : Iterator<Image>
	{		
		private readonly Iterator<Image> iterator;
		private Image last;
		
		public WaitDeltaImageIterator(Iterator<Image> iterator) {
			this.iterator = iterator;
			this.last = iterator.next();
		}
		
		public bool hasNext() {
			return iterator.hasNext();
		}
		
		public Image next() {
			while(true) {
				Image current = iterator.next();
                if (!areImagesEqual(last, current))
                {
                    last = current;
                    return current;
                }
                else
                {
                    Thread.Sleep(20);
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
	}
}
