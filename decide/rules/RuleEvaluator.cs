using System;
using System.Collections.Generic;

namespace PokerBot
{	
	public class RuleEvaluator
	{
        private class Interval : Object
        {
            private double min = 0, max = 1;

            public Interval(double min, double max)
            {
                this.min = min;
                this.max = max;
            }

            public bool contains(double value)
            {
                return value >= min && value <= max;
            }

            public override bool Equals(Object obj)
            {
                Interval interval = obj as Interval;
                return min == interval.min && max == interval.max;
            }

            public override int GetHashCode()
            {
                return min.GetHashCode() ^ max.GetHashCode();
            }
        }

        private class MaxBet : Interval { public MaxBet(double min, double max) : base(min, max) { } }
        private class PotSize : Interval { public PotSize(double min, double max) : base(min, max) { } }

        private class IntervalDictionary<I, T> : Dictionary<I, T> where I : Interval
        {
            public T getByInterval(double value)
            {
                foreach (I interval in this.Keys)
                {
                    if (interval.contains(value))
                    {
                        return this[interval];
                    }
                }

                throw new ArgumentException("No interval defined for '"+value+"'");
            }
        }

        private class PotSizeDictionary : IntervalDictionary<PotSize, Rule> { }
        private class MaxBetDictionary : IntervalDictionary<MaxBet, PotSizeDictionary> { }
        private class RuleDictionary : Dictionary<int, MaxBetDictionary> { }

        private RuleDictionary rules = new RuleDictionary();
		
		public RuleEvaluator(List<Rule> rules)
		{
			foreach(Rule rule in rules)
			{
				for(int opps = rule.MinOpponents; opps <= rule.MaxOpponents; opps++)
				{                    
                    if (rule.Position == PositionTypes.All)
                    {
                        {
                            int hash = getHashCode(rule.Street, rule.Hand, rule.Chance, opps, 
                                rule.OpponentAction, PositionTypes.Early);
                            addRule(hash, rule);
                        }
                        {
                            int hash = getHashCode(rule.Street, rule.Hand, rule.Chance, opps, 
                                rule.OpponentAction, PositionTypes.Late);
                            addRule(hash, rule);
                        }
                    }
                    else
                    {
                        int hash = getHashCode(rule.Street, rule.Hand, rule.Chance, opps, rule.OpponentAction, rule.Position);
                        addRule(hash, rule);
                    }
				}
			}
		}

        private void addRule(int hash, Rule rule)
        {
            // hash
            if (!rules.ContainsKey(hash))
            {
                rules.Add(hash, new MaxBetDictionary());
            }

            // bet
            MaxBetDictionary maxBetDict = rules[hash];
            MaxBet maxBet = new MaxBet(rule.MinMaxBet, rule.MaxMaxBet);
            if (!maxBetDict.ContainsKey(maxBet))
            {
                maxBetDict.Add(maxBet, new PotSizeDictionary());
            }

            // pot
            PotSizeDictionary potSizeDict = maxBetDict[maxBet];
            PotSize potSize = new PotSize(rule.MinPotSize, rule.MaxPotSize);
            if (!potSizeDict.ContainsKey(potSize))
            {
                potSizeDict.Add(potSize, rule);
            }
        }
		
		public Rule findRule(StreetTypes street, HandTypes hand, 
		                     ChanceTypes chance, int opponents,
		                     OpponentActionTypes action,
                             PositionTypes position,
                             double maxBet, double potSize)
		{
            // hash
            int hash = getHashCode(street, hand, chance, opponents, action, position);
			if(!rules.ContainsKey(hash))
			{
                Log.Debug("cannot find rule for this constellation -> " 
				                 + describe(street, hand, chance, opponents, action));
                return new Rule(street, hand, chance, opponents, opponents, action);
			}

            // intervals
			MaxBetDictionary maxBetDict = rules[hash];
            PotSizeDictionary potSizeDict = maxBetDict.getByInterval(maxBet);
            Rule rule = potSizeDict.getByInterval(potSize);

            return rule;
		}
		
		private string describe(Rule rule, int opponents)
		{
			return describe(rule.Street, rule.Hand, rule.Chance, opponents, rule.OpponentAction);
		}
		
		private string describe(StreetTypes street, HandTypes hand, 
		                     ChanceTypes chance, int opponents,
		                     OpponentActionTypes action)
		{
			return hand + " " + street + " " + chance + " " + opponents + " " + action;
		}
		
		private int getHashCode(StreetTypes street, HandTypes hand, 
		                        ChanceTypes chance, int opponents,
		                        OpponentActionTypes action,
                                PositionTypes position)
		{
			// ints
			int streetNum = (int) street;
			int handNum = (int) hand;
			int chanceNum = (int) chance;
			int actionNum = (int) action;
            int posNum = (int) position;
			
			// bits
			int bits = (streetNum   & mask(4));
			bits |=    (handNum     & mask(6)) << 4;
			bits |=    (chanceNum   & mask(4)) << 10;
			bits |=    (opponents   & mask(4)) << 14;
			bits |=    (actionNum   & mask(4)) << 18;
            bits |=    (posNum      & mask(2)) << 22;
			
			// hash
			return bits;
		}
		
		private static int mask(int len)
		{
			int mask = 0;
			for(int i = 0; i < len; i++)
			{
				mask |= 1 << i;
			}
			return mask;
		}
	}
}
