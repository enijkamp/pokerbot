using System;

namespace PokerBot
{
	public class TestEvaluateHand
	{      
		public static void Main(string[] args)
		{
			Console.WriteLine(new Hand("ah ac", "kd 8c 2c").Description);
		}
	}
}
