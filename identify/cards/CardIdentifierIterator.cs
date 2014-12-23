using System;
using System.Collections.Generic;

namespace PokerBot
{
	public class CardIdentifierIterator : Iterator<List<Card>>
	{
		private readonly Iterator<Image> iterator;
		private readonly CardIdentifier identifier;
		
		public CardIdentifierIterator(Iterator<Image> iterator,
		                              CardIdentifier.IdentifyRegions identifyRegions, 
		                              CardIdentifier.IdentifyCards identifyCards)
		{
            this.iterator = iterator;
			this.identifier = new CardIdentifier(identifyRegions, identifyCards);
		}
		
		public bool hasNext() 
		{
			return iterator.hasNext();
		}
		
		public List<Card> next() 
		{
			Image image = iterator.next();
			return identifier.identifyCards(image);
		}
	}
}
