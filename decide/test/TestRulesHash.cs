using System;

namespace PokerBot
{
	public class TestRulesHash
	{
		public static void Main(string[] args)
        {
            Console.WriteLine((int)StreetTypes.Preflop);

            printBitMask(22);
            Console.WriteLine();
		
			int streetNum = (int) StreetTypes.Preflop;
			int handNum = (int) 2;
			int chanceNum = (int) 3;
			int oppNum = 4;
            int actionNum = (int)5;

            int bits = (streetNum & mask(4));
            bits |= (handNum & mask(6)) << 4;
            bits |= (chanceNum & mask(4)) << 10;
            bits |= (oppNum & mask(4)) << 14;
            bits |= (actionNum & mask(4)) << 18;
			
			printBitMask(bits);

            Console.ReadKey();
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
		
		private static void printBitMask(int mask)
		{
			for(int i = 32; i >= 0; i--)
			{
				int pos = (int) Math.Pow(2, i);
				bool bit = (mask & pos) > 0;
				Console.Write(bit ? "1" : "0");
			}
		}
	}
}
