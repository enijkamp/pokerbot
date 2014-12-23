using System;

namespace PokerBot
{
	public interface Iterator<T>
	{
		bool hasNext();
		
		T next();
	}
}
