using System;
using System.Drawing;

namespace PokerBot
{
	public class ColorInverter
	{
		private readonly Color color1;
		private readonly Color color2;
		
		public ColorInverter(Color color1, Color color2)
		{
			this.color1 = color1;
			this.color2 = color2;
		}
		
		public Image invert(Image image)
		{
			int[] pixels = new int[image.pixels.Length];
			for(int i = 0; i < pixels.Length; i++)
			{
				if(image.pixels[i] == color1.ToArgb())
				{
					pixels[i] = color2.ToArgb();
				}
				else if(image.pixels[i] == color2.ToArgb())
				{
					pixels[i] = color1.ToArgb();
				}
				else
				{
					pixels[i] = image.pixels[i];
				}
			}			
			return new Image(pixels, image.width, image.height);
		}
	}
}
