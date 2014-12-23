using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
 
namespace PokerBot
{
    class TestSmartEvaluator
    {
		private const string BOARD = "As Kh 2d";
		private const string POCKET = "As Kh";
		private const string EVAL = "high card";
 
        public static void Main(string[] args)
        {
            // parse board hand
            string [] BoardCards = BOARD.Split(' ');
            string[] BoardSuits = new string[BoardCards.Length];
            int[] BoardRanks = new int[BoardCards.Length ];
            for(int i = 0; i < BoardCards.Length ; i++)
            {
                BoardSuits[i] = BoardCards[i].Substring(1, 1).ToLower();
            }
            for (int i = 0; i < BoardCards.Length; i++)
            {
                if (BoardCards[i].Substring(0, 1).ToUpper() == "A")
                {
                    BoardRanks[i] = 14;
                }
                else if (BoardCards[i].Substring(0, 1).ToUpper() == "K")
                {
                    BoardRanks[i] = 13;
                }
                else if (BoardCards[i].Substring(0, 1).ToUpper() == "K")
                {
                    BoardRanks[i] = 12;
                }
                else if (BoardCards[i].Substring(0, 1).ToUpper() == "J")
                {
                    BoardRanks[i] = 11;
                }
                else if (BoardCards[i].Substring(0, 1).ToUpper() == "T")
                {
                    BoardRanks[i] = 10;
                }
                else
                {
                    BoardRanks[i] = Convert.ToInt32(BoardCards[i].Substring(0, 1));
                }
            }
            Array.Sort(BoardRanks);
 
            // parse personal hand
            string[] PersonalCards = POCKET.Split(' ');
            string[] PersonalSuits = new string[PersonalCards.Length];
            int[] PersonalRanks = new int[PersonalCards.Length];
            for (int i = 0; i < PersonalCards.Length ; i++)
            {
                PersonalSuits[i] = PersonalCards[i].Substring(1, 1).ToLower();
            }
            for (int i = 0; i < PersonalCards.Length; i++)
            {
                if (PersonalCards[i].Substring(0, 1).ToUpper() == "A")
                {
                    PersonalRanks[i] = 14;
                }
                else if (PersonalCards[i].Substring(0, 1).ToUpper() == "K")
                {
                    PersonalRanks[i] = 13;
                }
                else if (PersonalCards[i].Substring(0, 1).ToUpper() == "K")
                {
                    PersonalRanks[i] = 12;
                }
                else if (PersonalCards[i].Substring(0, 1).ToUpper() == "J")
                {
                    PersonalRanks[i] = 11;
                }
                else if (PersonalCards[i].Substring(0, 1).ToUpper() == "T")
                {
                    PersonalRanks[i] = 10;
                }
                else
                {
                    PersonalRanks[i] = Convert.ToInt32(PersonalCards[i].Substring(0, 1));
                }
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
            bool BoardPaired = false;
            bool BoardTwoPaired = false;
            int lastPair = 0;
            int t = 0;
 
            // #@%*(@%(*#*%*@#%(*(*#@(*%(*)%@#(* TWO PAIRED DOESNT WORK UNLESS A A A 2 2, A A 2 2 doesnt #$*(@$*@(#*$(*@#$*(@(*#$(*#@$
            foreach (int i in BoardRanks)
            {
                t++;
                if(t<BoardRanks.Length)
                {
                    for (int j = t; j < BoardRanks.Length - 1;j++ )
                    {
                        if (i == BoardRanks[j] && BoardPaired && lastPair!=i) { BoardTwoPaired = true;  }
                        if (i == BoardRanks[j]) { BoardPaired = true; lastPair = i; }
                    }
                }
 
            }
 
            // check for flush draw
            bool FlushDraw = false;
            int FlushDrawIndex = 0;
            int Suit1 = 1;
            int Suit2 = 1;
            if( PersonalSuits[0] == PersonalSuits[1] )
            {
                Suit1 = 2;
                foreach (string i in BoardSuits)
                {
                    if(i==PersonalSuits[0]){Suit1++;}
                }
                FlushDrawIndex = 1;
            }
            else
            {
                foreach (string j in BoardSuits)
                {
                    if (PersonalSuits[0] == j) { Suit1++; }
                    if (PersonalSuits[1] == j) { Suit2++; }
                }
                if (Suit1 < Suit2) { FlushDrawIndex = 1; }
            }
            if (Math.Max(Suit1, Suit2) == 4) { FlushDraw = true;  }
 
 
 
 
            if (EVAL.ToLower() == "high card")
            {
                if (FlushDraw &&( PersonalRanks[FlushDrawIndex] == 13 || PersonalRanks[FlushDrawIndex] == 14))
                {
                    MessageBox.Show("A/K high flush draw");
                }
                else if (PersonalRanks[0] == 13 && PersonalRanks[1] == 14)
                {
                    MessageBox.Show("AK Overcards, Ace high");
                }
                else
                {
                    MessageBox.Show("Jackshit");
                }
            }
            else if (EVAL.ToLower() == "one pair")
            {
 
                // pocket pair
                if (PersonalRanks[0] == PersonalRanks[1])
                {
                    if (PersonalRanks[0] == 14 || PersonalRanks[0] == 13 && FlushDraw)
                    {
                        MessageBox.Show("AA/KK +Draw");
                    }
                    else if (PersonalRanks[0] == 13 && BoardRanks[BoardRanks.Length - 1] == 14)
                    {
                        MessageBox.Show("AA/KK w/A board");
                    }
                    else if (PersonalRanks[0] == 14 || PersonalRanks[0] == 13)
                    {
                        MessageBox.Show("AA/KK");
                    }
                    else if (PersonalRanks[0] > BoardRanks[BoardRanks.Length - 1])
                    {
                        MessageBox.Show("Overpair");
                    }
                    else if (FlushDraw && (PersonalRanks[FlushDrawIndex] == 13 || PersonalRanks[FlushDrawIndex] == 14))
                    {
                        MessageBox.Show("A/K high flush draw");
                    }
                    else
                    {
                        MessageBox.Show("Midpair");
                    }
 
                }
                // neither pocket pair nor board pair
                else
                {
                    if (
                        PersonalRanks[0] == BoardRanks[BoardRanks.Length - 1] ||
                        PersonalRanks[1] == BoardRanks[BoardRanks.Length - 1])
                    {
                        MessageBox.Show("Top pair");
                    }
                    else if (FlushDraw && (PersonalRanks[FlushDrawIndex] == 13 || PersonalRanks[FlushDrawIndex] == 14))
                    {
                        MessageBox.Show("A/K high flush draw");
                    }
                    else if (BoardPaired && PersonalRanks[0] == 13 && PersonalRanks[1] == 14)
                    {
                        MessageBox.Show("AK Overcards, Board Paired");
                    }
                    else if (BoardPaired)
                    {
                        MessageBox.Show("Just board paired");
                    }
                    else
                    {
                        MessageBox.Show("Midpair");
                    }
                }
            }
            else if (EVAL.ToLower() == "two pair")
            {
                if (BoardPaired)
                {
                    //GOTO ONE PAIR
 
                }
                else
                {
                    MessageBox.Show("Two Pair");
                }
            }
            else if (EVAL.ToLower() == "trips")
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
 
                //unless board has three of the same, its trips
                if (!BoardTripped) { MessageBox.Show("Three of a kind"); }
                else
                { //GOTO HIGH CARD..basically jackshit ; 
                }
            }
            else if (EVAL.ToLower() == "straight")
            {
                // only rare straight vs straight
                MessageBox.Show("Straight");
            }
            else if (EVAL.ToLower() == "flush")
            {
                // High flush if one of flushsuit in your hand is A/K, otherwise lowflush
                // ##################################
                // NEEDS TO BE IMRPOVED IF A/K FLUSHSUIT ON BOARD Q/J ARE HIGH FLUSHES... 
                // #################################
                if (PersonalRanks[FlushDrawIndex] == 13 || PersonalRanks[FlushDrawIndex] == 14)
                {
                    MessageBox.Show("A/K high flush");
                }
                else
                {
                    MessageBox.Show("Low Flush");
                }
            }
            else if (EVAL.ToLower() == "full house")
            {
                // not sure yet, prob just full house for now
                MessageBox.Show("Full house");
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
 
            if (SuitCount[3] > 3 || BoardTwoPaired)
            {
                MessageBox.Show("Extreme");
            }
            else if (SuitCount[3] == 3 || BoardPaired)
            {
                MessageBox.Show("GotHit");
            }
            else if (SuitCount[3] == 2)
            {
                MessageBox.Show("Draws");
            }
            else
            {
                MessageBox.Show("Safe");
            }
        }
    }
}