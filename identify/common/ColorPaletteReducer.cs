using System;
using System.Drawing;

namespace PokerBot
{	
	public class ColorPaletteReducer : ColorReducer
	{
		private readonly Color[] lowColors;
		
		public ColorPaletteReducer(Color[] lowColors)
		{
			this.lowColors = lowColors;
		}
		
		public Image reduceColors(Image image)
		{			
			int[] newPixels = new int[image.pixels.Length];
			
			for (int i = 0; i < image.pixels.Length; i++) {
				if(image.pixels[i] == Image.EmptyPixel)
					newPixels[i] = Image.EmptyPixel;
				else
					newPixels[i] = nearestColor(image.pixels[i]).ToArgb();
			}
			
			return new Image(newPixels, image.width, image.height);
		}
		
		private Color nearestColor(int pix)
		{
			Color nearestColor = Color.Empty;
			double nearestDistance = double.MaxValue;
			
			foreach (Color color in lowColors)
			{
				int R = (pix >> 16) & 0xff;
				int G = (pix >> 8) & 0xff;
				int B = pix & 0xff;

				double testR = Math.Pow(Convert.ToDouble(color.R) - R, 2.0);
				double testG = Math.Pow(Convert.ToDouble(color.G) - G, 2.0);
				double testB = Math.Pow(Convert.ToDouble(color.B) - B, 2.0);

				double distance = Math.Sqrt(testR + testG + testB);

				if(distance == 0.0)
				{
					return color;
				}
				else if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestColor = color;
				}
			}
			return nearestColor;

		}
	}
}
