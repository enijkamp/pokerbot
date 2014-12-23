using System;
using System.Collections.Generic;

namespace PokerBot
{	
	public interface CardStrategy
	{
		List<CardRegion> identifyRegions(Image value);		
		List<Card> identifyCards(List<CardRegion> value);
	}
}
