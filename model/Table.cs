using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerBot
{
	public class Table
	{
        private const int MIN_PLAYERS_SITOUT = 5;

        public const int NO_DEALER = -1;
        public const int NO_SEAT = -1;
        public const double NO_POT = -1;
        public const int NO_BLIND = -1;
        public const double NO_BET = -1;
        
        // players
		private List<Player> players = new List<Player>();
		private List<Card> community = new List<Card>();
		private List<Card> hand = new List<Card>();
		private int dealer = NO_DEALER;
		private int seat = NO_SEAT;
        private bool firstRound = true;
        private double pot = NO_POT;

        // controls
        private double maxBet = NO_BET;
		
		public Table()
		{
		}
	
		public List<Player> Players
		{
			get { return players; }
			set 
			{ 
				players.Clear();
				players.AddRange(value); 
			}
		}

        public Player MyPlayer
        {
            get { return players[seat]; }
        }
		
		public List<Card> Community
		{
			get { return community; }
			set 
			{ 
				community.Clear();
				community.AddRange(value); 
			}
		}
		
		public List<Card> Hand
		{
			get { return hand; }
			set 
			{ 
				hand.Clear();
				hand.AddRange(value); 
			}
		}
		
		public List<Card> Cards
		{
			get 
			{
				List<Card> cards = new List<Card>();
				cards.AddRange(community);
				cards.AddRange(hand);
				return cards; 
			}
		}
		
		public int Dealer
		{
			get { return dealer; }
			set { dealer = value; }
		}
		
		public int Seat
		{
			get { return seat; }
			set { seat = value; }
		}

        public bool HasSeat
        {
            get { return seat != NO_SEAT; }
        }

        public double MaxBet
        {
            get { return maxBet; }
            set { maxBet = value; }
        }

        public int SmallBlindSeat
        {
            get
            {
                if (Dealer == NO_DEALER) return -1;
                foreach (int position in FromToSeat(Dealer+1, Dealer))
                {
                    if (Players[position].HasBet)
                    {
                        return position;
                    }
                }
                return NO_BLIND;
            }
        }

        public int BigBlindSeat
        {
            get
            {
                if (Dealer == NO_DEALER || SmallBlindSeat == NO_BLIND) return NO_BLIND;
                foreach (int position in FromToSeat(SmallBlindSeat+1, Dealer))
                {
                    if (Players[position].HasBet)
                    {
                        return position;
                    }
                }
                return NO_BLIND;
            }
        }

        public bool IsMySmallBlind
        {
            get
            {
                return seat == SmallBlindSeat;
            }
        }        

        public bool IsMyBigBlind
        {
            get 
            {
                return seat == BigBlindSeat;                
            }
        }

        public double Pot
        {
            get { return pot; }
            set { pot = value; }
        }

        public List<double> Bets
        {
            get
            {
                List<double> bets = new List<double>();
                Players.ForEach(delegate(Player player) { bets.Add(player.Bet); });
                return bets;
            }
        }

        public bool IsFirstRound
        {
            get { return firstRound; }
            set { firstRound = value; }
        }

        public int PlayersWithMoney
        {
            get
            {
                int playersWithMoney = 0;
                foreach (Player player in Players)
                {
                    if (player.HasMoney) playersWithMoney++;
                }
                return playersWithMoney;
            }
        }

        public bool IsOutOfPlayers
        {
            get
            {
                return PlayersWithMoney <= MIN_PLAYERS_SITOUT;
            }
        }

        public List<int> FromToSeat(int start, int end)
        {
            List<int> seats = new List<int>();
            int current = ToSeat(start);
            int last = ToSeat(end);
            while (current != last)
            {
                seats.Add(current);
                current++;
                current = ToSeat(current);               
            }
            seats.Add(last);
            return seats;
        }

        public int ToSeat(int seat)
        {
            if (seat < 0)
            {
                seat += Players.Count;
            }
            if (seat > Players.Count - 1)
            {
                seat -= Players.Count;
            }
            return seat;
        }        
	}
}
