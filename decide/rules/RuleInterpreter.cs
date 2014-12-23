using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerBot
{
    public class RuleInterpreter
    {
        private string ALL_IN = "ALL-IN";

        private Dictionary<string, Decision.Types> types = new Dictionary<string, Decision.Types>()
        {
            {"CHECK", Decision.Types.CHECK},
            {"RAISE", Decision.Types.RAISE},
            {"BET",   Decision.Types.BET},
            {"CALL",  Decision.Types.CALL},
            {"FOLD",  Decision.Types.FOLD},
            {"N/A",   Decision.Types.FOLD}
        };

        delegate double Operation(List<double> values);
        private Dictionary<string, Operation> operations = new Dictionary<string, Operation>()
        {
            {"MIN", delegate(List<double> args){ return args.Min(); }},
            {"MAX", delegate(List<double> args){ return args.Max(); }}
        };

        private double smallBlind, bigBlind;
        private static Random random = new Random();

        public RuleInterpreter(double smallBlind, double bigBlind)
        {
            this.smallBlind = smallBlind;
            this.bigBlind = bigBlind;
        }

        public void precheck(List<Rule> rules)
        {
            Table table = new Table();
            table.Pot = 0.5;
            foreach (Rule rule in rules)
            {
                try
                {
                    interpret(table, rule.Decision);
                }
                catch (Exception)
                {
                    throw new ArgumentException("Error '" + rule.Decision + "' for Rule -> " + rule.ToString());
                }
            }
        }

        public Decision interpret(Table table, string action)
        {
            if (action == Rule.NO_RULE)
            {
                // check
                if (table.MyPlayer.Bet >= table.MaxBet)
                {
                    return new Decision(Decision.Types.CHECK);
                }

                // no bets
                bool betsExist = false;
                foreach (Player player in table.Players)
                {
                    if (player.HasBet)
                    {
                        betsExist = true;
                    }
                }
                return new Decision(!betsExist ? Decision.Types.CHECK : Decision.Types.FOLD);
            }
            else if (action == ALL_IN)
            {
                int amount = (int)(10 + (Math.Abs(random.NextDouble()) * 5));
                return new Decision(Decision.Types.RAISE, amount);
            }
            else
            {
                // parse rule
                Queue<string> terms = generateQueue(action);

                // decision
                Decision.Types type = evalType(terms);

                // amount
                if (terms.Count == 0)
                {
                    return new Decision(type);
                }
                else
                {
                    double amount = evalAmount(table, terms);
                    return new Decision(type, amount);
                }
            }
        }

        private Queue<string> generateQueue(string action)
        {
            Queue<string> queue = new Queue<string>();
            foreach (string key in types.Keys)
            {
                if (action.StartsWith(key))
                {
                    string type = key;
                    queue.Enqueue(type);
                    if (action.Trim().Length > key.Length)
                    {
                        string amount = action.Replace(key, string.Empty).Trim();
                        queue.Enqueue(amount);
                    }
                }
            }
            return queue;
        }

        private double evalAmount(Table table, Queue<string> terms)
        {           
            string term = terms.Dequeue();
            if (isOperation(term))
            {
                return evalOperation(table, term);
            }
            else
            {
                return evalVariable(table, term);
            }
        }

        private bool isOperation(string term)
        {
            foreach(string key in operations.Keys)
            {
                if(term.Contains(key)) return true;
            }
            return false;
        }

        private string getOperation(string term)
        {
            foreach (string key in operations.Keys)
            {
                if (term.Contains(key)) return key;
            }
            throw new ArgumentException();
        }

        private double evalOperation(Table table, string term)
        {
            string argsTerm = substring(term, '(', ')');
            string[] args = argsTerm.Split(',');
            List<double> values = new List<double>();
            foreach (string arg in args)
            {
                values.Add(evalVariable(table, arg));
            }
            return operations[getOperation(term)](values);
        }

        private string substring(string str, char ch1, char ch2)
        {
            int start = str.IndexOf(ch1) + 1;
            int end = str.IndexOf(ch2);
            return str.Substring(start, end - start);
        }

        private Decision.Types evalType(Queue<string> terms)
        {
            string term = terms.Dequeue().ToUpper();
            if (!types.ContainsKey(term))
            {
                Log.Error("Unknown Rule Term = " + term);
                throw new ArgumentException("Unknown Rule Term = " + term);
            }
            return types[term];
        }

        private double evalVariable(Table table, string term)
        {
            if (term.Contains("POT"))
            {
                return evalFactor(term, "POT") * table.Pot;
            }

            if (term.Contains("BB"))
            {
                return evalFactor(term, "BB") * bigBlind;
            }

            if (term.Contains("SB"))
            {
                return evalFactor(term, "SB") * smallBlind;
            }

            if (term.Contains("X"))
            {
                return evalFactor(term, "X") * table.MaxBet;
            }

            throw new ArgumentException("Unknown variable " + term);
        }

        private double evalFactor(string term, string identifier)
        {
            term = term.Trim();
            if (term.Length == identifier.Length)
            {
                return 1;
            }
            else
            {
                string factorTerm = term.Replace(identifier, string.Empty);
                if (factorTerm.StartsWith("."))
                {
                    return TextTools.ParseDouble("0" + factorTerm);
                }
                else
                {
                    return TextTools.ParseDouble(factorTerm);
                }
            }
        }
    }
}
