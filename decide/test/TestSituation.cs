using System;
using System.Collections.Generic;

namespace PokerBot
{
    public class TestSituation
    {
        public static void Main(string[] args)
        {
            string hand = "2d 2s";
            string board = "9d 2h 7s 9s Ts";
            StreetTypes street = StreetTypes.River;
            int opps = 1;
            OpponentActionTypes action = OpponentActionTypes.Raise;
            double maxBet = 1.26;
            double potSize = 0.64;

            HandAnalysis analysis = HandEvaluator.evalHandSmart(CardParser.parse(hand), CardParser.parse(board));
            Console.WriteLine("chance = " + analysis.Chance);
            Console.WriteLine("basic = " + analysis.HandBasic);
            Console.WriteLine("smart = " + analysis.HandSmart);

            List<Rule> rules = RulesReader.readRules();
            RuleEvaluator eval = new RuleEvaluator(rules);
            Rule rule = eval.findRule(street, analysis.HandSmart, analysis.Chance, opps, action, PositionTypes.Early, maxBet, potSize);
            Console.WriteLine("rule = " + rule.Decision);

            Console.ReadKey();
        }
    }
}