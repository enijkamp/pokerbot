using System;
using System.Collections.Generic;

namespace PokerBot
{	
    public class SmartEvaluator
    {
        private const int ACE = 14;
        private const int KING = 13;
        private const int QUEEN = 12;
        private const int JACK = 11;
        private const int TEN = 10;

        private const string SPADES = "s";
        private const string HEARTS = "h";
        private const string DIAMONDS = "d";
        private const string CLUBS = "c";

		
        private static Dictionary<char, int> cardMapping = new Dictionary<char, int>()
        {
            {'A', ACE},
            {'K', KING},
            {'Q', QUEEN},
            {'J', JACK},
            {'T', TEN},
            {'9', 9},
            {'8', 8},
            {'7', 7},
            {'6', 6},
            {'5', 5},
            {'4', 4},
            {'3', 3},
            {'2', 2},
            {'1', 1},
        };

		
        public static int[] getBoardRanks(string board)
        {
            // parse board hand
            string[] BoardCards = board.Split(' ');
			int[] BoardRanks = new int[BoardCards.Length];
            for (int i = 0; i < BoardCards.Length; i++)
            {
				char boardChar = BoardCards[i].Substring(0, 1).ToUpper().ToCharArray()[0];
				BoardRanks[i] = cardMapping[boardChar];
            }
            Array.Sort(BoardRanks);
			return BoardRanks;
        }
        public static int[] getUnsortedBoardRanks(string board)
        {
            // parse board hand
            string[] BoardCards = board.Split(' ');
            int[] BoardRanks = new int[BoardCards.Length];
            for (int i = 0; i < BoardCards.Length; i++)
            {
                char boardChar = BoardCards[i].Substring(0, 1).ToUpper().ToCharArray()[0];
                BoardRanks[i] = cardMapping[boardChar];
            }
            return BoardRanks;
        }
		
        private static string[] getBoardSuits(string board)
        {
            // parse board hand
            string[] BoardCards = board.Split(' ');
			string[] BoardSuits = new string[BoardCards.Length];
            for(int i = 0; i < BoardCards.Length ; i++)
            {
                BoardSuits[i] = BoardCards[i].Substring(1, 1).ToLower();
            }
			return BoardSuits;
        }

        private struct Pairs { public bool BoardPaired, BoardTwoPaired; public int FirstPair, SecondPair; }
		
        private static Pairs boardPairs(int[] BoardRanks)
        {
            // check for board pair
            bool BoardPaired = false;
            bool BoardTwoPaired = false;
            int lastPair = 0;
            int secondPair = 0;
            int t = 0;
 
            // #@%*(@%(*#*%*@#%(*(*#@(*%(*)%@#(* TWO PAIRED DOESNT WORK UNLESS A A A 2 2, A A 2 2 doesnt #$*(@$*@(#*$(*@#$*(@(*#$(*#@$
            foreach (int i in BoardRanks)
            {
                t++;
                for (int j = t; j < BoardRanks.Length; j++)
                {
                    if (i == BoardRanks[j] && BoardPaired && lastPair != i) { BoardTwoPaired = true; secondPair = i; break; }
                    if (i == BoardRanks[j]) { BoardPaired = true; lastPair = i; }
                    
                }
            }
			
            Pairs pairs = new Pairs();
            pairs.BoardPaired = BoardPaired;
            pairs.BoardTwoPaired = BoardTwoPaired;
            pairs.FirstPair = lastPair;
            pairs.SecondPair = secondPair;
            return pairs;
        }


