using System;

namespace PokerBot
{	
	public class Card
	{
		public enum RankEnum
		{
			Two,
			Three,
			Four,
			Five,
			Six,
			Seven,
			Eigth,
			Nine,
			Ten,
			Jack,
			Queen,
			King,
			Ace
		}
		
		public enum SuitEnum
		{
			Clubs,
			Diamonds,
			Hearts,
			Spades
		}
		
		private readonly RankEnum rank;
		private readonly SuitEnum suit;
		
		public Card(SuitEnum suit, RankEnum rank)
		{
			this.suit = suit;
			this.rank = rank;
		}
		
		public RankEnum Rank
		{
			get { return rank; }
		}
		
		public SuitEnum Suit
		{
			get { return suit; }
		}
		
		public override string ToString()
		{
			return suit.ToString().ToCharArray()[0] + ":" + rank.ToString();
		}

        public string ToSpeakableString()
        {
            return rank.ToString();
        }
	}
}
