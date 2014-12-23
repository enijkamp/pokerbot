using System;
using System.Collections.Generic;

namespace PokerBot
{	
	public class SituationEvaluator
	{
        public static Situation evaluateSituation(Image tableImage, Table table, List<TableControl> controls, double bigblind)
		{
			// street
            StreetTypes street = getStreetType(table);
			
			// hand
            HandAnalysis analysis = HandEvaluator.evalHandSmart(table.Hand, table.Community);

			// opponents
			int opponents = (street == StreetTypes.Preflop ?
                             getPreflopNumberOfOpponents(table) :
                             getPostflopNumberOfOpponents(table));
			
			// action
			OpponentActionTypes action = (street == StreetTypes.Preflop ?
                                          getPreflopOpponentAction(tableImage, table, controls, bigblind) :
                                          getPostflopOpponentAction(tableImage, table, controls, bigblind));
			
            // position
            PositionTypes position = (street == StreetTypes.Preflop ?
                                          getPreflopPosition(table) :
                                          getPostflopPosition(table));

            return new Situation(street, analysis.HandSmart, analysis.Chance, opponents, action, position);
		}

        private static PositionTypes getPreflopPosition(Table table)
        {
            if (table.ToSeat(table.Dealer) == table.ToSeat(table.Seat) ||
                table.ToSeat(table.Dealer-1) == table.ToSeat(table.Seat))
            {
                return PositionTypes.Late;
            }
            else
            {
                return PositionTypes.Early;
            }
        }

        private static PositionTypes getPostflopPosition(Table table)
        {
            foreach (int seat in table.FromToSeat(table.Seat, table.Dealer))
            {
                Player player = table.Players[seat];
                if (player.HasCards)
                {
                    return PositionTypes.Early;
                }
            }
            return PositionTypes.Late;
        }

		private static StreetTypes getStreetType(Table table)
		{
			int card = table.Cards.Count;
			return (StreetTypes) card;
		}
        
		private static int getPostflopNumberOfOpponents(Table table)
		{
			int opponents = 0;
			foreach(Player player in table.Players)
			{
				if(player.HasCards)
				{
					opponents++;
				}
			}
			return opponents;
		}
		
		private static int getPreflopNumberOfOpponents(Table table)
		{
			int opponents = 0;
            if (table.IsMyBigBlind || table.IsMySmallBlind)
            {
                // small/big blind opponent count
                foreach(int seat in table.FromToSeat(table.Seat+1, table.Seat-1))
                {
                    Player player = table.Players[seat];
                    if (player.HasCards)
                    {
                        opponents++;
                    }
                }                                
            }
            else
            {
                // count from big blind to my seat
                foreach (int seat in table.FromToSeat(table.Dealer+2, table.Seat))
                {
                    Player player = table.Players[seat];
                    if (player.HasCards)
                    {
                        opponents++;
                    }
                }
            }
			return opponents;
		}

        private static bool isFirstToActPreflop(int dealer, int seat, Table table, double bigblind)
        {
            // pre-flop: sanity check if bet identification fails
            if (table.MaxBet > bigblind)
            {
                return false;
            }

            // pre-flop: is my seat right after big blind
            foreach (int pos in table.FromToSeat(table.BigBlindSeat + 1, seat))
            {
                if (table.Players[pos].HasCards && pos != seat)
                    return false;
            }
            return true;
        }

        private static bool isFirstToActPostflop(int dealer, int seat, Table table)
        {
            // pre-flop: sanity check if bet identification fails
            if (table.MaxBet > 0)
            {
                return false;
            }

            // post-flop: is my seat right after dealer
            foreach (int pos in table.FromToSeat(dealer + 1, seat))
            {
                if (table.Players[pos].HasCards && pos != seat)
                    return false;
            }
            return true;
        }

