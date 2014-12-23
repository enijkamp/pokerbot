using System;
using System.Collections;
using System.Collections.Generic;

namespace PokerBot
{
	
	public class ImageHoriPartitionIterator : Iterator<List<List<Image>>>
	{
		private readonly Iterator<Image> iterator;
		
		public ImageHoriPartitionIterator(Iterator<Image> iterator) {
			this.iterator = iterator;
		}	

		public bool hasNext() {
			return iterator.hasNext();
		}
	
		public List<List<Image>> next() {
			Image image = iterator.next();
            return HorizontalPartitioner.partition(image);
		}
	}
}
