using System;
using System.Collections;
using System.Collections.Generic;

namespace PokerBot
{	
	public class HorizontalPartitioner
	{
		struct YBlock {
			public int y1, y2;
			
			public YBlock(int y1, int y2)
			{
				this.y1 = y1;
				this.y2 = y2;
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
        public static List<List<Image>> partition(Image image)
        {
            List<ImageLine> lines = partitionWithY(image);
            List<List<Image>> result = new List<List<Image>>();
            foreach (ImageLine line in lines)
            {
                result.Add(line);
            }
            return result;
        }

        public static List<ImageLine> partitionWithY(Image image)
        {		
			// identify horizontal blocks
			List<YBlock> yBlocks = identifyYBlocks(image);
			
			// for each line
            List<ImageLine> images = new List<ImageLine>();
			foreach(YBlock yBlock in yBlocks)
			{
				// identify blocks
				List<XYBlock> xyBlocks = identifyXYBlocks(image, yBlock);
			
				// extract images
				List<Image> lineImages = extractImages(image, xyBlocks);

                // line
                ImageLine line = new ImageLine(yBlock.y1, lineImages);
				
				// add line
                images.Add(line);
			}

			return images;
		}
		
		private static List<Image> extractImages(Image image, List<XYBlock> blocks) {
			List<Image> images = new List<Image>();
			foreach(XYBlock block in blocks) {
				Image sub = image.crop(block.x1, block.x2, block.y1, block.y2);
				images.Add(sub);
			}
			return images;
		}
		
		private static List<YBlock> identifyYBlocks(Image image) {
			List<YBlock> blocks = new List<YBlock>();
			bool isEmptyBlock = true;
			int startY = 0;
			for(int y = 0; y < image.height; y++) {
				// get line
				int[] line = image.getHorizontalLine(y);
				bool isEmptyLine = isEmpty(line);				
				
				// start of block
				if(!isEmptyLine && isEmptyBlock) {
					isEmptyBlock = false;					
					startY = y;										
				}
				
				// end of block
				if(isEmptyLine && !isEmptyBlock) {
					blocks.Add(new YBlock(startY, y));
					isEmptyBlock = true;
				}
			}
			return blocks;		
		}
		
		private static List<XYBlock> identifyXYBlocks(Image image, YBlock yBlock) {
			// coors
			int y1 = yBlock.y1; 
			int y2 = yBlock.y2;
			
			// sanity check
			if(y1 > y2) return new List<XYBlock>();
			
			// x blocks
			return identifyXBlocks(image, y1, y2);
		}
		
		private static List<XYBlock> identifyXBlocks(Image image, int y1, int y2) {
			List<XYBlock> blocks = new List<XYBlock>();
			bool isEmptyBlock = true;
			int x1 = 0;
			for(int x = 0; x < image.width; x++) {
				// get line
				int[] line = image.getVerticalLine(y1, y2, x);
				bool isEmptyLine = isEmpty(line);				
				
				// start of block
				if(!isEmptyLine && isEmptyBlock) {
					isEmptyBlock = false;					
					x1 = x;										
				}
			
				// end of block
				if(isEmptyLine && !isEmptyBlock) {
					// end
					int x2 = x;
					
					// sanity check
					if(x1 > x2 || y1 > y2) continue;
					
					// block
					blocks.Add(new XYBlock(x1, x2, y1, y2));
					isEmptyBlock = true;
				}
			}
			return blocks;		
		}
		
		private static bool isEmpty(int[] line) {
			foreach(int pixel in line) {
				if(pixel != Image.EmptyPixel) {
					return false;
				}
			}
			return true;
		}
	}
}
