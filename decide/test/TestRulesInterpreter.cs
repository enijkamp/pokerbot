using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerBot
{
    public class TestRulesInterpreter
    {
        private static string[] rules =
        {            
            "CHECK",
            "FOLD",
            "N/A",
            "BET MIN(.75POT)",
            "BET MAX(5POT)",
            "BET MAX(4BB, .6POT)",
            "BET MAX(1BB, .1POT)",
            "RAISE MAX(3X, .75POT, 15BB)",
            "ALL-IN",
            "RAISE MAX(.5POT, 6BB)"
        };

        public static void Main(string[] args)
        {
            Table table = new Table();
            table.Pot = 1;
            table.MaxBet = 5;
            foreach (string rule in rules)
            {
                Decision decision = new RuleInterpreter(0.01, 0.02).interpret(table, rule);
                Console.WriteLine(decision);
            }
            Console.ReadKey();
        }			
    }
}
