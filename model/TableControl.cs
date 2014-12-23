using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerBot
{
    public class TableControl
    {
        public enum ControlType { NONE, FOLD, CHECK, CALL, RAISE, BET, POST_BLIND };

        public int Position;
        public ControlType Type;
        public double Amount;

        public TableControl(ControlType type, int position, double amount)
        {
            this.Position = position;
            this.Type = type;
            this.Amount = amount;
        }

        public override string ToString()
        {
            return Position + ": " + Type + " " + Amount;
        }
    }
}
