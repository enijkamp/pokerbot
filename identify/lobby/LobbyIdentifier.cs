using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PokerBot
{
    public class LobbyIdentifier
    {
        private bool SAVE_UNKNOWN = false;

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

            public ValueWithY(int y)
            {
                this.Y = y;
            }

            public bool HasValue { get { return Value != NO_VALUE; } }
        }

        // reducers
        private ColorReducer reducer = new ColorReducers.LobbyChars();
        private ColorReplacer replacer = new ColorReplacer(Color.White, Color.Transparent);
        private ColorReplacer replacerTrans = new ColorReplacer(Color.Transparent, Color.Cyan);
        private ColorReducer reducerJoined = new ColorReducers.LobbyCharsJoined();

        // patterns        
        private CharIdentifier identifier;

        // layout
        private LobbyLayout layout = new LobbyLayout();

        private ImageRenderer renderer;

        public LobbyIdentifier()
        {
            List<CharPattern> patterns = CharReader.readCharsFromResourcesLobby();
            identifier = new CharIdentifier(patterns);
        }

        public ImageRenderer Renderer
        {
            get { return renderer; }
            set { renderer = value; }
        }

        public List<LobbyTable> identifyLobbyTables(Image tableList, Point offset)
        {
            // render
            renderImage(tableList, new Point(0,0));            

            // identify
            List<ValueWithY> playerCounts = identifyValues(tableList, layout.PlayersCountX,
                layout.PlayersCountX + layout.PlayersCountW, tableList.height);
            List<ValueWithY> potSizes = identifyValues(tableList, layout.PotX,
                layout.PotX + layout.PotW, tableList.height);
            List<ValueWithY> flops = identifyValues(tableList, layout.FlopsX,
                layout.FlopsX + layout.FlopsW, tableList.height);

            List<LobbyTable> tables = new List<LobbyTable>();
            for (int i = 0; i < playerCounts.Count; i++)
            {
                // enough rows
                if (playerCounts.Count <= i || potSizes.Count <= i || flops.Count <= i)
                {
                    break;
                }
                // missing value
                if (!playerCounts[i].HasValue || !potSizes[i].HasValue || !flops[i].HasValue)
                {
                    continue;
                }

                // location
                int absX = offset.X + layout.TableList.X + layout.TableList.Width / 2;
                int absY = offset.Y + layout.TableList.Y + playerCounts[i].Y;

                // joined
                int celly = playerCounts[i].Y;
                int cellx = layout.PlayersCountX;
                Image cell = tableList.crop(cellx, cellx + layout.PlayersCountW, celly, celly + layout.CellHeight);
                bool joined = ContainsJoinedColor(cell, reducerJoined);

                // table
                LobbyTable table = new LobbyTable(i + 1, (int)playerCounts[i].Value, potSizes[i].Value,
                    (int)flops[i].Value, absX, absY, layout.PlayersCountX, 
                    playerCounts[i].Y + (int)(layout.CellHeight / 2), joined);
                if (!table.IsIncomplete)
                {
                    tables.Add(table);
                }
            }
            return tables;
        }

        private static bool ContainsJoinedColor(Image cell, ColorReducer reducer)
        {
            cell = reducer.reduceColors(cell);
            foreach (int pixel in cell.pixels)
            {
                if (pixel == ColorReducers.LobbyCharsJoined.JOINED_COLOR.ToArgb())
                {
                    return true;
                }
            }
            return false;
        }

        private List<ValueWithY> identifyValues(Image tableList, int x, int width, int height)
        {
            // rows
            Image rows = tableList.crop(x, width, 0, height);

            // reduce & replace
            Image reducedRows = reducer.reduceColors(rows);
            Image replacedRows = replacer.replace(reducedRows);

            // render
            renderImage(reducedRows, new Point(x, 0));

            // chars
            List<ImageLine> lines = HorizontalPartitioner.partitionWithY(replacedRows);

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
                else
                {
                    result.Add(new ValueWithY(line.Y));
                }
            }

            return result;
        }

        private String identifyChars(CharIdentifier identifier, Image image)
        {
            try
            {
                return identifier.identifyChars(image);
            }
            catch (UnknownCharException ex)
            {
                try
                {
                    Log.Warn("Unknown char in lobby");
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

        private Bitmap toBitmap(Image image)
        {
            Bitmap bmp = new Bitmap(image.width, image.height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(
                               new Rectangle(0, 0, bmp.Width, bmp.Height),
                               ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(image.pixels, 0, bmpData.Scan0, image.pixels.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private void saveBitmap(string name, int id, System.Drawing.Image image)
        {
            image.Save(name + "_" + id + ".png", ImageFormat.Png);
        }

        private void renderImage(Image image, Point point)
        {
            if (Renderer != null)
                Renderer.renderImage(image, point);
        }

        private void renderImage(Image image, Rectangle point)
        {
            if (Renderer != null)
                Renderer.renderImage(image, point.Location);
        }
    }
}
