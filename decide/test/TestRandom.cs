using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerBot
{
    public class TestRandom
    {
        public static void Main(string[] args)
        {
            Random rnd = new Random();
            Console.WriteLine(rnd.NextDouble().ToString());
            Console.WriteLine(rnd.NextDouble().ToString());
            Console.WriteLine(rnd.NextDouble().ToString());
            Console.ReadKey();
        }
    }
}
