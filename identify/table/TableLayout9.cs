using System;
using System.Drawing;

namespace PokerBot
{	
	public class TableLayout9 : TableLayout
	{
        private static Rectangle[] hands = 
		{
			new Rectangle(496, 12, 40, 40),
            new Rectangle(635, 69, 40, 40), 
            new Rectangle(708, 200 , 40, 40), 
            new Rectangle(575, 315, 40, 40),
            new Rectangle(367, 337, 40, 40), 
            new Rectangle(165, 315, 40, 40),
            new Rectangle(30, 197, 40, 40), 
            new Rectangle(92, 71, 40, 40),
            new Rectangle(239, 13, 40, 40) 
		};

        private static Rectangle[] names = 
		{
			new Rectangle(560, 34, 89, 17),
			new Rectangle(699, 90, 89, 16),
			new Rectangle(693, 275, 87, 16),			
			new Rectangle(485, 338, 89, 16),			
			new Rectangle(352, 414, 89, 17),			
			new Rectangle(226, 338, 89, 16),			
			new Rectangle(14, 275, 89, 17),			
			new Rectangle(9, 90, 89, 16),			
			new Rectangle(152, 34, 89, 17)
		};

        private static Rectangle[] money = 
		{
			new Rectangle(560, 49, 89, 16),
			new Rectangle(699, 105, 89, 15),
			new Rectangle(693, 290, 87, 15),			
			new Rectangle(485, 353, 89, 15),			
			new Rectangle(352, 429, 89, 16),			
			new Rectangle(226, 353, 89, 15),			
			new Rectangle(14, 290, 89, 17),			
			new Rectangle(9, 105, 89, 15),			
			new Rectangle(152, 49, 89, 15)
		};

        private static Rectangle[] buttons = 
		{
			new Rectangle(509, 85, 28, 28),
			new Rectangle(650, 143, 28, 28),
			new Rectangle(669, 237, 28, 28),
			new Rectangle(547, 312, 28, 28),
			new Rectangle(380, 311, 28, 28),			
			new Rectangle(231, 312, 28, 28),			
			new Rectangle(107, 240, 28, 28),			
			new Rectangle(126, 140, 28, 28),
			new Rectangle(270, 85, 28, 28)
		};

        private static Rectangle[] bets = 
		{
            new Rectangle(415, 132, 125, 15),
            new Rectangle(540, 168, 90, 15),
            new Rectangle(550, 227, 120, 15),
            new Rectangle(465, 295, 75, 15),         
            new Rectangle(374, 292, 85, 15),
            new Rectangle(245, 295, 120, 15),
            new Rectangle(159, 226, 120, 15),
            new Rectangle(182, 161, 85, 15),
            new Rectangle(279, 131, 120, 15)
        };

        private static Rectangle[] smallcards = 
		{
            new Rectangle(533, 85, 30, 35),
            new Rectangle(596, 113, 30, 35),
            new Rectangle(670, 204, 30, 35),
            new Rectangle(585, 278, 30, 35),         
            new Rectangle(417, 311, 30, 35),
            new Rectangle(172, 279, 30, 35),
            new Rectangle(101, 205, 30, 35),
            new Rectangle(175, 112, 30, 35),
            new Rectangle(237,89, 30, 35)
        };

        private static Rectangle controls = new Rectangle(420, 490, 100, 40);

        private static Size size = new Size(795, 545);

        private static Point[] cards = 
        {
            new Point(268, 151),
            new Point(322, 151),
            new Point(376, 151),
            new Point(430, 151),
            new Point(484, 151)
        };

        private static Rectangle potRect = new Rectangle(396, 16, 35, 15);

        private static Point[] controlPoints = 
        { 
            new Point(470, 514), 
            new Point(600, 516), 
            new Point(725, 517) 
        };

        private static Rectangle[] controlRects = 
        { 
            new Rectangle(440, 500, 60, 25), 
            new Rectangle(556, 493, 88, 40), 
            new Rectangle(680, 493, 88, 40)
        };

        private static Rectangle[] safeMouse =
        {
            new Rectangle(460, 400, 130, 70),
            new Rectangle(680, 400, 100, 40),
            new Rectangle(300, 460, 90, 70)
        };

        private static int seatWidth = 50;
        private static int seatHeight = 20;

