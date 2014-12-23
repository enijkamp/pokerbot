using System;
using System.Collections.Generic;

namespace PokerBot
{
	public class TestSeatPositions
	{
        public static void Main(string[] args)
        {
            Table table = new Table();
            table.Players = new System.Collections.Generic.List<Player>();
            for (int i = 0; i < 9; i++)
            {
                table.Players.Add(new Player());
            }
            foreach (int seat in table.FromToSeat(2, 0))
                Console.WriteLine(seat);
            Console.ReadKey();
        }
	}
}
