using System;

namespace PokerBot
{
	public class IteratorProxy<T> : Iterator<T>
	{
		public delegate void NextEventHandler(T next);		
		public event NextEventHandler handler;
		
		private readonly Iterator<T> iterator;
		
		public IteratorProxy(Iterator<T> iterator) {
			this.iterator = iterator;
		}

		public bool hasNext() {
			return iterator.hasNext();
		}

		public T next() {
			T next = iterator.next();
			if(handler != null) handler(next);
			return next;
		}
	}
}