        private static Rectangle[] seats =
        {
            new Rectangle(497, 28,  seatWidth, seatHeight),
            new Rectangle(636, 89,  seatWidth, seatHeight),
            new Rectangle(715, 217, seatWidth, seatHeight),
            new Rectangle(593, 333, seatWidth, seatHeight),
            new Rectangle(373, 359, seatWidth, seatHeight),
            new Rectangle(161, 331, seatWidth, seatHeight),
            new Rectangle(35,  217, seatWidth, seatHeight),
            new Rectangle(117, 88,  seatWidth, seatHeight),
            new Rectangle(260, 28,  seatWidth, seatHeight)
        };

        private Rectangle sitOutPattern = new Rectangle(0, 362, 115, 20);
        private Rectangle autoBlindPattern = new Rectangle(0, 374, 115, 20);
        private Rectangle waitForBlindPattern = new Rectangle(0, 374, 115, 20);

        private Rectangle sitOutClick = new Rectangle(60, 370, 40, 3);
        private Rectangle autoBlindClick = new Rectangle(60, 384, 40, 3);
        private Rectangle waitForBlindClick = new Rectangle(60, 384, 40, 3);


        private Rectangle msgBox = new Rectangle(300, 225, 60, 60);

        private Point close = new Point(780, -17);

        private Point occupiedOk = new Point(395, 296);

        private Point offset = Point.Empty;

        private Rectangle imBack = new Rectangle(550, 450, 100, 35);

        private Rectangle postBlind = new Rectangle(425, 490, 60, 30);

        private Rectangle youHaveBeenRemoved = new Rectangle(186, 226, 52, 60);

        private Point sliderText = new Point(625, 450);
        private Point sliderClick = new Point(765, 450);

        private Rectangle buyInIcon = new Rectangle(230, 70, 57, 100);

        private Rectangle checkOrFold = new Rectangle(325, 230, 120, 30);

        private Rectangle cornerBottom = new Rectangle(775, 530, 25, 25);

        private Rectangle youHaveJustLeftThisTable = new Rectangle(224, 224, 60, 60);

        public TableLayout9()
        {
        }

        public TableLayout9(Point offset)
        {
            this.offset = offset;
        }

        public Rectangle[] Hands { get { return hands; } }
        public Rectangle[] Buttons { get { return buttons; } }
        public Rectangle[] Bets { get { return bets; } }
        public Rectangle[] Names { get { return names; } }
        public Rectangle[] Money { get { return money; } }
        public Rectangle[] SmallCards { get { return smallcards; } }

        public Rectangle Controls { get { return controls; } }

        public Point Offset { get { return offset; } }
        public Size Size { get { return size; } }
        public Rectangle Rect { get { return new Rectangle(Offset, Size); } }


        public Rectangle SitOutPattern { get { return sitOutPattern; } }
        public Rectangle AutoBlindPattern { get { return autoBlindPattern; } }
        public Rectangle WaitForBlindPattern { get { return waitForBlindPattern; } }

        public Rectangle SitOutClick { get { return sitOutClick; } }
        public Rectangle AutoBlindClick { get { return autoBlindClick; } }
        public Rectangle WaitForBlindClick { get { return waitForBlindClick; } }

        public Point[] Cards { get { return cards; } }
        public Rectangle Pot { get { return potRect; } }

        public Point SliderText { get { return sliderText; } }
        public Point SliderClick { get { return sliderClick; } }

        public Point[] ControlPoints { get { return controlPoints; } }
        public Rectangle[] ControlRects { get { return controlRects; } }

        public Rectangle[] SafeMousePositions { get { return safeMouse; } }

        public Rectangle[] Seats { get { return seats; } }

        public Point Close { get { return close; } }

        public Point Occupied { get { return occupiedOk; } }

        public Rectangle MessageBox { get { return msgBox; } }

        public Rectangle YouHaveJustLeftThisTable { get { return youHaveJustLeftThisTable; } }

        public Rectangle ImBack { get { return imBack; } }

        public Rectangle PostBlind { get { return postBlind; } }

        public Rectangle YouHaveBeenRemoved { get { return youHaveBeenRemoved; } }

        public Rectangle BuyInIcon { get { return buyInIcon; } }

        public Rectangle CheckOrFold { get { return checkOrFold; } }

        public Rectangle CornerBottom { get { return cornerBottom; } }
	}
}
