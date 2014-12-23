using System;
using System.Collections.Generic;

namespace PokerBot
{	
	public class CharIdentifier
	{
		struct Candidate
		{
			public double mse;
			public CharPattern pattern;
			public Candidate(double mse, CharPattern pattern)
			{
				this.mse = mse;
				this.pattern = pattern;
			}
		}
		
		private const double MSE_THRESHOLD = 5;

		private readonly List<CharPattern> patterns;
		private readonly int maxWidth;
		
		public CharIdentifier(List<CharPattern> patterns)
		{
			this.patterns = sortByPixels(patterns);
			this.maxWidth = getMaxWidth(patterns);
		}
		
		private List<CharPattern> sortByPixels(List<CharPattern> patterns)
		{
			// sort patterns according to the amount of pixels
			// thus the first matching pattern with mse == 0 
			// has the best intersection (in dimensions)
			patterns.Sort(delegate(CharPattern p1, CharPattern p2) 
			{ 
				return p2.pixels.Length - p1.pixels.Length; 
			});
			return patterns;
		}
		
		private int getMaxWidth(List<CharPattern> patterns)
		{
			int width = 0;
			foreach(CharPattern pattern in patterns)
			{
				if(pattern.width > width)
					width = pattern.width;
			}
			return width;
		}
		
		private Candidate findSubCandidate(Image test, int xOffset)
		{
			double bestMSE = double.MaxValue;
			CharPattern bestCandidate = patterns[0];
			
			foreach(CharPattern train in patterns)
			{					
				if(train.width > 2 && 
				   train.height <= test.height && 
				   (train.width + xOffset) <= test.width)
				{
					int distance = test.height - train.height;
					for(int y = 0; y <= distance; y++)
					{
						int xStart = xOffset;
						int xEnd = xStart + train.width;
						int yStart = y;
						int yEnd = yStart + train.height;
						Image subTest = test.crop(xStart, xEnd, yStart, yEnd);
						double mse = calcMSE(train, subTest);
							
						if(mse < bestMSE) 
						{
							bestMSE = mse;
							bestCandidate = train;
						}
						
						if(bestMSE == 0)
						{
							return new Candidate(bestMSE, bestCandidate);
						}
					}
				}
			}
			
			return new Candidate(bestMSE, bestCandidate);
		}
		
		private Candidate findCandidate(Image test)
		{
			double bestMSE = double.MaxValue;
			CharPattern bestCandidate = patterns[0];
			foreach(CharPattern train in patterns) 
			{
				double mse = calcMSE(train, test);
				if(mse < bestMSE) 
				{
					bestMSE = mse;
					bestCandidate = train;
				}
				
				if(mse == 0) 
				{
					break;
				}
			}
			return new Candidate(bestMSE, bestCandidate);
		}
		
		public String identifyChars(Image test) 
		{
			// single char
			Candidate candidate = findCandidate(test);
			if(candidate.mse < MSE_THRESHOLD) 
				return candidate.pattern.Character.ToString();
			
			// combined chars
			Candidate left = findSubCandidate(test, 0);
			Candidate right = findSubCandidate(test, left.pattern.width);
			if((left.mse + right.mse) < MSE_THRESHOLD)
				return left.pattern.Character.ToString() + right.pattern.Character.ToString();
			
			// unknown char(s)
			throw new UnknownCharException(test);
		}
		/**
		  * Calculate the error factor between a block of pixels and our image.
		  * @param theirPixels An array of grayscale pixels which contains the block to be compared
		  * This should be in binary format, with each pixel having a value of either <code>0</code>
		  * or <code>255</code>.
		  * @param w The width of the pixel array.
		  * @param h The height of the pixel array.
		  * @param x1 The position of the left border of the rectangle to be compared.
		  * @param y1 The position of the top border of the rectangle to be compared.
		  * @param x2 The position of the right border of the rectangle to be compared.
		  * Note that pixels up to, but not including this position, will be compared.
		  * @param y2 The position of the bottom border of the rectangle to be compared.
		  * Note that pixels up to, but not including this position, will be compared.
		  * @return A <code>double</code> representing the average per-pixel mean square error.
		  * Lower numbers indicate a better match.
		  */
		public double calcMSE(Image train, Image test) {

			// train
			int width = train.width;
			int height = train.height;
			int myMaxX = width - 1;
			int myMaxY = height - 1;
			int[] pixels = train.pixels;
			
			// test
			int[] theirPixels = test.pixels;
			int w = test.width;
			int x1 = 0;
			int x2 = test.width;
			int y1 = 0;
			int y2 = test.height;
			
			// least error
			int theirXRange = Math.Max((x2 - x1) - 1, 1);
			int theirYRange = Math.Max((y2 - y1) - 1, 1);
			int theirNPix = (theirXRange + 1) * (theirYRange + 1);
			int myX, myY;
			long thisError, totalError;
			long minError = long.MaxValue;
			int myLineIdx, theirIdx;
			totalError = 0L;
			for (int theirY = y1, yScan = 0;
				 theirY < y2; theirY++, yScan++) {
				theirIdx = (theirY * w) + x1;
				myY = ((yScan * myMaxY) / theirYRange);
				myLineIdx = myY * width;
				for (int theirX = x1, xScan = 0;
					 theirX < x2; theirX++, theirIdx++, xScan++) {
					myX = ((xScan * myMaxX) / theirXRange);
					if ((myX < 0) || (myX > myMaxX) || (myY < 0) || (myY > myMaxY)) {
						thisError = theirPixels[theirIdx] - 255;
					} else {
						thisError = theirPixels[theirIdx] - pixels[myLineIdx + myX];
					}
					totalError += (thisError * thisError);
				}
			}
			if (totalError < minError) {
				minError = totalError;
			}
			
			// adapt width
			int widthDiff = Math.Abs(width - w);
			int errorPixelsWidth = (widthDiff * test.height) * 255;
			minError += errorPixelsWidth;
			
			// adapt heigth
			int heightDiff = Math.Abs(height - test.height);
			int errorPixelsHeigth = (heightDiff * test.width) * 255;
			minError += errorPixelsHeigth;
			
			return Math.Sqrt((double) minError) / (double) theirNPix;
		}
	}
}
