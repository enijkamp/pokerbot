using System;
using System.Drawing;
using System.Threading;

namespace PokerBot
{	
	public class MockWaitImageIterator : Iterator<Image>
	{
		private Image image;
        private int ms = 2000;

		public MockWaitImageIterator() {

		}
        
        public MockWaitImageIterator(int ms, Image image) {
            this.ms = ms;
            this.image = image;
		}
		
		public MockWaitImageIterator(Image image) {
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
            Thread.Sleep(ms);
			return image;
		}
	}
}
