using System;
using System.Drawing;

namespace PokerBot
{	
	public class ButtonIdentifier
	{
        private const int MIN_WHITE_PIXELS = 170;
		private static ColorReducer reducer = new ColorReducers.Button();

		public static bool isReducedButton(Image image)
		{
			int whitePixels = countWhitePixels(image);
			return whitePixels > MIN_WHITE_PIXELS;
		}
		
		public static bool isButton(Image image)
		{
			Image reduced = reducer.reduceColors(image);
			return isReducedButton(reduced);
		}
		
		private static int countWhitePixels(Image image)
		{
			int count = 0;
			for(int i = 0; i < image.pixels.Length; i++)
			{
				if(image.pixels[i] == Color.White.ToArgb())
					count++;
			}
			return count;
		}
	}
}
