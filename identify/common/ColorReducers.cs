using System;
using System.Drawing;

namespace PokerBot
{
	public class ColorReducers
	{
		public class Card : ColorPaletteReducer
		{					
			private static Color[] colors =
			{
				Color.Black,
				Color.Red,
				Color.White
			};
			
			public Card() : base(colors) {}
		}

        public class Controls : ColorPaletteReducer
        {
            private static Color[] colors =
			{
				Color.Aqua,
				Color.Black,
				Color.Blue,
				Color.Brown,
				Color.Gray,
				Color.Green,
				Color.Orange,
				Color.Red,
				Color.White,
				Color.Yellow		
			};

            public Controls() : base(colors) { }
        }

        public class ControlsChars : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.WhiteSmoke,
				Color.Black			
			};

            public ControlsChars() : base(colors) { }
        }

        public class LobbyChars : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.White,
				Color.Black			
			};

            public LobbyChars() : base(colors) { }
        }

        public class LobbyCharsJoined : ColorPaletteReducer
        {
            public static Color JOINED_COLOR = Color.Red;

            private static Color[] colors =
			{
                Color.Green,
                Color.White,
				JOINED_COLOR		
			};

            public LobbyCharsJoined() : base(colors) { }
        }

        public class SeatOpen : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.Yellow,
                Color.Black		
			};

            public SeatOpen() : base(colors) { }
        }

        public class Error : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.Yellow,
                Color.Black,
                Color.White
			};

            public Error() : base(colors) { }
        }

        public class AutoBlind : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.Brown,
                Color.Black,
                Color.White
			};

            public AutoBlind() : base(colors) { }
        }


        public class SitOut : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.Brown,
                Color.White
			};

            public SitOut() : base(colors) { }
        }

        public class LobbyLogin : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.Red,
                Color.White
			};

            public LobbyLogin() : base(colors) { }
        }

        public class BuyIn : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.Black,
                Color.White,
                Color.Red
			};

            public BuyIn() : base(colors) { }
        }

        public class TaskBar : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.Black,
                Color.Red,
                Color.White
			};

            public TaskBar() : base(colors) { }
        }

        public class TableBottomCorner : ColorPaletteReducer
        {
            private static Color[] colors =
			{
				Color.Aqua,
				Color.Black,
				Color.Blue,
				Color.Brown,
                Color.LightYellow,
				Color.Gray,
				Color.Green,
				Color.Orange,
				Color.Red,
				Color.White,
				Color.Yellow
			};

            public TableBottomCorner() : base(colors) { }
        }

        public class LobbyLoginOk : BlackWhiteReducer { }
        public class Connecting : BlackWhiteReducer { }
        public class CheckOrFold : BlackWhiteReducer { }
        public class ImBack : BlackWhiteReducer { }
        public class PostBlind : BlackWhiteReducer { }
        public class News : BlackWhiteReducer { }
        public class Bets : BlackWhiteReducer { }
        public class Pot : BlackWhiteReducer { }
        public class Table : BlackWhiteReducer { }
        public class Lobby : BlackWhiteReducer { }
        public class LobbyWindow : BlackWhiteReducer { }
        public class TextBox : BlackWhiteReducer { }
        public class Button : BlackWhiteReducer { }
        public class SmallCards : BlackWhiteReducer { }

        public class BlackWhiteReducer : ColorPaletteReducer
        {
            private static Color[] colors =
			{
                Color.Black,
                Color.White
			};

            public BlackWhiteReducer() : base(colors) { }
        }
	}
}
