using System;

namespace PokerBot
{
	public class Situation
	{
		private StreetTypes street;
		private HandTypes hand;
		private ChanceTypes chance;
		private int opponents;
		private OpponentActionTypes action;
        private PositionTypes position;
		
		public Situation(StreetTypes street, HandTypes hand,
		            ChanceTypes chance, int opponents,
                    OpponentActionTypes action, PositionTypes position)
		{
			this.street = street;
			this.hand = hand;
			this.chance = chance;
			this.opponents = opponents;
			this.action = action;
            this.position = position;
		}
		
		public StreetTypes         Street         { get { return street; } }
		public HandTypes           Hand           { get { return hand; } }
		public ChanceTypes         Chance         { get { return chance; } }
		public int                 Opponents      { get { return opponents; } }
		public OpponentActionTypes OpponentAction { get { return action; } }
        public PositionTypes       Position       { get { return position; } }
	}
}
