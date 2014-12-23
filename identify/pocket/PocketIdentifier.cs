using System;
using System.Collections.Generic;
using System.Drawing;

namespace PokerBot
{	
	public class PocketIdentifier
	{
		private const int PATTERN_X = 2;
		private const int PATTERN_WIDTH = 12;
		private const int PATTERN_Y = 5;
		private const int PATTERN_HEIGHT = 30;
		
		private const int SECOND_CARD_X = 15;
		private const int SECOND_CARD_Y = 4;
		
		private readonly List<CardPattern> cards;

		public PocketIdentifier(List<CardPattern> cards)
		{
			this.cards = generatePatterns(cards);
		}
		
		private List<CardPattern> generatePatterns(List<CardPattern> patterns)
		{
			List<CardPattern> hands = new List<CardPattern>();
			foreach(CardPattern pattern in patterns)
			{
				int startX = PATTERN_X;
				int endX = PATTERN_X + PATTERN_WIDTH;
				int startY = PATTERN_Y;
				int endY = PATTERN_Y + PATTERN_HEIGHT;
				CardPattern hand = new CardPattern(pattern.Image.crop(startX, endX, startY, endY), 
				                                   pattern.Suit, 
				                                   pattern.Rank);
				hands.Add(hand);
			}
			return hands;
		}
		
		public List<Card> identifyCards(Image image) 
		{			
			// scan horizontal for first card
			int reachableScanHeight = image.height - PATTERN_HEIGHT;
			int reachableScanWidth = image.width - PATTERN_WIDTH;
			for(int x = 0; x < reachableScanWidth; x++)
			{
				for(int y = 0; y < reachableScanHeight; y++)
				{
					foreach(CardPattern card in cards)
					{
						if(isMatch(x, y, image, card))
						{
							// hand
							List<Card> hand = new List<Card>();
							hand.Add(card);
							hand.Add(getSecondCard(image, x, y));
							return hand;
						}
					}
				}
			}			
			return new List<Card>();			
		}
		
		private Card getSecondCard(Image image, int x, int y)
		{
			int secondHardX = x + SECOND_CARD_X;
			int secondHardY = y + SECOND_CARD_Y;
			foreach(CardPattern card in cards)
			{
				if(isMatch(secondHardX, secondHardY, image, card))
				{
					return card;
				}
			}
			throw new Exception("Cannot identify second card");
		}
		
		private bool isMatch(int x, int y, Image image, CardPattern card)
		{            
            // match
			for(int line = 0; line < card.Image.height; line++)
			{
                if((line + y) >= image.height)
                {
                    return false;
                }
				int[] cardLine = card.Image.getHorizontalLine(line);
				int[] imageLine = image.getHorizontalLine(x, y + line);
				if(!isLineMatch(cardLine, imageLine, cardLine.Length))
				{
					return false;
				}
			}
			return true;
		}
		
		private bool isLineMatch(int[] cardLine, int[] imageLine, int length)
		{
			for(int i = 0; i < length; i++)
			{
				if(cardLine[i] != imageLine[i])
				{
					return false;
				}
			}
			return true;
		}

	}
}
