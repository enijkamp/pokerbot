using System;
using System.Drawing;

namespace PokerBot
{
	
	public class ColorReplacer
	{
		private readonly Color color1;
		private readonly Color color2;
		
		public ColorReplacer(Color color1, Color color2)
		{
			this.color1 = color1;
			this.color2 = color2;
		}
		
		public Image replace(Image image)
		{
			int[] pixels = new int[image.pixels.Length];
			for(int i = 0; i < pixels.Length; i++)
			{
				if(image.pixels[i] == color1.ToArgb())
				{
					pixels[i] = color2.ToArgb();
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
