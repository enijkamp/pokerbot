using System;
using System.Collections;
using System.Collections.Generic;

namespace PokerBot
{	
	public class ImageVerticalPartitioner : Iterator<List<Image>>
	{
		struct XBlock {
			public int x1, x2;
			
			public XBlock(int x1, int x2)
			{
				this.x1 = x1;
				this.x2 = x2;
			}
		}
		
		struct XYBlock {
			public int x1, x2;
			public int y1, y2;
			
			public XYBlock(int x1, int x2, int y1, int y2)
			{
				this.x1 = x1;
				this.x2 = x2;
				this.y1 = y1;
				this.y2 = y2;
			}
		}

		private readonly Iterator<Image> iterator;
		
		public ImageVerticalPartitioner(Iterator<Image> iterator) {
			this.iterator = iterator;
		}	

		public bool hasNext() {
			return iterator.hasNext();
		}
		
		public List<Image> next() {
			Image image = iterator.next();
		
			// identify vertical blocks
			List<XBlock> xBlocks = identifyXBlocks(image);
			
			// identify blocks
			List<XYBlock> xyBlocks = identifyXYBlocks(image, xBlocks);
			
			// extract images
			List<Image> images = extractImages(image, xyBlocks);

			return images;
		}
		
		private List<Image> extractImages(Image image, List<XYBlock> blocks) {
			List<Image> images = new List<Image>();
			foreach(XYBlock block in blocks) {
				Image sub = image.crop(block.x1, block.x2, block.y1, block.y2);
				images.Add(sub);
			}
			return images;
		}
		
		private List<XBlock> identifyXBlocks(Image image) {
			List<XBlock> blocks = new List<XBlock>();
			bool isEmptyBlock = true;
			int startX = 0;
			for(int x = 0; x < image.width; x++) {
				// get line
				int[] line = image.getVerticalLine(x);
				bool isEmptyLine = isEmpty(line);				
				
				// start of block
				if(!isEmptyLine && isEmptyBlock) {
					isEmptyBlock = false;					
					startX = x;										
				}
				
				// end of block
				if(isEmptyLine && !isEmptyBlock) {
					blocks.Add(new XBlock(startX, x));
					isEmptyBlock = true;
				}
			}
			return blocks;		
		}
		
		private List<XYBlock> identifyXYBlocks(Image image, List<XBlock> xBlocks) {
			List<XYBlock> blocks = new List<XYBlock>();
			foreach(XBlock xBlock in xBlocks) {
				// coors
				int y1 = 0; 
				int y2 = image.height;			
				int x1 = xBlock.x1; 
				int x2 = xBlock.x2;
				
				// sanity check
				if(x1 > x2) {
					continue;
				}
				
				// find top
				for(int y = 0; y < image.height; y++) {
					int[] line = image.getHorizontalLine(x1, x2, y);
					if(isEmpty(line)) {
						y1 = y;
					} else {
						break;
					}
				}
				
				// find bottom
				for(int y = image.height - 1; y > 0; y--) {
					int[] line = image.getHorizontalLine(x1, x2, y);
					if(isEmpty(line)) {
						y2 = y;
					} else {
						break;
					}
				}
				
				// sanity check
				if(x1 > x2 || y1 > y2) {
					continue;
				}
				
				// crop
				XYBlock block = new XYBlock(x1, x2, y1, y2);
				blocks.Add(block);
			}
			
			return blocks;		
		}
		
		private bool isEmpty(int[] line) {
			foreach(int pixel in line) {
				if(pixel != Image.EmptyPixel) {
					return false;
				}
			}
			return true;
		}
	}
}
