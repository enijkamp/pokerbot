using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;

namespace PokerBot
{
    public class RandomClicker
    {
        private const int WIDTH = 100;
        private const int HEIGHT = 100;

        private Point point;
        private Mouse mouse;
        private Random random = new Random();

        public RandomClicker(Point point, Mouse mouse)
        {
            this.point = point;
            this.mouse = mouse;
        }
        
        private int randomInt(int begin, int end)
        {
            return (int)(random.NextDouble() * Math.Abs(end - begin)) + begin;
        }

        public void click()
        {
            if (random.NextDouble() > 0.5)
            {
                mouse.MoveAndLeftClick(point.X - (WIDTH / 2), point.Y + (HEIGHT / 2), 50, 50);
            }
            if (random.NextDouble() > 0.3)
            {
                Thread.Sleep(randomInt(10, 100));
                mouse.MoveAndLeftClick(point.X - (WIDTH / 2), point.Y + (HEIGHT / 2), 50, 50);
            }
        }

        public void click(TableLayout table)
        {
            mouse.MoveAndLeftClick(table.Offset.X + table.Cards[2].X + 100,
                table.Offset.Y + table.Cards[2].Y + 100, 140, 80);
        }
    }
}
