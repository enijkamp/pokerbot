using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PokerBot
{	
	public class CardStrategyFast : CardStrategy
	{
        private const double REGIONS_MIN_OVERLAP_SKIP = .4;
        private const double REGIONS_MIN_OVERLAP_ERROR = .9;

        private ColorReducer reducer = new ColorReducers.Card();
		private List<CardPattern> cards;
        private TableLayout layout;

        public CardStrategyFast(List<CardPattern> cards, TableLayout layout)
		{
			this.cards = cards;
            this.layout = layout;
            reducePattern();
		}


        private void reducePattern()
        {
            foreach (CardPattern pattern in cards)
            {
                pattern.Image = reducer.reduceColors(pattern.Image);
            }
        }
		
		public List<Card> identifyCards(List<CardRegion> regions)
		{
			// find cards in a sorted manner
			List<Card> sortedCards = new List<Card>();
            foreach (CardRegion region in regions) 
			{
                Log.Fine("x=" + region.x + " -> probability(card,screen)=" + region.overlap + " -> candidate=" + region.card);
                
                sortedCards.Add(region.card);
			}
			
			return sortedCards;
		}

        private Image cropAndReduceCard(Image screenImage, Point point, int cardWidth, int cardHeight)
        {
            // crop seems to be wrong by one pixel
            int OFFSET_Y_FIX = +1;

            // crop & reduce
            Image cropped = screenImage.crop(point.X, point.X + cardWidth, point.Y + OFFSET_Y_FIX, point.Y + OFFSET_Y_FIX + cardHeight);
            Image reduced = reducer.reduceColors(cropped);
            return reduced;                
        }

		public List<CardRegion> identifyRegions(Image screenImage)
		{
            // result
			List<CardRegion> regions = new List<CardRegion>();

            // card dimensions
            int cardWidth = cards[0].Image.width;
            int cardHeight = cards[0].Image.height;
			
			// scan horizontal
			foreach(Point point in layout.Cards)
			{
                Image screenCard = cropAndReduceCard(screenImage, point, cardWidth, cardHeight);
                float bestOverlap = 0.0f;
                CardPattern bestCard = null;
                foreach (CardPattern card in cards)
                {
                    // compare
                    bool[] matches = compare(card.Image, screenCard, point);
                    float overlap = percent(matches);

                    // debug
                    if (overlap > 0.1)
                    {
                        Log.Finest(point.X + " -> " + card + " " + overlap);
                    }

                    // no more cards
                    if (overlap < REGIONS_MIN_OVERLAP_SKIP)
                    {
                        return regions;
                    }
                    // best
                    else if (overlap > bestOverlap)
                    {
                        // store
                        bestOverlap = overlap;
                        bestCard = card;
                    }
                }
                // sanity check
                if (bestOverlap < REGIONS_MIN_OVERLAP_ERROR)
                {
                    ErrorHandler.BeepError();
                    ErrorHandler.ReportCardException(screenCard, "Best overlap " + bestOverlap + " < " + REGIONS_MIN_OVERLAP_ERROR + " for card " + bestCard);
                }
                // add best card to regions
                regions.Add(new CardRegion(bestCard, point.X, bestOverlap));
            }
			return regions;			
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
		
		private bool[] compare(Image cardImage, Image screenCard, Point point)
		{
			bool[] matches = new bool[cardImage.pixels.Length];
            for (int i = 0; i < cardImage.pixels.Length; i++)
            {
                int pixelCard = cardImage.pixels[i];
                int pixelScreen = screenCard.pixels[i];
                matches[i] = IsMatch(pixelCard, pixelScreen);
            }
            return matches;
		}

        private bool IsMatch(int pixel1, int pixel2)
        {
            if (pixel1 == Image.EmptyPixel || pixel2 == Image.EmptyPixel)
            {
                return true;
            }
            else
            {
                return pixel1 == pixel2;
            }
        }
	}
}
