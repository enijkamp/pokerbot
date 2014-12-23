using System;
using System.Collections.Generic;
using System.Drawing;

namespace PokerBot
{	
	public class DecomposeImageIterator : IteratorApply<Image>
	{	
		public DecomposeImageIterator(Iterator<List<List<Image>>> iterator) : base(iterator, apply)
		{
		}
		
		private static List<Image> apply(Image image)
		{
			return CharDecomposer.decompose(image);
		}
	}
}
