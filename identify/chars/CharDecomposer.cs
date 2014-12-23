using System;
using System.Collections.Generic;

namespace PokerBot
{	
	public class CharDecomposer
	{
		private const int SPLIT_MIN_PIXELS = 1;
		private const int MIN_WIDTH = 10;

        public static List<Image> decompose(Image image, int minSplitPixels)
        {
            if (image.width > MIN_WIDTH)
            {
                for (int x = 0; x < image.width; x++)
                {
                    int[] line = image.getVerticalLine(x);
                    int pixels = countPixels(line);
                    // split image
                    if (pixels <= minSplitPixels)
                    {
                        Image left = image.crop(0, x + 1, 0, image.height);
                        Image right = image.crop(x + 1, image.width, 0, image.height);
                        return new List<Image>() { left, right };
                    }
                }
            }

            return new List<Image>() { image };
        }
		
		public static List<Image> decompose(Image image)
		{
            return decompose(image, SPLIT_MIN_PIXELS);
		}
		
		private static int countPixels(int[] pixels)
		{
			int count = 0;
			foreach(int pixel in pixels)
			{
				if(pixel != Image.EmptyPixel)
				{
					count++;
				}
			}
			return count;
		}
	}
}
