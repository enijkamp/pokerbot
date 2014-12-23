using System;
using System.Drawing;

namespace PokerBot
{	
	public class MockOneImageIterator : Iterator<Image>
	{
		private Image image;
        private bool hasNextImage = true;

		public MockOneImageIterator() {

		}
		
		public MockOneImageIterator(Image image) {
			this.image = image;
		}
		
		public Image Image
		{
			set { image = value; }
			get { return image; }
		}
		
		public bool hasNext() {
			return hasNextImage;
		}

		public Image next() {
            hasNextImage = false;
			return image;
		}
	}
}