        private static OpponentActionTypes getPostflopOpponentAction(Image tableImage, Table table, List<TableControl> controls, double bigblind)
        {
			// from dealer to my position
			int dealer = table.Dealer;
			int seat = table.Seat;

            // round
            foreach (int i in table.FromToSeat(seat + 1, dealer))
            {
                // player states
				Player player = table.Players[i];

                bool hasFolded = player.HasFolded;
                bool hasCheck = !player.HasBet;

                // from seat+1 to dealer somebody has checked or folded -> not 1st round
                if (!hasFolded && !hasCheck)
                {
                    table.IsFirstRound = false;
                    break;
                }
            }

 			// 1-st round
            if (table.IsFirstRound)
            {
                // get first bet after dealer
                double maxBet = GetFirstBet(table, dealer + 1);
                bool raised = false;
                foreach (int i in table.FromToSeat(dealer + 1, seat))
                {
                    // player states
                    Player player = table.Players[i];

                    // seat not occupied or skipping
                    if (!player.IsExistent)
                    {                        
                        // this player is skipping
                        // the current round
                        player.Action = Player.Actions.NONE;
                        continue;
                    }

                    // fold
                    if (player.HasFolded)
                    {
                        player.Action = Player.Actions.FOLD;
                        continue;
                    }
                    // check
                    if (!player.HasBet)
                    {
                        player.Action = Player.Actions.CHECK;
                        continue;
                    }
                    // call
                    if (player.Bet == maxBet)
                    {
                        player.Action = Player.Actions.CALL;
                        continue;
                    }
                    // raise
                    if (!raised && player.Bet > maxBet)
                    {
                        raised = true;
                        maxBet = player.Bet;
                        player.Action = Player.Actions.RAISE;
                        continue;
                    }
                    // reraise
                    if (raised && player.Bet > maxBet)
                    {
                        maxBet = player.Bet;
                        player.Action = Player.Actions.RAISE;
                        continue;
                    }
                }

                // get 'highest' action
                Player.Actions maxAction = Player.Actions.NONE;
                foreach (Player player in table.Players)
                {
                    if (player.Action > maxAction)
                    {
                        maxAction = player.Action;
                    }
                }

                // mapping
                if (isFirstToActPostflop(dealer, seat, table))
                {
                    return OpponentActionTypes.First_to_act;
                }
                if (maxAction == Player.Actions.CALL)
                {
                    return OpponentActionTypes.Bet;
                }
                if (maxAction == Player.Actions.CHECK)
                {
                    // sanity check (could not read bet)
                    if (table.MaxBet > 0)
                    {
                        return OpponentActionTypes.Raise;
                    }
                    else
                    {
                        return OpponentActionTypes.Check;
                    }
                }
                if (maxAction == Player.Actions.RAISE)
                {
                    return OpponentActionTypes.Raise;
                }
                if (maxAction == Player.Actions.RERAISE)
                {
                    // TODO temporary disabled for 1/2 stakes
                    return OpponentActionTypes.Raise;
                }

                // error
                ErrorHandler.ReportExceptionWithImage(new Exception("opponent postflop action '" + maxAction + "' mismatch -> return raise"), "opponent action error", tableImage);
                return OpponentActionTypes.Raise;
            }
            // n-th round
            else            
            {
                // active players
                List<Player> activePlayers = new List<Player>();                
                foreach (Player player in table.Players)
                {
                    if (player.HasCards && player.HasBet)
                    {
                        activePlayers.Add(player);
                    }
                }
         
                // raise
                bool raised = false, reraised = false;
                for (int i = 1; i < activePlayers.Count; i++)
                {
                    Player previous = activePlayers[i-1];
                    Player current = activePlayers[i];
                    if (previous.Bet != current.Bet)
                    {
                        if (!raised)
                        {
                            raised = true;
                        }
                        else
                        {
                            reraised = true;
                        }
                    }
                }

                // action
                if (reraised)
                {
                    // temporary disabled for 1/2 stake rules
                    return OpponentActionTypes.Raise;
                }
                else if (raised)
                {
                    return OpponentActionTypes.Raise;
                }
                else
                {
                    return OpponentActionTypes.Bet;
                }
            }
       }

        private static double GetFirstBet(Table table, int position)
        {
            double bet = 0.0;
            foreach(int pos in table.FromToSeat(position, position-1))
            {
                if(table.Players[pos].HasBet)
                {
                    return table.Players[pos].Bet;
                }
            }
            return bet;
        }
		
	     private static OpponentActionTypes getPreflopOpponentAction(Image tableImage, Table table, List<TableControl> controls, double bigblind)
         {
            // from dealer to my position
            int dealer = table.Dealer;
            int seat = table.Seat;

            // PREFLOP actions
            double maxBet = bigblind;
            bool raised = false;
            foreach (int i in table.FromToSeat(table.BigBlindSeat + 1, seat))
            {
                // player states
                Player player = table.Players[i];

                // fold
                if (player.HasFolded)
                {
                    player.Action = Player.Actions.FOLD;
                    continue;
                }

                // call
                if (player.Bet == maxBet)
                {
                    player.Action = Player.Actions.CALL;
                    continue;
                }
                // raise
                if (!raised && player.Bet > maxBet)
                {
                    raised = true;
                    maxBet = player.Bet;
                    player.Action = Player.Actions.RAISE;
                    continue;
                }
                // reraise
                if (raised && player.Bet > maxBet)
                {
                    maxBet = player.Bet;
                    player.Action = Player.Actions.RERAISE;
                    continue;
                }
            }

            // get 'highest' action
            Player.Actions maxAction = Player.Actions.NONE;
            foreach (Player player in table.Players)
            {
                if (player.Action > maxAction)
                {
                    maxAction = player.Action;
                }
            }


            // # mapping #
            // first to act
            if (isFirstToActPreflop(dealer, seat, table, bigblind))
            {
                return OpponentActionTypes.First_to_act;
            }

            // max action: raise
            if (maxAction == Player.Actions.RAISE)
            {
                return OpponentActionTypes.Raise;
            }

            // max action: reraise
            if (maxAction == Player.Actions.RERAISE)
            {
                // TODO : temporary disabled for 1/2 stakes
                return OpponentActionTypes.Raise;
            }

            // controls: limp
            if (isCallSmallerOrEqualBB(table, bigblind))                
            {
                return OpponentActionTypes.Limp;
            }

            // controls: raise
            if (isCallBiggerBB(table, bigblind))
            {
                return OpponentActionTypes.Raise;
            }

            // error
            ErrorHandler.ReportExceptionWithImage(new Exception("opponent preflop action mismatch -> return raise"), "opponent action error", tableImage);
            return OpponentActionTypes.Raise;
        }

         private static bool isCallBiggerBB(Table table, double bigblind)
         {
             return table.MaxBet > bigblind;
         }

         private static bool isCallSmallerOrEqualBB(Table table, double bigblind)
         {
             return table.MaxBet <= bigblind;
         }
	}
}
