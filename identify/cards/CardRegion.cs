using System;

namespace PokerBot
{
	public struct CardRegion
	{
		public CardRegion(Card card, int x, float overlap)
		{
			this.card = card;
			this.x = x;
			this.overlap = overlap;
		}
		
		public readonly Card card;
		public readonly int x;
		public readonly float overlap;
	}
}
