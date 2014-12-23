using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;

namespace PokerBot
{
	public class CardIdentifier
	{		
		public delegate List<CardRegion> IdentifyRegions(Image value);		
		public delegate List<Card> IdentifyCards(List<CardRegion> value);
		
		private readonly IdentifyRegions identifyRegionsFunction;
		private readonly IdentifyCards identifyCardsFunction;
		
		public CardIdentifier(IdentifyRegions identifyRegions, 
		                      IdentifyCards identifyCards)
		{
			this.identifyRegionsFunction = identifyRegions;
			this.identifyCardsFunction = identifyCards;
		}
		
		public List<Card> identifyCards(Image image) 
	    {
			List<CardRegion> cardRegions = identifyRegionsFunction(image);

			List<Card> sortedCards = identifyCardsFunction(cardRegions);
			
			return sortedCards;
		}
	}
}
