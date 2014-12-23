using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerBot
{
    public class LobbyTable
    {
        public static LobbyTable Empty = new LobbyTable(0, 0, 0, 0, 0, 0, 0, 0, false);

        public const int NO_VALUE = -1;

        public int Players = NO_VALUE;
        public double PotSize = NO_VALUE;
        public int PlayersFlop = NO_VALUE;
        public int AbsX, AbsY;
        public int RelX, RelY;
        public bool IsJoined;
        public int Num;

        public LobbyTable(int num, int players, double potsize, int playersflop, int absX, int absY, int relX, int relY, bool joined)
        {
            this.Num = num;
            this.Players = players;
            this.PotSize = potsize;
            this.PlayersFlop = playersflop;
            this.AbsX = absX;
            this.AbsY = absY;
            this.RelX = relX;
            this.RelY = relY;
            this.IsJoined = joined;
        }

        public double Score
        {
            get { return (Math.Sqrt(PotSize) * .3) + (PlayersFlop / 100.0); }
        }        

        public bool IsIncomplete
        {
            get
            {
                return Players == NO_VALUE || PotSize == NO_VALUE || PlayersFlop == NO_VALUE;
            }
        }

        public override string ToString()
        {
            return Num + ": x=" + AbsX + " y=" + AbsY + " players=" + Players + " pot=" + PotSize + " flop=" + PlayersFlop;
        }

        public string ToShortString()
        {
            return "   players=" + Players + "   pot=" + String.Format("{0:0.00}", PotSize) + "   flop=" + String.Format("{0:0.00}", PlayersFlop) + "   score=" + String.Format("{0:0.00}", Score);
        }

        public string ToCsvString()
        {
            return Players + ";" + String.Format("{0:0.00}", PotSize) + ";" + String.Format("{0:0.00}", PlayersFlop) + ";" + String.Format("{0:0.00}", Score);
        }  
    }
}
