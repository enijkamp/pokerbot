using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PokerBot
{
    public class TestStuff
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (i == 3)
                    {
                        break;
                    }
                }
            }


            List<TableContainer> containers = new List<TableContainer>();
            containers.Add(TableContainer.EmptyContainer());
            if (containers.Contains(TableContainer.EmptyContainer()))
            {
                Console.WriteLine("argh");
            }
            else
            {
                Console.WriteLine("ok");
            }

            Thread.Sleep(2000);
            Iterator<Image> screen = new ScreenImageIterator(new Win32Control());
            Image s1 = screen.next();
            Image s2 = screen.next();

            DateTime start = DateTime.Now;
            areImagesEqual(s1, s1);

            Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds / 1000);
            Console.ReadKey();
        }

        private static bool areImagesEqual(Image img1, Image img2)
        {
            int[] timg1 = img1.pixels;
            int[] timg2 = img2.pixels;
            for (int i = 0; i < timg1.Length; i++)
            {
                if (timg1[i] != timg2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
