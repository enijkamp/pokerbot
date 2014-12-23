using System;

namespace PokerBot
{	
	public class ReduceColorIterator : Iterator<Image>
	{
		private readonly Iterator<Image> iterator;
		private readonly ColorReducer reducer;
		
		public ReduceColorIterator(Iterator<Image> iterator, ColorReducer reducer)
		{
			this.iterator = iterator;
			this.reducer = reducer;
		}
		
		public bool hasNext() 
		{
			return iterator.hasNext();
		}
		
		public Image next() 
		{
			return reducer.reduceColors(iterator.next());
		}
	}
}
