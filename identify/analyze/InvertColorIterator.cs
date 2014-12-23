using System;
using System.Drawing;

namespace PokerBot
{
	public class InvertColorIterator : Iterator<Image>
	{
		private readonly Iterator<Image> iterator;
		private readonly ColorInverter inverter;
		
		public InvertColorIterator(Iterator<Image> iterator, Color color1, Color color2)
		{
			this.iterator = iterator;
			this.inverter = new ColorInverter(color1, color2);
		}

		public bool hasNext() 
		{
			return iterator.hasNext();
		}
		
		public Image next() 
		{
			Image image = iterator.next();		
			return inverter.invert(image);;
		}
		
	}
}
