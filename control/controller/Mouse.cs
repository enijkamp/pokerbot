using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerBot
{
    public interface Mouse
    {
        void Move(int x, int y);

        void LeftClick();

        void RightClick();

        void DoubleLeftClick();

        void MoveAndLeftClick(int x, int y, int randomX, int randomY);

        Point Position { get; }
    }
}