using System;
using System.Drawing;

namespace PokerBot
{
	public class RenderImage
	{
		private System.Drawing.Image image;
		private Point pos;
		
		public RenderImage()
		{
		}
		
		public RenderImage(System.Drawing.Image image, Point pos)
		{
			this.image = image;
			this.pos = pos;
		}
		
		public System.Drawing.Image Image
		{
			get { return image; }
			set { image = value; }
		}
		
		public Point Position
		{
			get { return pos; }
			set { pos = value; }
		}
	}
}
