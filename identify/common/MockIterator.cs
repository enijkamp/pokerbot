using System;
using System.Drawing;

namespace PokerBot
{	
	public class MockIterator : Iterator<Image>
	{
		private Image image;

		public MockIterator() {

		}
		
		public MockIterator(Image image) {
			this.image = image;
		}
		
		public Image Image
		{
			set { image = value; }
			get { return image; }
		}
		
		public bool hasNext() {
			return true;
		}

		public Image next() {
			return image;
		}
	}
}
