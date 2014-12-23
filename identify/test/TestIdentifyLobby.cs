using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PokerBot
{	
	public class TestIdentifyLobby : TestBase
	{
        // const
        private const bool USE_SCREEN = true;
        private const bool SAVE_UNKNOWN = false;

        private class ValueWithY
        {
            public const int NO_VALUE = -1;

            public double Value = NO_VALUE;
            public int Y = NO_VALUE;

            public ValueWithY(double value, int y)
            {
                this.Value = value;
                this.Y = y;
            }
        }


		public static void Main(string[] args)
		{
            Log.SetLevel(Log.Level.FINEST);
            Iterator<Image> screenIter;
            LobbyLayout layout = new LobbyLayout();
            Point offset;

            if (USE_SCREEN)
            {
                // wait
                Log.Info("waiting ...");
                Thread.Sleep(3000);

                // full screen
                Console.WriteLine("## scanning for lobby ##");
                Image fullScreen = new ScreenImageIterator(new Win32Control()).next();
                offset = PatternLocator.locateLobby(fullScreen);
                Console.WriteLine("lobby found at x=" + offset.X + " y=" + offset.Y);

                // lobby
                screenIter = new ScreenImageIterator(new Win32Control(), new Point(offset.X, offset.Y));
                screenIter = new WaitDeltaImageIterator(screenIter);
            }
            else
            {
                offset = new Point(0,0);
                screenIter = new MockOneImageIterator(toImage(new Bitmap("test/lobby1.bmp")));
            }

			// screen + reduce + invert
            ColorReducer reducer = new ColorReducers.LobbyChars();
            ColorReducer reducerJoined = new ColorReducers.LobbyCharsJoined();
            ColorReplacer replacer = new ColorReplacer(Color.White, Color.Transparent);
            
			
			// renderer
			ImagesRenderer renderer = newImageRenderer();
			ColorReplacer replacerTrans = new ColorReplacer(Color.Transparent, Color.Cyan);
			
			// patterns
			List<CharPattern> patterns = CharReader.readCharsFromResourcesLobby();
			CharIdentifier identifier = new CharIdentifier(patterns);
			
			// identify
            while (screenIter.hasNext()) 
			{
                // rows
                List<LobbyTable> rows = new List<LobbyTable>();

                // image
                Image screen = screenIter.next();

                // list
                Image tableList = screen.crop(layout.TableList.X, layout.TableList.X + layout.TableList.Width,
                    layout.TableList.Y, layout.TableList.Y + layout.TableList.Height);

                // identify
                List<ValueWithY> playerCounts = identifyValues(tableList, layout.PlayersCountX, 
                    layout.PlayersCountX + layout.PlayersCountW, tableList.height,
                    reducer, replacer, identifier, renderer);
                List<ValueWithY> potSizes = identifyValues(tableList, layout.PotX,
                    layout.PotX + layout.PotW, tableList.height,
                    reducer, replacer, identifier, renderer);
                List<ValueWithY> flops = identifyValues(tableList, layout.FlopsX,
                    layout.FlopsX + layout.FlopsW, tableList.height,
                    reducer, replacer, identifier, renderer);

                List<LobbyTable> tables = new List<LobbyTable>();
                for (int i = 0; i < playerCounts.Count; i++)
                {
                    // location
                    int x = offset.X + layout.TableList.X + layout.TableList.Width / 2;
                    int y = offset.Y + layout.TableList.Y + playerCounts[i].Y;

                    // cell
                    int celly = playerCounts[i].Y;
                    int cellx = layout.PlayersCountX;
                    Image cell = tableList.crop(cellx, cellx + layout.PlayersCountW, celly, celly + layout.CellHeight);
                    bool joined = ContainsJoinedColor(cell, reducerJoined, renderer);

                    // table
                    tables.Add(new LobbyTable(i+1, (int)playerCounts[i].Value, potSizes[i].Value, (int)flops[i].Value, x, y, offset.X, offset.Y, joined));
                }

                // print
                foreach (LobbyTable table in tables)
                {
                    Console.WriteLine(table.ToString());
                }

                // wait
                Console.ReadKey();
			}
		}

        private static bool ContainsJoinedColor(Image cell, ColorReducer reducer, ImagesRenderer renderer)
        {
            cell = reducer.reduceColors(cell);
            addImage(renderer, toBitmap(cell));
            foreach (int pixel in cell.pixels)
            {
                if (pixel == ColorReducers.LobbyCharsJoined.JOINED_COLOR.ToArgb())
                {
                    return true;
                }
            }
            return false;
        }


        private static List<ValueWithY> identifyValues(Image tableList, int x, int width, int height, ColorReducer reducer,
            ColorReplacer replacer, CharIdentifier identifier, ImagesRenderer renderer)
        {
            // rows
            Image rows = tableList.crop(x, width, 0, height);

            // reduce & replace
            rows = reducer.reduceColors(rows);
            rows = replacer.replace(rows);

            // render
            setImage(renderer, toBitmap(rows));

            // chars
            List<ImageLine> lines = HorizontalPartitioner.partitionWithY(rows);

            // chars
            List<ValueWithY> result = new List<ValueWithY>();
            foreach (ImageLine line in lines)
            {
                String textLine = "";
                foreach (Image chars in line)
                {
                    List<Image> combos = CharDecomposer.decompose(chars);
                    foreach (Image chr in combos)
                    {
                        Image character = ImageCropper.crop(chr);
                        textLine += identifyChars(identifier, character);
                    }
                }

                // pure numbers
                string numericTextLine = textLine;
                foreach (char chr in textLine)
                {
                    if (!TextTools.IsNumeric(chr) && !TextTools.IsPoint(chr))
                    {
                        numericTextLine = numericTextLine.Replace(chr.ToString(), "");
                    }
                }

                // convert
                if (numericTextLine.Length != 0)
                {
                    double value = TextTools.ParseDouble(numericTextLine);
                    result.Add(new ValueWithY(value, line.Y));
                }
            }

            return result;
        }
        		
		public static String identifyChars(CharIdentifier identifier, Image image)
		{
			try
			{
				return identifier.identifyChars(image);
			}
			catch(UnknownCharException ex)
			{
                try
                {
                    Console.WriteLine("Unknown");
                    if (SAVE_UNKNOWN)
                    {
                        saveBitmap("lobby", 0, toBitmap(ex.image));
                    }
                }
                catch (Exception)
                {
                }
                return "?";
			}
		}
	}
}
