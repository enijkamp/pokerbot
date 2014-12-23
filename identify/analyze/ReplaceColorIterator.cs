using System;
using System.Drawing;

namespace PokerBot
{	
	public class ReplaceColorIterator : Iterator<Image>
	{
		private readonly Iterator<Image> iterator;
		private readonly ColorReplacer replacer;
		
		public ReplaceColorIterator(Iterator<Image> iterator, Color color1, Color color2)
		{
			this.iterator = iterator;
			this.replacer = new ColorReplacer(color1, color2);
		}
		
		public bool hasNext() 
		{
			return iterator.hasNext();
		}
		
		public Image next() 
		{
			return replacer.replace(iterator.next());
		}

	}
}
