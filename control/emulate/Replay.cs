using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerBot
{
    public interface Replay
    {
        int getSpinWait();

        List<Point> getReplay();
    }
}
