using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerBot
{
    public class LobbyLayout
    {
        private static Rectangle TABLE_LIST = new Rectangle(10, 200, 740, 332);
        private static int PLAYERS_COUNTS_X = 380, PLAYERS_COUNTS_W = 25;
        private static int AVG_POT_SIZE_X = 494, AVG_POT_SIZE_W = 45;
        private static int FLOPS_PLAYER_X = 573, FLOPS_PLAYER_W = 40;

        private static Point LAST_ROW = new Point(PLAYERS_COUNTS_X + PLAYERS_COUNTS_W + 15, 435);

        private static Point MOUSE = new Point(PLAYERS_COUNTS_X + PLAYERS_COUNTS_W + 15, 380);

        private static Point TASKBAR = new Point(104, 613);

        private static Point ICON = new Point(37, 94);        

        private static int CELL_HEIGHT = 6;

        public Rectangle TableList
        {
            get { return TABLE_LIST; }
        }

        public int PlayersCountX
        {
            get { return PLAYERS_COUNTS_X; }
        }

        public int PlayersCountW
        {
            get { return PLAYERS_COUNTS_W; }
        }

        public int PotX
        {
            get { return AVG_POT_SIZE_X; }
        }

        public int PotW
        {
            get { return AVG_POT_SIZE_W; }
        }

        public int FlopsX
        {
            get { return FLOPS_PLAYER_X; }
        }

        public int FlopsW
        {
            get { return FLOPS_PLAYER_W; }
        }

        public int CellHeight
        {
            get { return CELL_HEIGHT; }
        }

        public Point Mouse
        {
            get { return MOUSE; }
        }
        
        public Point Icon
        {
            get { return ICON; }
        }

        public Point Taskbar
        {
            get { return TASKBAR; }
        }

        public Point LastRow
        {
            get { return LAST_ROW; }
        }
    }
}
