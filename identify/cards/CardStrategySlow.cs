using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PokerBot
{	
	public class CardStrategySlow : CardStrategy
	{
        private const double REGIONS_SCANNED_CARD_HEIGHT = 0.75;
        private const int REGIONS_MIN_SPACE_BETWEEN_CARDS = 3;
        private const double REGIONS_X_SCAN_MIN_MATCH = 0.72;
        private const double REGIONS_X_SCAN_MIN_MATCH_5_6_OR_8_9 = 0.78;

        private const double CARDS_MIN_OVERLAP_FOR_SKIP = 0.7;
		
		
		private readonly List<CardPattern> cards;
		private readonly int maxCardWidth;
		
		public CardStrategySlow(List<CardPattern> cards)
		{
			this.cards = cards;
			this.maxCardWidth = getMaxCardWidth(cards);
		}		
				
		private int getMaxCardWidth(List<CardPattern> cards)
		{
			int width = 0;
			foreach(CardPattern pattern in cards)
			{
				if(pattern.Image.width > width)
					width = pattern.Image.width;
			}
			return width;			
		}
		
		public List<Card> identifyCards(List<CardRegion> regions)
		{
			// find best overlaps
			Dictionary<int, CardRegion> bestOverlaps = new Dictionary<int, CardRegion>();
			foreach(CardRegion region in regions)
			{
                // debug
                if (region.overlap > 0.1) Log.Finest(region.x + " -> " + region.card + " " + region.overlap);

                // store best overlap for x coordinate
				if(!bestOverlaps.ContainsKey(region.x))
				{
                    // first hit
					bestOverlaps[region.x] = region;
				}
				else
				{
                    // better overlap
					if(region.overlap > bestOverlaps[region.x].overlap)
						bestOverlaps[region.x] = region;
				}
			}
			
			// sort keys according to x
			int[] sortedXCoordinates = new int[bestOverlaps.Keys.Count];
            bestOverlaps.Keys.CopyTo(sortedXCoordinates, 0);
            Array.Sort(sortedXCoordinates);

			// find cards in a sorted manner
			List<Card> sortedCards = new List<Card>();
            foreach (int x in sortedXCoordinates) 
			{
                if (bestOverlaps[x].overlap >= CARDS_MIN_OVERLAP_FOR_SKIP)
				{
					Log.Fine("x="+x+" -> probability(card,screen)="+bestOverlaps[x].overlap+" -> candidate="+bestOverlaps[x].card);
					sortedCards.Add(bestOverlaps[x].card);
				}
			}
			
			return sortedCards;
		}

		public List<CardRegion> identifyRegions(Image screenImage)
		{		
			List<CardRegion> regions = new List<CardRegion>();
			
			// scan horizontal
			int reachableScanWidth = screenImage.width - maxCardWidth;
			for(int x = 0; x < reachableScanWidth; x++)
			{
				foreach(CardPattern card in cards)
				{					
					// card image
					Image cardImage = card.Image;
					
					// compare
					bool[] matches = compare(cardImage, screenImage, x);
					float overlap = percent(matches);
					
					// region
					CardRegion region = new CardRegion(card, x, overlap);
					regions.Add(region);
					
					// skip if mininum overlap reached
                    if (overlap >= GetMinimumOverlaphForCard(region)) 
					{
                        x += cardImage.width + REGIONS_MIN_SPACE_BETWEEN_CARDS;
						break;
					}
				}			
			}
			return regions;			
		}

        private double GetMinimumOverlaphForCard(CardRegion card)
        {
            if (card.card.Rank == Card.RankEnum.Five ||
                card.card.Rank == Card.RankEnum.Six ||
                card.card.Rank == Card.RankEnum.Eigth ||
                card.card.Rank == Card.RankEnum.Nine)
            {
                return REGIONS_X_SCAN_MIN_MATCH_5_6_OR_8_9;
            }
            else
            {
                return REGIONS_X_SCAN_MIN_MATCH;
            }
        }
		
		private float percent(bool[] booleans)
		{
			float size = booleans.Length;
			float count = 0;
			foreach(bool boolean in booleans)
				if(boolean)
					count++;
			return count / size;
		}
		
		private bool[] compare(Image cardImage, Image screenImage, int x)
		{
			bool[] matches = new bool[cardImage.height];
			for(int yImage = 0, yCard = 0; yImage < screenImage.height; yImage++)
			{				
				// get lines
				int[] imageLine = screenImage.getHorizontalLine(yImage);
				
				// iterate over all card lines
				if(yCard == 0)
				{
                    int maxCardHeight = (int)(cardImage.height * REGIONS_SCANNED_CARD_HEIGHT);
					for(int yCurrentCard = 0; yCurrentCard < maxCardHeight; yCurrentCard++)
					{
						// flags
						int[] cardLine = cardImage.getHorizontalLine(yCurrentCard);
						bool isMatch = IsMatch(imageLine, cardLine, x);

						// check
						Debug.Assert(!IsTransparent(cardLine));
						
						// match
						if(isMatch)
						{
							yCard = yCurrentCard;
							matches[yCard] = true;
							break;
						}
					}					
				}				
				// continue with next card line
				else
				{
					// next line
					yCard++;
					
					// check
					if(yCard >= cardImage.height) break;

					// pixels
					int[] cardLine = cardImage.getHorizontalLine(yCard);
					
					// check
					bool isMatch = IsMatch(imageLine, cardLine, x);				
						
					// match
					if(isMatch)
					{
						matches[yCard] = true;		
					}
				}
			}
			return matches;
		}
		
		private bool IsMatch(int[] imageLine, int[] cardLine, int x)
		{
			int minWidth = imageLine.Length < cardLine.Length ? imageLine.Length : cardLine.Length;
			for(int i = 0; i < minWidth; i++)
			{
				if(cardLine[i] == Image.EmptyPixel)
				{
					continue;
				}
				else if(cardLine[i] != imageLine[i+x]) 
				{
					return false;
				}
			}
			return true;
		}
		
		private bool IsTransparent(int[] line)
		{
			foreach(int pixel in line)
			{
				if(pixel != Image.EmptyPixel)
					return false;
			}
			return true;
		}
	}
}
