using System;

namespace PokerBot
{	
	public class ImageCropper
	{
		public static Image crop(Image image)
		{
			int yStart = 0;
			for(int y = 0; y < image.height; y++)
			{
				int[] line = image.getHorizontalLine(y);
				if(!isTransparent(line))
				{
					yStart = y;
					break;
				}
			}
			
			int yEnd = image.height-1;
			for(int y = image.height-1; y > yStart; y--)
			{
				int[] line = image.getHorizontalLine(y);
				if(!isTransparent(line))
				{
					yEnd = y;
					break;
				}
			}
			return getYSubImage(image, yStart, yEnd);
		}
		
		private static bool isTransparent(int[] line)
		{
			foreach(int pixel in line)
			{
				if(pixel != Image.EmptyPixel)
					return false;
			}
			return true;
		}
		
				
		public static Image getYSubImage(Image image, int yStart, int yEnd) {
			int yEndLine = yEnd + 1;
			int newHeight = yEndLine - yStart;
			int arrayStart = yStart * image.width;
			int arrayLength = newHeight * image.width;
			int[] newPixels = new int[arrayLength];			
			Array.Copy(image.pixels, arrayStart, newPixels, 0, arrayLength);
			return new Image(newPixels, image.width, newHeight);
		}
	}
}
