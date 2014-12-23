using System;

namespace PokerBot
{
	public class CardPattern : Card
	{
		private Image image;
		
		public CardPattern(Image image, SuitEnum suit, RankEnum rank) : base(suit, rank)
		{
			this.image = image;
		}
		
		public Image Image
		{
			get { return image; }
            set { image = value; }
		}
	}
}