        public static ChanceTypes evalChance(List<Card> hand, HandTypes smart, string board)
        {
            // parse board hand
            string[] BoardSuits = getBoardSuits(board);
            int[] BoardRanks = getBoardRanks(board);
            Array.Sort(BoardRanks);
			
            // pairs
            Pairs pairs = boardPairs(BoardRanks);

            // straight dangers
            int maxConnected = 1;
            int tempConnected = 1;
            int Holes = 0;
            for (int i = 1; i < BoardRanks.Length ; i++)
            {
                if (BoardRanks[i] == BoardRanks[i - 1] + 1 && Holes < 2)
                {
                    tempConnected++;
                    if (maxConnected < tempConnected) { maxConnected = tempConnected; };
                }
                else if (BoardRanks[i] == BoardRanks[i - 1] + 2 && Holes < 1)
                {
                    tempConnected++;
                    Holes++;
                    if (maxConnected < tempConnected) { maxConnected = tempConnected; };
                }
                else
                {
                    if (maxConnected < tempConnected) { maxConnected = tempConnected; };
                    Holes = 0;
                    tempConnected = 1;
                }


            }

			
            // ############################ BOARD TYPE ######################################
            int[] SuitCount = {0,0,0,0};
            foreach (string s in BoardSuits)
            {
                if (s == "h") { SuitCount[0]++; }
                else if (s == "d") { SuitCount[1]++; }
                else if (s == "c") { SuitCount[2]++; }
                else if (s == "s") { SuitCount[3]++; }
 
            }
            Array.Sort(SuitCount);
 
            if (SuitCount[3] > 3 || pairs.BoardTwoPaired || (maxConnected > 3 && Holes == 0) || maxConnected >4)
            {
                return ChanceTypes.Extreme;
            }
            else if (SuitCount[3] == 3 || maxConnected > 3)
            {
                // opponent could have hit flush
                return ChanceTypes.GotHit;
            }
            else if (pairs.BoardPaired)
            {
                // opponent could have trips/full house
                if (smart == HandTypes.Trips || smart == HandTypes.Straight)
                {
                    // we got trips/full house
                    return ChanceTypes.Safe;
                }
                else if (smart == HandTypes.AA_KK)
                {
                    // if low boardpair, still safe for AA/KK
                    if (pairs.FirstPair < JACK) { return ChanceTypes.Safe; };
                    return ChanceTypes.GotHit;
                }
                else
                {
                    return ChanceTypes.GotHit;
                }
            }
            else if (BoardRanks.Length == 5)
            {
                return ChanceTypes.Safe;
            }
            else if (SuitCount[3] == 2)
            {
                return ChanceTypes.Draws;
            }
            else 
            {
                return ChanceTypes.Safe;
            }
        }

        // PREFLOP
        public static HandTypes evalHand(string hand)
        {
            // parse personal hand
            string[] PersonalCards = hand.Split(' ');
            string[] PersonalSuits = new string[PersonalCards.Length];
            int[] PersonalRanks = new int[PersonalCards.Length];
            for (int i = 0; i < PersonalCards.Length; i++)
            {
                PersonalSuits[i] = PersonalCards[i].Substring(1, 1).ToLower();
            }
            for (int i = 0; i < PersonalCards.Length; i++)
            {
                char cardChar = PersonalCards[i].Substring(0, 1).ToUpper().ToCharArray()[0];
                PersonalRanks[i] = cardMapping[cardChar];
            }

            // pairs
            if (PersonalRanks[0] == PersonalRanks[1])
            {
                if (PersonalRanks[0] == ACE || PersonalRanks[0] == KING)
                {
                    return HandTypes.AA_KK;
                }
                else if (PersonalRanks[0] == QUEEN)
                {
                    return HandTypes.QQ;
                }
                else
                {
                    return HandTypes.Low_Pair;
                }
            }

            // ace king
            Array.Sort(PersonalRanks);
            if (PersonalRanks[1] == ACE && PersonalRanks[0] == KING)
            {
                return HandTypes.AK;
            }
            // ace queen suited
            Array.Sort(PersonalRanks);
            if (PersonalRanks[1] == ACE && PersonalRanks[0] == QUEEN)
            {
                return HandTypes.AQ;
            }
            
            return HandTypes.None;
        }
        
