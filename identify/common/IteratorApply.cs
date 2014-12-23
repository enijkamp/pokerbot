using System;
using System.Collections.Generic;

namespace PokerBot
{	
	public class IteratorApply<T> : Iterator<List<List<T>>>
	{
		public delegate List<T> Apply(T value);
		
		private readonly Apply apply;
		private readonly Iterator<List<List<T>>> iterator;

		public IteratorApply(Iterator<List<List<T>>> iterator, Apply apply)
		{
			this.iterator = iterator;
			this.apply = apply;
		}
		
		public bool hasNext() {
			return iterator.hasNext();
		}

		public List<List<T>> next() {
			List<List<T>> next = iterator.next();
			List<List<T>> result = new List<List<T>>();
			foreach(List<T> list in next)
			{
				List<T> resultList = new List<T>();
				foreach(T element in list)
				{
					List<T> elements = apply(element);
					resultList.AddRange(elements);
				}
				result.Add(resultList);
			}
			return result;
		}
	}
}
