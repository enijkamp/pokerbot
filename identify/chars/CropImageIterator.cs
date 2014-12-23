using System;
using System.Collections.Generic;
using System.Drawing;

namespace PokerBot
{	
	public class CropImageIterator : IteratorApply<Image>
	{	
		public CropImageIterator(Iterator<List<List<Image>>> iterator) : base(iterator, apply)
		{
		}
		
		private static List<Image> apply(Image image)
		{
			return new List<Image>() { ImageCropper.crop(image) };
		}
	}
}