        // POSTFLOP
        public static HandTypes evalHand(Hand.HandTypes eval, string hand, string board)
        {
            // parse board hand
            string[] BoardSuits = getBoardSuits(board);
            int[] BoardRanks = getBoardRanks(board);
 
            // parse personal hand
            string[] PersonalCards = hand.Split(' ');
            string[] PersonalSuits = new string[PersonalCards.Length];
            int[] PersonalRanks = new int[PersonalCards.Length];
            for (int i = 0; i < PersonalCards.Length ; i++)
            {
                PersonalSuits[i] = PersonalCards[i].Substring(1, 1).ToLower();
            }
            for (int i = 0; i < PersonalCards.Length; i++)
            {
				char cardChar = PersonalCards[i].Substring(0, 1).ToUpper().ToCharArray()[0];
				PersonalRanks[i] = cardMapping[cardChar];
            }
            if (PersonalRanks[0] > PersonalRanks[1])
            {
                int tempI = PersonalRanks[0];
                PersonalRanks[0] = PersonalRanks[1];
                PersonalRanks[1] = tempI;
 
                string tempS = PersonalSuits[0];
                PersonalSuits[0] = PersonalSuits[1];
                PersonalSuits[1] = tempS;
            }
 
            // ############################ HAND TYPE ######################################
            // check for board pair
            Pairs pairs = boardPairs(BoardRanks);


            // ######### FLUSH ANALYSIS #################
            // parse personal hand UNSORTED
            string[] PersonalCardsUNSORTED = hand.Split(' ');
            string[] PersonalSuitsUNSORTED = new string[PersonalCardsUNSORTED.Length];
            int[] PersonalRanksUNSORTED = new int[PersonalCards.Length];
            for (int i = 0; i < PersonalCardsUNSORTED.Length; i++)
            {
                PersonalSuitsUNSORTED[i] = PersonalCardsUNSORTED[i].Substring(1, 1).ToLower();
            }
            for (int i = 0; i < PersonalCardsUNSORTED.Length; i++)
            {
                char cardChar = PersonalCardsUNSORTED[i].Substring(0, 1).ToUpper().ToCharArray()[0];
                PersonalRanksUNSORTED[i] = cardMapping[cardChar];
            }


            //SPADES,CLUBS,DIAMONDS,HEARTS
            int[] numBoardSuits = { 0, 0, 0, 0 };
            int[] numTotalSuits = { 0, 0, 0, 0 };

            int firstPersonalCard = -1;
            int secondPersonalCard = -1;

            foreach (string s in BoardSuits)
            {
                if (s == SPADES) { numBoardSuits[0]++; numTotalSuits[0]++; };
                if (s == CLUBS) { numBoardSuits[1]++; numTotalSuits[1]++; };
                if (s == DIAMONDS) { numBoardSuits[2]++; numTotalSuits[2]++; };
                if (s == HEARTS) { numBoardSuits[3]++; numTotalSuits[3]++; };
            }

            foreach (string s in PersonalSuitsUNSORTED)
            {
                if (firstPersonalCard == -1)
                {
                    if (s == SPADES) { numTotalSuits[0]++; firstPersonalCard = 0; };
                    if (s == CLUBS) { numTotalSuits[1]++; firstPersonalCard = 1; };
                    if (s == DIAMONDS) { numTotalSuits[2]++; firstPersonalCard = 2; };
                    if (s == HEARTS) { numTotalSuits[3]++; firstPersonalCard = 3; };
                }
                else
                {
                    if (s == SPADES) { numTotalSuits[0]++; secondPersonalCard = 0; };
                    if (s == CLUBS) { numTotalSuits[1]++; secondPersonalCard = 1; };
                    if (s == DIAMONDS) { numTotalSuits[2]++; secondPersonalCard = 2; };
                    if (s == HEARTS) { numTotalSuits[3]++; secondPersonalCard = 3; };
                }
            }

            bool FlushDraw = false;
            for (int x = 0; x < 4; x++)
            {
                if (numTotalSuits[x] == 4 && numTotalSuits[x] - numBoardSuits[x] == 2 && BoardRanks.Length < 5)
                {
                    //  flushdraw if kicker higher than than J kicker

                    if (PersonalRanks[1] > JACK) { FlushDraw = true; }
                }
                else if (numTotalSuits[x] == 4 && numTotalSuits[x] - numBoardSuits[x] == 1 && BoardRanks.Length < 5)
                {
                    // flushdraw if flushindex higher than queen
                    if (firstPersonalCard == x)
                    {
                        if (PersonalRanksUNSORTED[0] > QUEEN) { FlushDraw = true; }
                    }
                    else
                    {
                        if (PersonalRanksUNSORTED[1] > QUEEN) { FlushDraw = true; }
                    }
                }
            }




 
            // ## AK ##
            if (eval == Hand.HandTypes.HighCard)
            {
                if (FlushDraw)
                {
                    return HandTypes.AK_high_flush_draw;
                }
                else if (PersonalRanks[0] == KING && PersonalRanks[1] == ACE)
                {
                    return HandTypes.AK_overcards_ace_high;
                }
                else
                {
                    return HandTypes.None;
                }
            }
			
            // ## ONE PAIR ##
            else if (eval == Hand.HandTypes.Pair)
            {
 
                // pocket pair
                if (PersonalRanks[0] == PersonalRanks[1])
                {
                    if ((PersonalRanks[0] == ACE || PersonalRanks[0] == KING) && FlushDraw)
                    {
                        return HandTypes.AA_KK_plus_draw;
                    }
                    else if (PersonalRanks[0] == KING && BoardRanks[BoardRanks.Length - 1] == ACE)
                    {
                        return HandTypes.KK_with_A_board;
                    }
                    else if (PersonalRanks[0] == ACE || PersonalRanks[0] == KING)
                    {
                        return HandTypes.AA_KK;
                    }
                    else if (PersonalRanks[0] > BoardRanks[BoardRanks.Length - 1] && PersonalRanks[0] > JACK)
                    {
                        return HandTypes.Top_pair;
                    }
                    else if (FlushDraw)
                    {
                        return HandTypes.AK_high_flush_draw;
                    }
                    else if (PersonalRanks[0] > BoardRanks[BoardRanks.Length - 1])
                    {
                        return HandTypes.Weak_top_pair;
                    }
                    else if (PersonalRanks[0] == QUEEN)
                    {
                        int[] boardranks2 = getUnsortedBoardRanks(board);
                        if (boardranks2[0] < QUEEN && boardranks2[1] < QUEEN && boardranks2[2] < QUEEN)
                        {
                            return HandTypes.Top_pair;
                        }
                        return HandTypes.Mid_pair;
                    }
                    else
                    {   
                        return HandTypes.Mid_pair;
                    }
 
                }
                // neither pocket pair nor board pair
                else
                {
                    if (PersonalRanks[0] == BoardRanks[BoardRanks.Length - 1] &&
                        PersonalRanks[0] > JACK &&
                        (PersonalRanks[1] == QUEEN || PersonalRanks[1] == KING || PersonalRanks[1] == ACE || PersonalRanks[1] == JACK) ||
                        PersonalRanks[1] == BoardRanks[BoardRanks.Length - 1] &&
                        PersonalRanks[1] > JACK &&
                        (PersonalRanks[0] == QUEEN || PersonalRanks[0] == KING || PersonalRanks[0] == ACE || PersonalRanks[0] == JACK))
                    {
                        return HandTypes.Top_pair;
                    }
                    else if (FlushDraw)
                    {
                        return HandTypes.AK_high_flush_draw;
                    }
                    else if (PersonalRanks[0] == BoardRanks[BoardRanks.Length - 1] ||
                        PersonalRanks[1] == BoardRanks[BoardRanks.Length - 1])
                    {
                        return HandTypes.Weak_top_pair;
                    }

                    else if (pairs.BoardPaired && PersonalRanks[0] == KING && PersonalRanks[1] == ACE)
                    {
						return HandTypes.AK_overcards_ace_high;
                    }
                    else if (pairs.BoardPaired)
                    {
						//return HandTypes.Just_board_paired;
                        return HandTypes.None;
                    }
                    else
                    {
                        string board2 = board;
                        if (board.Length > 8) { board2 = board.Remove(8); }
                        int[] boardranks2 = getBoardRanks(board2);
                        if (PersonalRanks[0] == boardranks2[boardranks2.Length - 1] &&
                            PersonalRanks[0] > JACK &&
                            (PersonalRanks[1] == QUEEN || PersonalRanks[1] == KING || PersonalRanks[1] == ACE || PersonalRanks[1] == JACK) ||
                            PersonalRanks[1] == boardranks2[boardranks2.Length - 1] &&
                            PersonalRanks[1] > JACK &&
                            (PersonalRanks[0] == QUEEN || PersonalRanks[0] == KING || PersonalRanks[0] == ACE || PersonalRanks[0] == JACK))
                        {
                            return HandTypes.Top_pair;
                        }


                        return HandTypes.Mid_pair;
                    }
                }
            }
			
            // ## TWO PAIR ##
            else if (eval == Hand.HandTypes.TwoPair)
            {
                if (pairs.BoardPaired)
                {
                    // pocket pair
                    if (PersonalRanks[0] == PersonalRanks[1])
                    {
                        if ((PersonalRanks[0] == ACE || PersonalRanks[0] == KING) && FlushDraw)
                        {
                            return HandTypes.AA_KK_plus_draw;
                        }
                        else if (PersonalRanks[0] == KING && BoardRanks[BoardRanks.Length - 1] == ACE)
                        {
                            return HandTypes.KK_with_A_board;
                        }
                        else if (PersonalRanks[0] == ACE || PersonalRanks[0] == KING)
                        {
                            return HandTypes.AA_KK;
                        }
                        else if (PersonalRanks[0] > BoardRanks[BoardRanks.Length - 1] && PersonalRanks[0] > JACK)
                        {
                            return HandTypes.Top_pair;
                        }
                        else if (FlushDraw)
                        {
                            return HandTypes.AK_high_flush_draw;
                        }
                        else if (PersonalRanks[0] > BoardRanks[BoardRanks.Length - 1])
                        {
                            return HandTypes.Weak_top_pair;
                        }
                        else if (PersonalRanks[0] == QUEEN)
                        {
                            int[] boardranks2 = getUnsortedBoardRanks(board);
                            if (boardranks2[0] < QUEEN && boardranks2[1] < QUEEN && boardranks2[2] < QUEEN)
                            {
                                return HandTypes.Top_pair;
                            }
                            return HandTypes.Mid_pair;
                        }

                        else
                        {
                            return HandTypes.Mid_pair;
                        }

                    }
                    // neither pocket pair nor board pair
                    else
                    {
                        if (PersonalRanks[0] == BoardRanks[BoardRanks.Length - 1] &&
                            PersonalRanks[0] > JACK &&
                            (PersonalRanks[1] == QUEEN || PersonalRanks[1] == KING || PersonalRanks[1] == ACE || PersonalRanks[1] == JACK) ||
                            PersonalRanks[1] == BoardRanks[BoardRanks.Length - 1] &&
                            PersonalRanks[1] > JACK &&
                            (PersonalRanks[0] == QUEEN || PersonalRanks[0] == KING || PersonalRanks[0] == ACE || PersonalRanks[0] == JACK))
                        {
                            return HandTypes.Top_pair;
                        }
                        else if (FlushDraw )
                        {
                            return HandTypes.AK_high_flush_draw;
                        }
                        else if (PersonalRanks[0] == BoardRanks[BoardRanks.Length - 1] ||
                      
                            PersonalRanks[1] == BoardRanks[BoardRanks.Length - 1])
                        {
                            return HandTypes.Weak_top_pair;
                        }

                        else if (pairs.BoardPaired && PersonalRanks[0] == KING && PersonalRanks[1] == ACE)
                        {
                            return HandTypes.AK_overcards_ace_high;
                        }
                        else
                        {
                            string board2 = board;
                            if (board.Length > 8) { board2 = board.Remove(8); }
                            int[] boardranks2 = getBoardRanks(board2);
                            if (PersonalRanks[0] == boardranks2[boardranks2.Length - 1] &&
                                PersonalRanks[0] > JACK &&
                                (PersonalRanks[1] == QUEEN || PersonalRanks[1] == KING || PersonalRanks[1] == ACE || PersonalRanks[1] == JACK) ||
                                PersonalRanks[1] == boardranks2[boardranks2.Length - 1] &&
                                PersonalRanks[1] > JACK &&
                                (PersonalRanks[0] == QUEEN || PersonalRanks[0] == KING || PersonalRanks[0] == ACE || PersonalRanks[0] == JACK))
                            {
                                return HandTypes.Top_pair;
                            }
                            return HandTypes.Mid_pair;
                        }
                    }
                }
                else
                {
                    return HandTypes.Two_pair;
                }
            }
			
            // ## TRIPS ##
            else if (eval == Hand.HandTypes.Trips)
            {
                bool BoardTripped = false;
                int tempPair2 = 0;


                foreach (int i in BoardRanks)
                {
                    foreach (int j in BoardRanks)
                    {
                        if (i == j) { tempPair2++; };
                        if (tempPair2 > 2) { BoardTripped = true; };
                    }
                    tempPair2 = 0;
                }
 
                //board not tripped and high kicker = trips
                if (!BoardTripped) 
                {
                    if (PersonalRanks[0] == PersonalRanks[1]) { return HandTypes.Set; }
                    else
                    {
                        if((PersonalRanks[0] == pairs.FirstPair || PersonalRanks[0] == pairs.SecondPair) &&
                            (PersonalRanks[1] == KING || PersonalRanks[1] == QUEEN || PersonalRanks[1] == ACE || PersonalRanks[1] == JACK))
                        {
                            return HandTypes.Trips;
                        }
                        else if ((PersonalRanks[1] == pairs.FirstPair || PersonalRanks[1] == pairs.SecondPair) &&
                            (PersonalRanks[0] == KING || PersonalRanks[0] == QUEEN || PersonalRanks[0] == ACE || PersonalRanks[0] == JACK))
                        {
                            return HandTypes.Trips;
                        }
                        // #################### TWO_PAIR But then it says gothit.. ########################
                        return HandTypes.Two_pair;
                    }
                }
                else
                { 
                    //GOTO HIGH CARD..basically jackshit ; 
                    return HandTypes.None;
                }
            }
			
            // ## STRAIGHT ##
            else if (eval == Hand.HandTypes.Straight)
            {
                /*
                int Connected = 1;
                int Holes = 0;
                for (int i = 1; i < BoardRanks.Length; i++)
                {
                    if (BoardRanks[i] == BoardRanks[i - 1] + 1)
                    {
                        Connected++;
                    }
                    else if ((BoardRanks[i] == BoardRanks[i - 1] + 2) &&
                    ((PersonalRanks[0] == BoardRanks[i] - 1) ||
                    (PersonalRanks[1] == BoardRanks[i] - 1)))
                    {
                        Connected++;
                        Holes++;
                    }
                    else if ((BoardRanks[i] == PersonalRanks[0] + 1) || (BoardRanks[i] == PersonalRanks[1] + 1))
                    {
                        Connected++;
                    }
                    else 
                    {
                        if (BoardRanks.Length > i + 1 && BoardRanks[i + 1] != BoardRanks[i] + 2)
                        {
                            if ((BoardRanks[i] == PersonalRanks[0] - 1) || (BoardRanks[i] == PersonalRanks[1] - 1))
                            {
                                Connected++;
                                Holes++;
                            }
                            else if (Connected < 5)
                            {
                                Connected = 1;
                                Holes = 1;
                            }
                        }
                        

                    }
                }*/

                int Connected = 1;
                bool Safe = true;
                for (int i = 1; i < BoardRanks.Length; i++)
                {
                    if (BoardRanks[i] == BoardRanks[i - 1] + 1)
                    {
                        Connected++;
                        if (Connected > 3)
                        {
                            Safe = false;
                            if (PersonalRanks[0] -1 == BoardRanks[i] || PersonalRanks[1] -1  == BoardRanks[i])
                            {
                                Safe = true; 
                            }
                        }
                    }
                    else if (Connected < 4)
                    {
                        Connected = 1;
                    }
                }

                if (Safe)
                {
                    return HandTypes.Straight;
                }
                return HandTypes.Top_pair;
            }
			
            // ## FLUSH ##
            else if (eval == Hand.HandTypes.Flush)
            {

                for (int x = 0; x < 4; x++)
                {
                    if (numTotalSuits[x] == 5 && numTotalSuits[x]-numBoardSuits[x] == 2)
                    {
                        //HighFlush if Queen King or Ace
                        if (PersonalRanks[1] > QUEEN) { return HandTypes.AK_high_flush; }
                        return HandTypes.Low_flush;
                    }
                    else if (numTotalSuits[x] > 5 && numBoardSuits[x] < numTotalSuits[x])
                    {
                        //HighFlush if Ace or King
                        if (firstPersonalCard == x)
                        {
                            if (PersonalRanksUNSORTED[0] > QUEEN) { return HandTypes.AK_high_flush; }
                        }
                        if (secondPersonalCard == x)
                        {
                            if (PersonalRanksUNSORTED[1] > QUEEN) { return HandTypes.AK_high_flush; }
                        }
                        return HandTypes.Mid_pair;
                    }
                    
                    
                    
                }
                //just board flushed
                return HandTypes.Mid_pair;
                
            }
			
            // ## FULL HOUSE ##
            else if (eval == Hand.HandTypes.FullHouse)
            {
                bool BoardTripped = false;
                int tempPair2 = 0;


                foreach (int i in BoardRanks)
                {
                    foreach (int j in BoardRanks)
                    {
                        if (i == j) { tempPair2++; };
                        if (tempPair2 > 2) { BoardTripped = true; };
                    }
                    tempPair2 = 0;
                }

                if (BoardTripped)
                {/*
                    if (PersonalRanks[0] == PersonalRanks[1])
                    {
                        if (PersonalRanks[0] == ACE || PersonalRanks[0] == KING || PersonalRanks[0] == QUEEN || PersonalRanks[0] == JACK)
                        {
                            return HandTypes.AK_high_flush;
                        }
                        else
                        {
                            return HandTypes.Top_pair;
                        }
                    }
                    else*/
                    {
                        Array.Sort(BoardRanks);
                        if ((BoardRanks[BoardRanks.Length - 1] == PersonalRanks[0] ||
                             BoardRanks[BoardRanks.Length - 1] == PersonalRanks[1]) &&
                             (BoardRanks[BoardRanks.Length - 1] == ACE || BoardRanks[BoardRanks.Length - 1] == KING || BoardRanks[BoardRanks.Length - 1] == QUEEN || BoardRanks[BoardRanks.Length - 1] == JACK))
                        {
                            return HandTypes.Low_flush;
                        }
                        else if (PersonalRanks[0] == PersonalRanks[1] && (PersonalRanks[0] == ACE || PersonalRanks[0] == KING || PersonalRanks[0] == QUEEN || PersonalRanks[0] == JACK))
                        {
                            return HandTypes.Low_flush;
                        }
                        else
                        {
                            return HandTypes.Top_pair;
                        }
                    }
                         
                }
                
                if (pairs.BoardTwoPaired)
                {
                    if (((PersonalRanks[0] == pairs.FirstPair || PersonalRanks[1] == pairs.FirstPair) && pairs.FirstPair > pairs.SecondPair) ||
                        ((PersonalRanks[0] == pairs.SecondPair || PersonalRanks[1] == pairs.SecondPair) && pairs.FirstPair < pairs.SecondPair))
                    {
                        return HandTypes.AK_high_flush;
                    }
                    else
                    {
                        return HandTypes.Low_flush;
                    }
                }

                if (pairs.BoardPaired)
                {
                    return HandTypes.AK_high_flush;
                }


                
                return HandTypes.Low_flush;
            }
            // ## QUADS ##
            else if (eval == Hand.HandTypes.FourOfAKind)
            {
                bool BoardQuads = false;
                int tempPair2 = 0;
                int notQuadded = 0;
                foreach (int i in BoardRanks)
                {
                    foreach (int j in BoardRanks)
                    {
                        if (i == j) { tempPair2++; }
                        else {notQuadded = i;}
                        if (tempPair2 > 3) { BoardQuads = true; };
                    }
                    tempPair2 = 0;
                }

                if (!BoardQuads) { return HandTypes.AK_high_flush; }
                else if(notQuadded == ACE) { return HandTypes.AK_high_flush;}
                else if(PersonalRanks[0] == ACE || PersonalRanks[1] == ACE) { return HandTypes.AK_high_flush;}
                else { return HandTypes.None;}
            }
            // ## STRAIGHTFLUSH ##
            else if (eval == Hand.HandTypes.StraightFlush)
            {
                return HandTypes.AK_high_flush;
            }

            return HandTypes.None;
        }
    }
}
