using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerBot
{
    public class CardParser
    {
        public static List<Card> parse(string cards)
        {
		    // rank
			Dictionary<char, Card.RankEnum> ranks = new Dictionary<char, Card.RankEnum>();
			ranks.Add('2', Card.RankEnum.Two);
			ranks.Add('3', Card.RankEnum.Three);
			ranks.Add('4', Card.RankEnum.Four);
			ranks.Add('5', Card.RankEnum.Five);
			ranks.Add('6', Card.RankEnum.Six);
			ranks.Add('7', Card.RankEnum.Seven);
			ranks.Add('8', Card.RankEnum.Eigth);
			ranks.Add('9', Card.RankEnum.Nine);
			ranks.Add('T', Card.RankEnum.Ten);
			ranks.Add('A', Card.RankEnum.Ace);
			ranks.Add('J', Card.RankEnum.Jack);
			ranks.Add('K', Card.RankEnum.King);
			ranks.Add('Q', Card.RankEnum.Queen);

            // suit
			Dictionary<char, Card.SuitEnum> suits = new Dictionary<char, Card.SuitEnum>();
			suits.Add('C', Card.SuitEnum.Clubs);
			suits.Add('D', Card.SuitEnum.Diamonds);
			suits.Add('H', Card.SuitEnum.Hearts);
			suits.Add('S', Card.SuitEnum.Spades);

		    // parse
            string[] parts = cards.ToUpper().Split(new char[] { ' ' });
            List<Card> result = new List<Card>();
            if (cards.Length == 0)
            {
                return result;
            }
            else
            {
                foreach (string part in parts)
                {
                    Card.RankEnum rank = ranks[part[0]];
                    Card.SuitEnum suit = suits[part[1]];
                    result.Add(new Card(suit, rank));
                }
                return result;
            }
        }
    }
}
