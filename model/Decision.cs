using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerBot
{
    public class Decision
    {
        public enum Types
        {
            CHECK,
            RAISE,
            BET,
            CALL,
            FOLD
        }

        private Types type;
        private double amount = 0.0;
        private bool hasAmount;

        public Decision(Types type)
        {
            this.type = type;
            this.hasAmount = false;
        }

        public Decision(Types type, double amount)
        {
            this.amount = amount;
            this.type = type;
            this.hasAmount = true;
        }

        public Types DecisionType
        {
            get { return type; }
        }

        public bool HasAmount
        {
            get { return hasAmount; }
        }

        public double Amount
        {
            get { return amount; }
        }

        public override string ToString()
        {
            return type + (hasAmount ? " " + amount : "");
        }
    }
}
