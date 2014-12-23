using System;
using System.Collections.Generic;

namespace PokerBot
{
	public class TestRulesReader
	{
		public static void Main(string[] args)
        {
            string hand = "qh kc";
            string board = "qs 7c qc 8d 8s";
            HandAnalysis analysis = HandEvaluator.evalHandSmart(CardParser.parse(hand), CardParser.parse(board));
            Console.WriteLine("chance = " + analysis.Chance);
            Console.WriteLine("basic = " + analysis.HandBasic);
            Console.WriteLine("smart = " + analysis.HandSmart);

            List<Rule> rules = RulesReader.readRules();
            RuleEvaluator eval = new RuleEvaluator(rules);
            Rule rule = eval.findRule(StreetTypes.Turn, HandTypes.Top_pair, ChanceTypes.Draws, 4, OpponentActionTypes.Check, PositionTypes.Early, 0.5, 0.7);
            Console.WriteLine(rule.Decision);
            Console.ReadKey();
		}
	}
}