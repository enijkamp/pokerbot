using System;
using System.Drawing;

namespace PokerBot
{	
	public class ControlsIdentifier
	{
		private const double MIN_COLORED_PIXELS = .8, MIN_RANGE_PIXELS = .45;
		private static ColorReducer reducer = new ColorReducers.Controls();

        public static bool areControlsVisibleWithNativeColor(Image image, Color[] range)
        {
            double coloredPixels = ImageTools.countPixels(image, range);
            double percentage = coloredPixels / image.pixels.Length;
            return percentage > MIN_RANGE_PIXELS;
        }
		
		public static bool areReducedControlsVisible(Image image)
		{
            double coloredPixels = countBrownPixels(image);
            double percentage = coloredPixels / image.pixels.Length;
            return percentage > MIN_COLORED_PIXELS;
		}
		
		public static bool areControlsVisible(Image image)
		{
			Image reduced = reducer.reduceColors(image);
            return areReducedControlsVisible(reduced);
		}
		
		private static int countBrownPixels(Image image)
		{
			int count = 0;
			for(int i = 0; i < image.pixels.Length; i++)
			{
				if(image.pixels[i] == Color.Brown.ToArgb())
					count++;
			}
			return count;
		}
	}
}
