using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerBot
{
    public interface TableLayout
    {
        // table
        Point Offset { get; }
        Size Size { get; }
        Rectangle Rect { get; }

        // check boxes        
        Rectangle SitOutPattern { get; }
        Rectangle AutoBlindPattern { get; }
        Rectangle WaitForBlindPattern { get; }

        Rectangle SitOutClick { get; }
        Rectangle AutoBlindClick { get; }
        Rectangle WaitForBlindClick { get; }
        

        // sliders
        Point SliderText { get; }
        Point SliderClick { get; }

        Point[] ControlPoints { get; }
        Rectangle[] ControlRects { get; }

        Rectangle[] Hands { get; } 
        Rectangle[] Buttons  { get; } 
        Rectangle[] Bets  { get; }
        Rectangle[] Names { get; }
        Rectangle[] Money { get; }
        Rectangle[] SmallCards { get; }

        Rectangle Controls { get; }

        Point[] Cards { get; }
        Rectangle Pot { get; }

        Rectangle[] SafeMousePositions { get; }

        Rectangle[] Seats { get; }

        Point Close { get; }

        Point Occupied { get; }

        Rectangle MessageBox { get; }

        Rectangle YouHaveJustLeftThisTable { get; }

        Rectangle ImBack { get; }

        Rectangle PostBlind { get; }

        Rectangle YouHaveBeenRemoved { get; }

        Rectangle BuyInIcon { get; }

        Rectangle CheckOrFold { get; }

        Rectangle CornerBottom { get; }
    }
}
