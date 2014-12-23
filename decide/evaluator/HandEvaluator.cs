using System;
using System.Collections.Generic;

namespace PokerBot
{
	public class HandEvaluator
	{
		public static HandAnalysis evalHandSmart(List<Card> hand, List<Card> board)
		{
            // preflop
            if(board.Count == 0)
            {
                Log.Fine("evaluating preflop cards -> {" + map(hand) + "}");
    			HandTypes smart = SmartEvaluator.evalHand(map(hand));
    			return new HandAnalysis(smart);
            }
			// postflop
            else
            {
                Log.Fine("evaluating postflop cards -> {" + map(hand) + "}  {" + map(board) + "}");
    			Hand.HandTypes basic = evalHand(hand, board).HandTypeValue;
    			HandTypes smart = SmartEvaluator.evalHand(basic, map(hand), map(board));
                ChanceTypes chance = SmartEvaluator.evalChance(hand, smart, map(board));
    			return new HandAnalysis(basic, smart, chance);
            }
		}
		
		public static Hand evalHand(List<Card> hand, List<Card> board)
		{
			// eval
		    return new Hand(map(hand), map(board));
		}

		private static string map(List<Card> cards)
		{
		    // mapping
			Dictionary<Card.RankEnum, char> ranks = new Dictionary<Card.RankEnum, char>();
			ranks.Add(Card.RankEnum.Two, '2');
			ranks.Add(Card.RankEnum.Three, '3');
			ranks.Add(Card.RankEnum.Four, '4');
			ranks.Add(Card.RankEnum.Five, '5');
			ranks.Add(Card.RankEnum.Six, '6');
			ranks.Add(Card.RankEnum.Seven, '7');
			ranks.Add(Card.RankEnum.Eigth, '8');
			ranks.Add(Card.RankEnum.Nine, '9');
			ranks.Add(Card.RankEnum.Ten, 'T');
			ranks.Add(Card.RankEnum.Ace, 'A');
			ranks.Add(Card.RankEnum.Jack, 'J');
			ranks.Add(Card.RankEnum.King, 'K');
			ranks.Add(Card.RankEnum.Queen, 'Q');
		    // build
 			String ids = "";
			foreach(Card card in cards)
			{
				char suit = card.Suit.ToString().ToLower().ToCharArray()[0];
			    char rank = ranks[card.Rank];
				ids += rank.ToString() + suit.ToString() + " ";
			}
			ids = ids.Trim();
			return ids;
		}
	}
}
