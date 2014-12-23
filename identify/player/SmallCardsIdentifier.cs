using System;
using System.Drawing;

namespace PokerBot
{	
	public class SmallCardsIdentifier
	{
		private const int MIN_WHITE_PIXELS = 500;
		
		public static bool areReducedSmallCards(Image image)
		{
			int whitePixels = countWhitePixels(image);
			return whitePixels > MIN_WHITE_PIXELS;
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
