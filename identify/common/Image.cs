using System;
using System.Drawing;
using System.Diagnostics;

namespace PokerBot
{
	public class Image
	{		
		public static int EmptyPixel = Color.Transparent.ToArgb();
        public static Image Empty = null;

		private int[][] linesLazyLoad = null;
		public readonly int[] pixels;
		public readonly int width;
		public readonly int height;
		public readonly int npix;
		public readonly float aspectRatio;

		public Image(int[] pixels, int width, int height) {
			this.pixels = pixels;
			this.width = width;
			this.height = height;
			this.npix = width * height;
			this.aspectRatio = ((float) width) / ((float) height);
		}
	
		public Image(Image image) {
			this.pixels = image.pixels;
			this.height = image.height;
			this.width = image.width;
			this.npix = image.npix;
			this.aspectRatio = image.aspectRatio;
		}
		
		private int[][] constructHorizontalLines() {
			int[][] lines = new int[height][];
			for(int y = 0; y < height; y++)
			{
				int from = width * y;
				int length = width;
				lines[y] = new int[length];
				Array.Copy(pixels, from, lines[y], 0, length);
			}
			return lines;
		}

        public int[][] lines
        {
            get 
            {
                if (linesLazyLoad == null)
                {
                    linesLazyLoad = constructHorizontalLines();
                }
                return linesLazyLoad;
            }
        }

		public int getPixelIndex(int x, int y) {
			return (y * width) + x;
		}

		public int getPixel(int x, int y) {
			return pixels[(y * width) + x];
		}
		
		public Image crop(int xStart, int xEnd, int yStart, int yEnd) {
            if (xStart < 0 || xEnd > this.width)
            {
                throw new ArgumentException("crop x arguments invalid");
            }
            if (yStart < 0 || yEnd > this.height)
            {
                throw new ArgumentException("crop y arguments invalid");
            }
            if (xStart >= xEnd)
            {
                throw new ArgumentException("crop invalid xStart ("+xStart+") >= xEnd ("+xEnd+")");
            }
            if (yStart >= yEnd)
            {
                throw new ArgumentException("crop invalid yStart ("+yStart+") >= yEnd ("+yEnd+")");
            }
			int width = xEnd - xStart;
			int height = yEnd - yStart;
			int[] newPixels = new int[width*height];
			int i = 0;
			for(int y = yStart; y < yEnd; y++) {
				int arrayStart = (y * this.width) + xStart;
				int arrayLength = xEnd - xStart;
				Array.Copy(pixels, arrayStart, newPixels, i, arrayLength);
				i += arrayLength;
			}
			return new Image(newPixels, width, height);
		}
		
		public int[] getHorizontalLine(int y) {
			return lines[y];
		}
		
		public int[] getHorizontalLine(int xStart, int xEnd, int y) {
			int from = (width * y) + xStart;
			int length = xEnd - xStart;
			int[] line = new int[length];
			Array.Copy(pixels, from, line, 0, length);			
			return line;
		}
		
		public int[] getHorizontalLine(int x, int y) {
			int from = (width * y) + x;
			int length = width - x;
			int[] line = new int[length];
			Array.Copy(pixels, from, line, 0, length);			
			return line;
		}
		
		public int[] getVerticalLine(int x) {
			int[] line = new int[height];
			for(int y = 0; y < height; y++) {
				line[y] = getPixel(x, y);
			}
			return line;
		}
		
		public int[] getVerticalLine(int yStart, int yEnd, int x) {
			int height = yEnd - yStart;
			int[] line = new int[height];
			for(int y = 0; y < height; y++) {
				int absY = y + yStart;
				line[y] = getPixel(x, absY);
			}
			return line;
		}
		
		public Image toRGB() {
			int[] newPixels = new int[pixels.Length];
			for (int i = 0; i < newPixels.Length; i++) {
				int pix = pixels[i];
				newPixels[i] = (int)(pix | (pix << 8) | (pix << 16) | 0xff000000);
			}
			return new Image(newPixels, width, height);
		}

	}
}
