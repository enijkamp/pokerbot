using System;

namespace PokerBot
{
    public class Rule
    {
        public const string NO_RULE = "No Rule";

        private StreetTypes street;
        private HandTypes hand;
        private ChanceTypes chance;
        private int minOpps, maxOpps;
        private OpponentActionTypes action;
        private PositionTypes position;
        private double minMaxBet, maxMaxBet;
        private double minPotSize, maxPotSize;
        private string decision;

        public Rule(StreetTypes street, HandTypes hand,
            ChanceTypes chance, int minOpps, int maxOpps,
            OpponentActionTypes action)
        {
            this.street = street;
            this.hand = hand;
            this.chance = chance;
            this.minOpps = minOpps;
            this.maxOpps = maxOpps;
            this.action = action;
            this.decision = NO_RULE;
        }

        public Rule(StreetTypes street, HandTypes hand,
                    ChanceTypes chance, int minOpps, int maxOpps,
                    OpponentActionTypes action,
                    PositionTypes position,
                    double minMaxBet, double maxMaxBet,
                    double minPotSize, double maxPotSize,
                    string decision)
        {
            this.street = street;
            this.hand = hand;
            this.chance = chance;
            this.minOpps = minOpps;
            this.maxOpps = maxOpps;
            this.action = action;
            this.position = position;
            this.minMaxBet = minMaxBet;
            this.maxMaxBet = maxMaxBet;
            this.minPotSize = minPotSize;
            this.maxPotSize = maxPotSize;
            this.decision = decision;
        }

        public StreetTypes Street { get { return street; } }
        public HandTypes Hand { get { return hand; } }
        public ChanceTypes Chance { get { return chance; } }
        public int MinOpponents { get { return minOpps; } }
        public int MaxOpponents { get { return maxOpps; } }
        public OpponentActionTypes OpponentAction { get { return action; } }
        public PositionTypes Position { get { return position; } }
        public double MinMaxBet { get { return minMaxBet; } }
        public double MaxMaxBet { get { return maxMaxBet; } }
        public double MinPotSize { get { return minPotSize; } }
        public double MaxPotSize { get { return maxPotSize; } }
        public string Decision { get { return decision; } }

        public override string ToString()
        {
            return "[ " + Hand + " | " + Street + " | " + Chance + " | " + minOpps + "," + maxOpps + " | " + action + " | " + decision + " ]";
        }
    }
}
