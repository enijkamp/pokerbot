using System;
using System.Drawing;

namespace PokerBot
{	
	public class Player
    {
        public enum States
        {
            NON_EXISTENT,
            EXISTENT,
            FOLDED_CARDS,
            HAS_CARDS
        }
        
        public enum Actions
        {
            NONE,
            FOLD,           
            CHECK,
            CALL,
            BET,
            RAISE,
            RERAISE
        }

        public const double NO_BET = -1;
        public const double NO_MONEY = -1;
        
		private Rectangle nameRect, moneyRect;
		private int position;
		private String name = String.Empty;
        private double money = NO_MONEY;		
        private double bet;
		private States state = States.NON_EXISTENT;
        private Actions action = Actions.NONE;
		
		public Player()
		{
		}
		
		public Rectangle NameRect
		{
            get { return nameRect; }
            set { nameRect = value; }
		}

        public Rectangle MoneyRect
        {
            get { return moneyRect; }
            set { moneyRect = value; }
        }
		
		public String Name
		{
			get { return name; }
			set { name = value; }
		}
		
		public double Money
		{
			get { return money; }
			set { money = value; }
		}
		
		public double Bet
		{
			get { return bet; }
			set { bet = value; }
		}   
        
        public States State
        {
            get { return state; }
            set { state = value; }
        }
        
        public Actions Action
        {
            get { return action; }
            set { action = value; }
        }
        		
		public bool IsExistent
		{
			get { return state != States.NON_EXISTENT; }
		}
        
		public bool HasFolded
		{
			get { return state == States.FOLDED_CARDS; }
		}
        
		public bool HasCards
		{
			get { return state == States.HAS_CARDS; }
		}

        public bool HasMoney
        {
            get { return money != NO_MONEY; }
        }

        public bool HasBet
        {
            get { return bet != NO_BET; }
        }     
		
		public int Position
		{
			get { return position; }
			set { position = value; }
		}
	}
}
