using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerBot
{
    public class TestHandEvaluator
    {
        public static void Main(string[] args)
        {
            // preflop
            {
                string hand = "kh as";
                string board = "";
                HandAnalysis analysis = HandEvaluator.evalHandSmart(CardParser.parse(hand), CardParser.parse(board));
                Console.WriteLine("chance = " + analysis.Chance);
                Console.WriteLine("basic = " + analysis.HandBasic);
                Console.WriteLine("smart = " + analysis.HandSmart);
            }

            // postflop
            {
                string hand = "qh kc";
                string board = "qs 7c qc 8d 8s";
                HandAnalysis analysis = HandEvaluator.evalHandSmart(CardParser.parse(hand), CardParser.parse(board));
                Console.WriteLine("chance = " + analysis.Chance);
                Console.WriteLine("basic = " + analysis.HandBasic);
                Console.WriteLine("smart = " + analysis.HandSmart);
                Console.ReadKey();
            }
        }
    }
}
