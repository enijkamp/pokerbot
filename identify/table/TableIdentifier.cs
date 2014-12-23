using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace PokerBot
{	
	public class TableIdentifier
	{
        // # public
        public class OpenSeat
        {
            public static OpenSeat NOT_FOUND = null;

            public int TotalSeatsOpen;
            public int Num;
            public Rectangle Offset;

            public OpenSeat(int num, Rectangle offset, int totalSeatsOpen)
            {
                this.Num = num;
                this.TotalSeatsOpen = totalSeatsOpen;
                this.Offset = offset;
            }
        }

        // # pipelines
		private class PocketPipeline
		{
			private ColorReducer reducer = new ColorReducers.Card();
			private PocketIdentifier identifier = new PocketIdentifier(CardReader.readCardsFromResources());
			
			public List<Card> process(Image image, Rectangle rect, TableIdentifier parent)
			{
				// colors
				Image reduced = reducer.reduceColors(image);
				parent.renderImage(reduced, rect);
				
				// hand
				List<Card> cards = identifier.identifyCards(reduced);
				return cards;
			}
		}

        private class CardPipeline
        {
            private ColorReducer reducer = new ColorReducers.Card();
            private CardIdentifier identifier;

            public CardPipeline(TableLayout layout)
            {
                // patterns
                List<CardPattern> patterns = CardReader.readCardsFromResources();
                CardStrategy strategy = new CardStrategyFast(patterns, layout);
                this.identifier = new CardIdentifier(strategy.identifyRegions,
                                                     strategy.identifyCards);
            }

            public List<Card> process(Image image, TableIdentifier parent)
            {
                List<Card> cards = identifier.identifyCards(image);
                Log.Fine("found " + cards.Count + " cards");
                return cards;
            }

            private bool nearlySameColor(Image image)
            {
                // count
                Dictionary<int, int> colors = new Dictionary<int, int>();
                foreach (int pixel in image.pixels)
                {
                    if (!colors.ContainsKey(pixel))
                        colors.Add(pixel, 1);
                    else
                        colors[pixel]++;
                }
                // check
                foreach (int count in colors.Values)
                {
                    float percentage = (float)count / image.pixels.Length;
                    if (percentage >= .95f)
                        return true;
                }
                return false;
            }
        }
	
		
		private class CardPipelineSlow
		{
			private ColorReducer reducer = new ColorReducers.Card();
			private CardIdentifier identifier;

            public CardPipelineSlow()
			{
				// patterns
				List<CardPattern> patterns = CardReader.readCardsFromResources();
				CardStrategy strategy = new CardStrategySlow(patterns);
				this.identifier = new CardIdentifier(strategy.identifyRegions, 
			                                         strategy.identifyCards);
			}
			
			public List<Card> process(Image image, Rectangle rect, TableIdentifier parent)
			{
				// colors
				Image reduced = reducer.reduceColors(image);
				parent.renderImage(reduced, rect);
				
				// empty?
				if(!nearlySameColor(reduced))
				{
					List<Card> cards = identifier.identifyCards(reduced);
					Log.Fine("found " + cards.Count + " cards");
					return cards;
				}
				else
				{
                    Log.Fine("no cards");
					return new List<Card>();
				}
			}
			
			private bool nearlySameColor(Image image)
			{
				// count
				Dictionary<int, int> colors = new Dictionary<int, int>();
				foreach(int pixel in image.pixels)
				{
					if(!colors.ContainsKey(pixel))
						colors.Add(pixel, 1);
					else
						colors[pixel]++;
				}			
				// check
				foreach(int count in colors.Values)
				{
					float percentage = (float) count / image.pixels.Length;
					if(percentage >= .95f)
						return true;
				}
				return false;			
			}
		}
		
		private class TextBoxPipeline
		{
			private ColorReducer reducer = new ColorReducers.TextBox();
			private ColorInverter inverter = new ColorInverter(Color.White, Color.Black);
			private ColorReplacer replacer = new ColorReplacer(Color.White, Color.Transparent);
			private CharIdentifier identifier = new CharIdentifier(CharReader.readCharsFromResourcesPlayer());
			private ColorReplacer inker = new ColorReplacer(Color.Transparent, Color.Cyan);
			
			public List<String> process(Image image, int id, Rectangle rect, TableIdentifier parent, bool isMySeat)
			{
				// colors
				Image reduced = reducer.reduceColors(image);
				Image inverted = inverter.invert(reduced);
				Image replaced = replacer.replace(inverted);

                // text
                List<String> textLines = identifyLines(replaced, rect, parent);

                // if highlighted
                if (isMySeat && textLines.Count == 0)
                {
                    // smaller rect
                    Image smallerImage = reduced.crop(15, reduced.width - 15, 1, reduced.height-1);

                    // not inverted
                    Image notInverted = replacer.replace(smallerImage);
                    textLines = identifyLines(notInverted, rect, parent);
                }

                return textLines;
			}

            private List<String> identifyLines(Image replaced, Rectangle rect, TableIdentifier parent)
            {
                // image
                parent.renderImage(inker.replace(replaced), rect);

                // partition
                List<List<Image>> lines = HorizontalPartitioner.partition(replaced);

                // chars
                List<String> textLines = new List<String>();
                foreach (List<Image> line in lines)
                {
                    String textLine = "";
                    foreach (Image chars in line)
                    {
                        List<Image> combos = CharDecomposer.decompose(chars);
                        foreach (Image chr in combos)
                        {
                            Image character = ImageCropper.crop(chr);
                            textLine += identifyChars(character);
                        }
                    }
                    textLines.Add(textLine);
                }

                return textLines;
            }
			
			private String identifyChars(Image image)
			{
				try
				{
					return identifier.identifyChars(image);
				}
				catch(UnknownCharException ex)
				{
					if(SAVE_UNKNOWN_IMAGES)
						saveBitmap("player", (int) DateTime.Now.Ticks, ImageTools.toBitmap(ex.image));
					return "?";
				}
			}
			
			private void saveBitmap(string name, int id, System.Drawing.Image image)
			{
				image.Save(name + "_" + id + ".png", ImageFormat.Png);
			}
		}

        private class PotPipeline
        {
            private ColorReducer reducer = new ColorReducers.Pot();
            private ColorInverter inverter = new ColorInverter(Color.White, Color.Black);
            private ColorReplacer replacer = new ColorReplacer(Color.White, Color.Transparent);
            private CharIdentifier identifier = new CharIdentifier(CharReader.readCharsFromResourcesPot());
            private ColorReplacer inker = new ColorReplacer(Color.Transparent, Color.DeepPink);

            public double process(Image image, Rectangle rect, TableIdentifier parent)
            {
                // colors
                Image cropped = crop(image, rect);
                Image reduced = reducer.reduceColors(cropped);
                Image replaced = replacer.replace(reduced);

                // image
                parent.renderImage(inker.replace(replaced), rect);

                // partition
                List<List<Image>> lines = HorizontalPartitioner.partition(replaced);
                if (lines.Count == 0)
                {
                    return Table.NO_POT;
                }

                // chars
                String textLine = "";
                foreach (Image chars in lines[0])
                {
                    List<Image> combos = CharDecomposer.decompose(chars);
                    foreach (Image chr in combos)
                    {
                        Image character = ImageCropper.crop(chr);
                        textLine += identifyChars(character);
                    }
                }

                // check for digit
                if (!TextTools.ContainsDigit(textLine))
                {
                    return Table.NO_POT;
                }

                // format
                textLine = textLine.Replace("$", "").Replace("?", "").Trim();

                // money
                return TextTools.ParseDouble(textLine);
            }

            private String identifyChars(Image image)
            {
                try
                {
                    return identifier.identifyChars(image);
                }
                catch (UnknownCharException ex)
                {
                    if (SAVE_UNKNOWN_IMAGES)
                        saveBitmap("pot", (int)DateTime.Now.Ticks, ImageTools.toBitmap(ex.image));
                    return "?";
                }
            }

            private void saveBitmap(string name, int id, System.Drawing.Image image)
            {
                image.Save(name + "_" + id + ".png", ImageFormat.Png);
            }
        }
        
		private class BetPipeline
		{		
			private ColorReducer reducer = new ColorReducers.Bets();
			private ColorInverter inverter = new ColorInverter(Color.White, Color.Black);
			private ColorReplacer replacer = new ColorReplacer(Color.White, Color.Transparent);
			private CharIdentifier identifier = new CharIdentifier(CharReader.readCharsFromResourcesBets());
			private ColorReplacer inker = new ColorReplacer(Color.Transparent, Color.DeepPink);            
            
			public double process(Image image, Rectangle rect, TableIdentifier parent)
			{
				// colors
				Image reduced = reducer.reduceColors(image);
				Image inverted = inverter.invert(reduced);
				Image replaced = replacer.replace(inverted);
				
				// image
				parent.renderImage(inker.replace(replaced), rect);		
				
				// partition
				List<List<Image>> lines = HorizontalPartitioner.partition(replaced);
                if(lines.Count == 0)
                {
                    return Player.NO_BET;
                }
				
				// chars
				String textLine = "";
				foreach(Image chars in lines[0]) 
				{
					List<Image> combos = CharDecomposer.decompose(chars);
					foreach(Image chr in combos)
					{
						Image character = ImageCropper.crop(chr);
						textLine += identifyChars(character);
					}						
				}
                
                // check for digit
                if (!TextTools.ContainsDigit(textLine))
                {
                    return Player.NO_BET;
                }

                // sanity check
                if (!textLine.Contains("$"))
                {
                    throw new ArgumentException("bet text has no dollar sign");
                }

                // replace all non-numeric chars
                textLine = textLine.Trim();
                string numericTextLine = textLine;
                foreach (char chr in textLine)
                {
                    if (!TextTools.IsNumeric(chr) && !TextTools.IsPoint(chr))
                    {
                        numericTextLine = numericTextLine.Replace(chr.ToString(), "");
                    }
                }


                // sanity check (sometimes some pixels are identifier as '.')
                if (numericTextLine.StartsWith("."))
                {
                    numericTextLine = numericTextLine.Substring(1);
                }
                
                // money
                return TextTools.ParseDouble(numericTextLine);
			}
			
			private String identifyChars(Image image)
			{
				try
				{
					return identifier.identifyChars(image);
				}
				catch(UnknownCharException ex)
				{
					if(SAVE_UNKNOWN_IMAGES)
                        saveBitmap("bet", (int)DateTime.Now.Ticks, ImageTools.toBitmap(ex.image));
					return "?";
				}
			}
			
			private void saveBitmap(string name, int id, System.Drawing.Image image)
			{
				image.Save(name + "_" + id + ".png", ImageFormat.Png);
			}
		}

        private class ControlsCharPipeline
        {
            private ColorReducer reducer = new ColorReducers.ControlsChars();
            private ColorInverter inverter = new ColorInverter(Color.WhiteSmoke, Color.Black);
            private ColorReplacer replacer = new ColorReplacer(Color.WhiteSmoke, Color.Transparent);
            private CharIdentifier identifier = new CharIdentifier(CharReader.readCharsFromResourcesControls());
            private ColorReplacer inker = new ColorReplacer(Color.Transparent, Color.Yellow);

            public TableControl process(Image image, int position, Rectangle rect, TableIdentifier parent)
            {
                // crop
                image = crop(image, rect);

                // colors
                Image reduced = reducer.reduceColors(image);
                Image inverted = inverter.invert(reduced);
                Image replaced = replacer.replace(inverted);

                // image
                parent.renderImage(inker.replace(replaced), rect);

                // partition
                List<List<Image>> lines = HorizontalPartitioner.partition(replaced);

                // ## action ##                
                TableControl.ControlType type = TableControl.ControlType.NONE;
                if (lines.Count >= 1)
                {
                    // read chars
                    String actionText = "";
                    foreach (Image chars in lines[0])
                    {
                        List<Image> combos = CharDecomposer.decompose(chars, 0);
                        foreach (Image chr in combos)
                        {
                            Image character = ImageCropper.crop(chr);
                            actionText += identifyChars(character);
                        }
                    }
                    if (actionText == "Fold")
                    {
                        type = TableControl.ControlType.FOLD;
                    }
                    else if (actionText == "Check")
                    {
                        type = TableControl.ControlType.CHECK;
                    }
                    else if (actionText.StartsWith("Cal"))
                    {
                        type = TableControl.ControlType.CALL;
                    }
                    else if (actionText == "Bet")
                    {
                        type = TableControl.ControlType.BET;
                    }
                    else if (actionText.StartsWith("Raise"))
                    {
                        type = TableControl.ControlType.RAISE;
                    }
                    else if (actionText.StartsWith("Post"))
                    {
                        type = TableControl.ControlType.POST_BLIND;
                    }
                }

                // ## amount ##
                double amount = 0.0;
                if (lines.Count >= 2)
                {
                    // read chars
                    String amountText = "";
                    foreach (Image chars in lines[1])
                    {
                        List<Image> combos = CharDecomposer.decompose(chars, 0);
                        foreach (Image chr in combos)
                        {
                            Image character = ImageCropper.crop(chr);
                            amountText += identifyChars(character);
                        }
                    }

                    // format
                    amountText = amountText.Replace("$", "").Replace("?", "").Trim();

                    // money
                    amount = TextTools.ParseDouble(amountText);
                }


                return new TableControl(type, position, amount);
            }

            private String identifyChars(Image image)
            {
                try
                {
                    return identifier.identifyChars(image);
                }
                catch (UnknownCharException ex)
                {
                    if (SAVE_UNKNOWN_IMAGES)
                        saveBitmap("controls", (int)DateTime.Now.Ticks, ImageTools.toBitmap(ex.image));
                    return "?";
                }
            }

            private void saveBitmap(string name, int id, System.Drawing.Image image)
            {
                image.Save(name + "_" + id + ".png", ImageFormat.Png);
            }
        }
		
		private class ButtonPipeline
		{
			private ColorReducer reducer = new ColorReducers.Button();
			
			public int process(Image image, Rectangle[] rects, TableIdentifier parent)
			{
                // crop
                List<Image> images = new List<Image>();
                foreach (Rectangle rect in rects)
                {
                    Image cropped = crop(image, rect);
                    Image reduced = reducer.reduceColors(cropped);
                    images.Add(reduced);
                    parent.renderImage(reduced, rect);
                }

                // positions
                for (int i = 0; i < images.Count; i++)
                {
                    if (ButtonIdentifier.isReducedButton(images[i]))
                    {
                        return i;
                    }
                }
                throw new Exception("Cannot identify button");			
			}
		}
        
		private class SmallCardsPipeline
		{
			private ColorReducer reducer = new ColorReducers.SmallCards();
			
			public bool process(Image image, Rectangle rect, TableIdentifier parent)
			{
				// colors
				Image reduced = reducer.reduceColors(image);

				// render
				parent.renderImage(reduced, rect);
				
				// positions
				return SmallCardsIdentifier.areReducedSmallCards(reduced);
            }
		}

        private class ControlsPipeline
        {
            private ColorReducer reducer = new ColorReducers.Controls();
            private Color[] realButtonColor = null;

            public bool process(Image image, Rectangle rect, TableIdentifier parent)
            {
                // crop
                Image cropped = crop(image, rect);

                // real color unknown
                if (realButtonColor == null)
                {
                    // colors
                    Image reduced = reducer.reduceColors(cropped);
                    parent.renderImage(reduced, rect);

                    // check
                    bool visible = ControlsIdentifier.areReducedControlsVisible(reduced);
                    if (visible) realButtonColor = ImageTools.getSignificantColorRange(cropped);
                    return visible;
                }
                // real color known
                else
                {
                    // check
                    return ControlsIdentifier.areControlsVisibleWithNativeColor(cropped, realButtonColor);
                }
            }
        }

        private class OpenSeatPipeline
        {
            private ColorReducer reducer = new ColorReducers.SeatOpen();
            private Image pattern;

            public OpenSeatPipeline()
            {
                Log.Debug("reading seat pattern");
                Stream stream = AssemblyTools.getAssemblyStream("open_seat.png");
                Bitmap bitmap = Bitmap.FromStream(stream) as Bitmap;
                pattern = ImageTools.toImage(bitmap);
                pattern = reducer.reduceColors(pattern);
            }

            public OpenSeat process(Image tableImage, TableLayout layout, TableIdentifier parent)
            {
                // seats
                DateTime seatsStart = DateTime.Now;
                int num = 0, totalOpen = 0; ;
                int MAX_Y_SCAN = 5;
                int openSeatNum = -1;
                Rectangle openSeatRect = Rectangle.Empty;
                foreach (Rectangle seat in layout.Seats)
                {
                    bool isOpen = IsOpen(reducer, seat, pattern, tableImage, MAX_Y_SCAN);
                    if (isOpen)
                    {
                        if (openSeatNum == -1)
                        {
                            openSeatNum = num;
                            openSeatRect = seat;
                        }
                        totalOpen++;
                    }
                    num++;
                }
                Log.Debug("## seat scan -> " + DateTime.Now.Subtract(seatsStart).TotalMilliseconds + " ms ##");

                // positions
                if (openSeatNum == -1)
                {
                    return OpenSeat.NOT_FOUND;
                }
                else
                {
                    return new OpenSeat(openSeatNum, openSeatRect, totalOpen);
                }
            }

            private bool IsOpen(ColorReducer reducer, Rectangle seat, Image pattern, Image tableImage, int maxScanY)
            {
                Image seatCropped = tableImage.crop(seat.X, seat.X + seat.Width, seat.Y, seat.Y + seat.Height);
                Image seatReduced = reducer.reduceColors(seatCropped);
                for (int y = 0; y < maxScanY; y++)
                {
                    Log.FineIf(y % 100 == 0, y + "/" + seatReduced.height);
                    for (int x = 0; x < seatReduced.width - pattern.width; x++)
                    {
                        Image sub = seatReduced.crop(x, x + pattern.width, y, y + pattern.height);
                        if (ImageTools.match(sub, pattern))
                        {
                            Log.Info("found open seat pattern x=" + x + ", y=" + y);
                            return true;
                        }
                    }
                }
                return false;
            }
        }
		
		// const
		private const bool SAVE_UNKNOWN_IMAGES = false;
        private const bool SAVE_EXCEPTION_REPORTS = false;

        // enums
        public enum PlayerInfoEnum { NAME, MONEY, BOTH };
		
		// pipelines
        private CardPipeline cardPipe;
		private TextBoxPipeline textPipe = new TextBoxPipeline();
		private ButtonPipeline buttonPipe = new ButtonPipeline();
		private PocketPipeline pocketPipe = new PocketPipeline();
        private BetPipeline betPipe = new BetPipeline();
        private PotPipeline potPipe = new PotPipeline();
        private SmallCardsPipeline smallCardsPipe = new SmallCardsPipeline();
        private ControlsPipeline controlsPipe = new ControlsPipeline();
        private ControlsCharPipeline controlsCharPipe = new ControlsCharPipeline();
        private OpenSeatPipeline openSeatPipe = new OpenSeatPipeline();
		
		// rendering
		public delegate void RenderImage(Image image, Point location);		
		public event RenderImage RenderImageEvent;
        private ImageRenderer renderer;
		
		// 9/10 players rects
        private TableLayout layout;

        // default bet
        private const double DEFAULT_BET = 1;
		
		public TableIdentifier(TableLayout layout)
		{
            this.cardPipe = new CardPipeline(layout);
            this.layout = layout;
		}

        public TableLayout Layout
        {
            get { return layout; }
        }

        public ImageRenderer Renderer
        {
            get { return renderer; }
            set { renderer = value; }
        }

        public OpenSeat identifyOpenSeat(Image tableImage)
        {
            return openSeatPipe.process(tableImage, layout, this);
        }

        public bool identifyMove(Image tableImage)
        {
            Log.Debug("# checking move");

            return controlsPipe.process(tableImage, layout.Controls, this);
        }

        public Table identifyTableForLocation(Image tableImage)
        {
            Table table = new Table();

            // players
            Log.Debug("# scanning player infos");
            List<Player> players = new List<Player>();
            for (int i = 0; i < layout.Names.Length; i++)
            {
                // new player
                Player player = new Player();
                players.Add(player);

                // name & money
                processTextBox(textPipe, player, tableImage, i, PlayerInfoEnum.NAME, false);
                Log.Fine("name " + (i + 1) + " = " + (player.Name.Length == 0 ? "no name" : "'" + player.Name + "'"));

            }
            table.Players = players;

            return table;
        }

        public Table identifyTable(Image tableImage, PlayerInfoEnum playerInfo)
        {
            return identifyTable(tableImage, playerInfo, DEFAULT_BET, Table.NO_SEAT);
        }

        public Table identifyTable(Image tableImage, PlayerInfoEnum playerInfo, double defaultBet, int seat)
		{
			Table table = new Table();
			
			// community
            Log.Debug("# scanning cards");
            table.Community = cardPipe.process(tableImage, this);
			
			// dealer
            Log.Debug("# scanning dealer");
            table.Dealer = buttonPipe.process(tableImage, layout.Buttons, this);
			Log.Debug("dealer position " + table.Dealer);
			
            // pot
            Log.Debug("# scanning pot");
            table.Pot = potPipe.process(tableImage, layout.Pot, this);
            Log.Debug("pot size " + table.Pot);

            // seat
            table.Seat = seat;

			// players
            Log.Debug("# scanning player infos");
			List<Player> players = new List<Player>();
            for (int i = 0; i < layout.Names.Length; i++)
			{
                // new player
				Player player = new Player();
                players.Add(player);
                
                // name & money
                bool isMySeat = (i == seat);
                processTextBox(textPipe, player, tableImage, i, playerInfo, isMySeat);
                Log.Fine("name " + (i + 1) + " = " + (player.Name.Length == 0 ? "no name" : "'" + player.Name + "'"));
                
                // bet
                Image betImage = crop(tableImage, layout.Bets[i]);
                double bet = processBet(tableImage, betImage, layout.Bets[i], this, defaultBet);
				player.Bet = bet;
                Log.Fine("bet " + (i + 1) + " = " + (bet == -1 ? "no bet" : bet.ToString()));
                
                // small cards
                Image smallCardsImage = crop(tableImage, layout.SmallCards[i]);
                bool hasCards = smallCardsPipe.process(smallCardsImage, layout.SmallCards[i], this);
                processPlayerState(player, hasCards);
                Log.Fine("has cards " + (i + 1) + " = " + hasCards);                
			}
			table.Players = players;
			
			return table;
		}

        private double processBet(Image table, Image bet, Rectangle rect, TableIdentifier parent, double defaultBet)
        {
            try
            {
                return betPipe.process(bet, rect, parent);
            }
            catch (Exception ex)
            {
                ErrorHandler.BeepError();
                if (SAVE_EXCEPTION_REPORTS)
                {
                    ErrorHandler.ReportBetException(ex, table, bet, "Cannot identify bet");
                }
                else
                {
                    Log.Error("Cannot identify bet");
                }
                return defaultBet;
            }
        }
		
		public Table identifyTable(Image tableImage, int seat)
		{
            // table
			Table table = identifyTable(tableImage, PlayerInfoEnum.MONEY, DEFAULT_BET, seat);
			
			// pocket
            Log.Debug("# scanning pocket");
			Image handImage = crop(tableImage, layout.Hands[seat]);
			table.Hand = pocketPipe.process(handImage, layout.Hands[seat], this);
			Log.Debug("hand cards " + table.Hand.Count);
			
			return table;
		}

        public List<TableControl> identifyControls(Image tableImage)
        {
            int LEFT = 0;
            int RIGHT = 2;

            Log.Debug("# scanning controls");
            List<TableControl> controls = new List<TableControl>();

            // controls
            for(int i = LEFT; i <= RIGHT; i++)
            {
                TableControl control = controlsCharPipe.process(tableImage, i, layout.ControlRects[i], this);
                if (control.Type != TableControl.ControlType.NONE)
                {
                    controls.Add(control);
                }
            }

            return controls;
        }

        private void processPlayerState(Player player, bool hasCards)
        {
            if(hasCards) 
            {
                player.State = Player.States.HAS_CARDS;
            }
            else if(player.State == Player.States.EXISTENT)
            {
                player.State = Player.States.FOLDED_CARDS;
            }
        }

        private void processTextBox(TextBoxPipeline pipe, Player info, Image table, int id, PlayerInfoEnum read, bool isMySeat)
		{
            try
            {
                if (read == PlayerInfoEnum.BOTH || read == PlayerInfoEnum.NAME)
                {
                    // textbox
                    Rectangle rect = layout.Names[id];
                    info.Position = id;
                    info.NameRect = rect;
                    Image image = crop(table, rect);
                    List<String> textLines = pipe.process(image, id, rect, this, isMySeat);
                    if (textLines.Count == 0)
                    {
                        info.State = Player.States.NON_EXISTENT;
                    }
                    else
                    {
                        if (!containsChars(textLines[0]))
                        {
                            info.State = Player.States.NON_EXISTENT;
                        }
                        else
                        {
                            info.State = Player.States.EXISTENT;
                            info.Name = textLines[0];
                        }
                    }
                }

                if (read == PlayerInfoEnum.BOTH || read == PlayerInfoEnum.MONEY)
                {
                    // textbox
                    Rectangle rect = layout.Money[id];
                    info.Position = id;
                    info.MoneyRect = rect;
                    Image image = crop(table, rect);
                    List<String> textLines = pipe.process(image, id, rect, this, isMySeat);

                    if (textLines.Count == 0)
                    {
                        info.State = Player.States.NON_EXISTENT;
                    }
                    else
                    {
                        if (!containsChars(textLines[0]))
                        {
                            info.State = Player.States.NON_EXISTENT;
                        }
                        else
                        {
                            info.State = Player.States.EXISTENT;
                            if (textLines[0].Contains("$"))
                            {
                                info.State = Player.States.EXISTENT;
                                info.Money = TextTools.ParseDouble(textLines[0].Replace("$", "").Trim());
                            }
                            else
                            {
                                info.Money = Player.NO_MONEY;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Log.Warn("Unable to read player info (mode = "+read+")");
            }
		}
        
        private static bool containsChars(string chars)
        {
            foreach(char chr in chars.ToUpper().ToCharArray())
            {
                if((chr >= 'A' && chr <= 'Z') || (chr >= '0' && chr <= '9'))
                    return true;
            }
            return false;
        }
		
		private static Image crop(Image image, Rectangle rect)
		{
			int xStart = rect.Location.X;
			int xEnd = xStart + rect.Width;
			int yStart = rect.Location.Y;
			int yEnd = yStart + rect.Height;
			return image.crop(xStart, xEnd, yStart, yEnd);
		}
		
		private void renderImage(Image image, Point point)
		{
            if(RenderImageEvent != null)
			    RenderImageEvent(image, point);
            if (Renderer != null)
                Renderer.renderImage(image, point);
		}
		
		private void renderImage(Image image, Rectangle point)
		{
            if(RenderImageEvent != null)
			    RenderImageEvent(image, point.Location);
            if (Renderer != null)
                Renderer.renderImage(image, point.Location);
		}

	}
}
