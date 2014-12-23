using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace PokerBot
{
	public class PatternLocator
	{
        public static Point locatePattern(string patternName, ColorReducer reducer, Image screen)
        {
            // get a reference to the current assembly
            Log.Debug("screen dimensions {" + screen.width + "," + screen.height + "}");

            // read from assembly
            Log.Debug("reading " + patternName + " pattern");
            Image pattern = AssemblyTools.getAssemblyImage(patternName);

            // reduce colors
            Log.Debug("reducing pattern colors");
            pattern = reducer.reduceColors(pattern);
            screen = reducer.reduceColors(screen);

            // locate pattern
            Log.Fine("scanning lines ...");
            for (int y = 0; y < screen.height - pattern.height; y++)
            {
                Log.FineIf(y % 100 == 0, y + "/" + screen.height);
                for (int x = 0; x < screen.width - pattern.width; x++)
                {
                    Image sub = screen.crop(x, x + pattern.width, y, y + pattern.height);
                    if (ImageTools.match(sub, pattern))
                    {
                        Log.Info("found "+patternName+" pattern x=" + x + ", y=" + y);
                        return new Point(x, y);
                    }
                }
            }
            return Point.Empty;
        }

        private static Image iconPattern = Image.Empty;
        public static List<Point> locateTaskBarPrograms(Image screen, int xOff, int yOff)
        {
            // pattern
            Image originalScreen = screen;
            Image pattern = iconPattern;

            // reduce if real pattern not set
            if (pattern == Image.Empty)
            {
                pattern = AssemblyTools.getAssemblyImage("taskbar_icon.png");
                ColorReducer reducer = new ColorReducers.TaskBar();
                pattern = reducer.reduceColors(pattern);
                screen = reducer.reduceColors(screen);
            }            

            // locate pattern
            Dictionary<int, List<Point>> icons = new Dictionary<int, List<Point>>();
            for (int y = 0; y < screen.height - pattern.height; y++)
            {
                for (int x = 0; x < screen.width - pattern.width; x++)
                {
                    Image sub = screen.crop(x, x + pattern.width, y, y + pattern.height);
                    if (ImageTools.match(sub, pattern))
                    {
                        // remember pattern
                        if (iconPattern == Image.Empty)
                        {
                            iconPattern = originalScreen.crop(x, x + pattern.width, y, y + pattern.height);
                        }

                        // store
                        Log.Debug("found taskbar_icon.png pattern x=" + x + ", y=" + y);
                        Point point = new Point(xOff + x, yOff + y);
                        bool found = false;

                        // check for near y
                        foreach(int knownY in icons.Keys)
                        {
                            if (Math.Abs(y - knownY) < 5)
                            {
                                icons[knownY].Add(point);
                                found = true;
                                break;
                            }
                        }

                        // add new y
                        if (!found)
                        {
                            icons.Add(y, new List<Point>(){ new Point(xOff + x, yOff + y) });
                        }

                        // skip x
                        x += pattern.width;
                    }
                }
            }

            // sort
            Dictionary<int, List<Point>>.ValueCollection sorted = icons.Values;
            List<Point> result = new List<Point>();
            foreach (List<Point> points in sorted)
            {
                var sortedX = from p in points orderby p.X ascending select p;
                result.AddRange(sortedX);
            }
            return result;
        }

        public static Point locateLobbyLoginOk(Image screen)
        {
            return locatePattern("lobby_login_ok.png", new ColorReducers.LobbyLoginOk(), screen);
        }

        public static Point locateLobbyLogin(Image screen)
        {
            return locatePattern("lobby_login.png", new ColorReducers.LobbyLogin(), screen);
        }

        public static Point locateLobbyNewsPattern(Image screen)
        {
            return locatePattern("news_close.png", new ColorReducers.News(), screen);
        }

        private static Image notLoggedInLobbyPattern = AssemblyTools.getAssemblyImage("lobby_pattern_no_login.png");
        public static bool isNotLoggedInLobbyVisible(Image image)
        {
            return isPatternVisible(image, new ColorReducers.Lobby(), notLoggedInLobbyPattern, "lobby_pattern_no_login.png");
        }

        private static Image autoPostBlindPattern = AssemblyTools.getAssemblyImage("check.png");
        public static bool isAutoPostBlindsChecked(Image check)
        {
            return isPatternVisible(check, new ColorReducers.SitOut(), autoPostBlindPattern, "check.png");
        }

        private static Image sitOutPattern = AssemblyTools.getAssemblyImage("sit_out.png");
        public static bool isSitOutVisible(Image image)
        {
            return isPatternVisible(image, new ColorReducers.PostBlind(), sitOutPattern, "sit_out.png");
        }

        private static Image postBlindPattern = AssemblyTools.getAssemblyImage("post_blind.png");
        public static bool isPostBlindVisible(Image image)
        {
            return isPatternVisible(image, new ColorReducers.PostBlind(), postBlindPattern, "post_blind.png");
        }

        private static Image checkPattern = AssemblyTools.getAssemblyImage("check_or_fold.png");
        public static bool isCheckOrFoldMsgBoxVisible(Image image)
        {
            return isPatternVisible(image, new ColorReducers.CheckOrFold(), checkPattern, "check_or_fold.png");
        }        

        private static Image imBackPattern = AssemblyTools.getAssemblyImage("im_back.png");
        public static bool isImBackVisible(Image image)
        {
            return isPatternVisible(image, new ColorReducers.ImBack(), imBackPattern, "im_back.png");
        }

        private static Image buyInPattern = AssemblyTools.getAssemblyImage("table_buy_in.png");
        public static bool isBuyInVisible(Image image)
        {
            return isPatternVisible(image, new ColorReducers.BuyIn(), buyInPattern, "table_buy_in.png");
        }

        private static Image sitOutCheckPattern = AssemblyTools.getAssemblyImage("check.png");
        public static bool isSitOutBoxChecked(Image check)
        {
            return isPatternVisible(check, new ColorReducers.SitOut(), sitOutCheckPattern, "check.png");
        }

        private static Image sitOutUncheckPattern = AssemblyTools.getAssemblyImage("uncheck.png");
        public static bool isSitOutBoxUnchecked(Image check)
        {
            return isPatternVisible(check, new ColorReducers.SitOut(), sitOutUncheckPattern, "uncheck.png");
        }

        private static Image connectingPattern = AssemblyTools.getAssemblyImage("lobby_connecting.png");
        public static bool isConnectingVisible(Image check)
        {
            return isPatternVisible(check, new ColorReducers.Connecting(), connectingPattern, "lobby_connecting.png");
        }

        private static Image tableCornerBR = AssemblyTools.getAssemblyImage("table_right_bottom_corner.png");
        public static bool isTableBottomRightCornerVisible(Image check)
        {
            return isPatternVisible(check, new ColorReducers.TableBottomCorner(), tableCornerBR, "table_right_bottom_corner.png");
        }

        private static Image lobbyPattern = AssemblyTools.getAssemblyImage("lobby_pattern.png");
        public static bool isLoggedInLobbyVisible(Image screen)
        {
            return isPatternVisible(screen, new ColorReducers.Lobby(), lobbyPattern, "lobby_pattern.png");
        }

        private static Image errorPattern = AssemblyTools.getAssemblyImage("error.png");
        public static bool locateError(Image screen)
        {
            return isPatternVisible(screen, new ColorReducers.Error(), errorPattern, "error.png");
        }

        private static Image autoBlindPattern = AssemblyTools.getAssemblyImage("blind_pattern.png");
        public static bool locateAutoPostBlinds(Image screen)
        {
            return isPatternVisible(screen, new ColorReducers.AutoBlind(), autoBlindPattern, "blind_pattern.png");
        }

        private static bool isPatternVisible(Image image, ColorReducer reducer, Image pattern, string patternName)
        {
            // reduce colors
            Log.Debug("reducing pattern colors");
            pattern = reducer.reduceColors(pattern);
            image = reducer.reduceColors(image);

            // locate pattern
            Log.Debug("scanning lines ...");
            for (int y = 0; y < image.height - pattern.height; y++)
            {
                Log.FineIf(y % 100 == 0, y + "/" + image.height);
                for (int x = 0; x < image.width - pattern.width; x++)
                {
                    Image sub = image.crop(x, x + pattern.width, y, y + pattern.height);
                    if (ImageTools.match(sub, pattern))
                    {
                        Log.Info("found " + patternName + " pattern x=" + x + ", y=" + y);
                        return true;
                    }
                }
            }
            return false;
        }

        private static Image tablePattern = Image.Empty;
        public static Point locateUnknownTable(Image screen, List<Point> offsets, TableLayout layout)
        {
            // vars
            Image originalScreen = screen;
            Image pattern = tablePattern;
            DateTime start = DateTime.Now;

            // reduce or use exact image
            if (pattern == Image.Empty)
            {
                // read from assembly
                Log.Debug("reading table pattern");
                pattern = AssemblyTools.getAssemblyImage("table_top_left_corner.png");

                // reduce colors
                Log.Debug("reducing pattern colors");
                ColorReducer reducer = new ColorReducers.Table();
                screen = reducer.reduceColors(screen);
                pattern = reducer.reduceColors(pattern);
            }            

            // first line
            int[] patternFirstLine = pattern.lines[0];

            // locate pattern
            for (int y = 0; y < screen.height - pattern.height; y++)
            {
                int[] screenLine = screen.lines[y];
                for (int x = 0; x < screen.width - pattern.width; x++)
                {
                    // check first line
                    if (match(patternFirstLine, screenLine, x))
                    {
                        // first corner
                        Image cornerTL = screen.crop(x, x + pattern.width, y, y + pattern.height);                        
                        if (ImageTools.match(cornerTL, pattern))
                        {
                            // second corner
                            Image cornerBR = originalScreen.crop(x + layout.CornerBottom.X, x + layout.CornerBottom.X + layout.CornerBottom.Width,
                            y + layout.CornerBottom.Y, y + layout.CornerBottom.Y + layout.CornerBottom.Height);
                            if (isTableBottomRightCornerVisible(cornerBR))
                            {
                                // remember exact pattern
                                if (tablePattern == Image.Empty)
                                {
                                    tablePattern = originalScreen.crop(x, x + pattern.width, y, y + pattern.height);
                                }

                                // return location
                                Point point = new Point(x, y);
                                if (!offsets.Contains(point))
                                {
                                    Log.Info("found new table pattern x=" + x + ", y=" + y + " took " + DateTime.Now.Subtract(start).TotalMilliseconds + "ms");
                                    return point;
                                }
                            }
                        }
                    }
                }
            }
            return Point.Empty;
        }

        private static bool match(int[] pattern, int[] screen, int xOff)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] != screen[xOff + i])
                {
                    return false;
                }
            }
            return true;
        }

        public static Point locateTable(Image screen)
		{
			// get a reference to the current assembly
			Log.Debug("screen dimensions {"+screen.width+","+screen.height+"}");
			
			// read from assembly
            Log.Debug("reading table pattern");
            Image pattern = AssemblyTools.getAssemblyImage("table_top_left_corner.png");
			
			// reduce colors
            Log.Debug("reducing pattern colors");
			ColorReducer reducer = new ColorReducers.Table();
			pattern = reducer.reduceColors(pattern);
			screen = reducer.reduceColors(screen);
			
			// locate pattern
            Log.Fine("scanning lines ...");
			for(int y = 0; y < screen.height - pattern.height; y++)
			{
                Log.FineIf(y % 100 == 0, y + "/" + screen.height);
				for(int x = 0; x < screen.width - pattern.width; x++)
				{
					Image sub = screen.crop(x, x + pattern.width, y, y + pattern.height);
                    if (ImageTools.match(sub, pattern))
					{
                        Log.Info("found table pattern x=" + x + ", y=" + y);
                        return new Point(x, y);
					}
				}
			}			
			throw new Exception("Table pattern not found");					
		}

        public static Point locateLobby(Image screen)
        {
            // get a reference to the current assembly
            Log.Debug("screen dimensions {" + screen.width + "," + screen.height + "}");

            // read from assembly
            Log.Debug("reading lobby pattern");
            Image pattern = AssemblyTools.getAssemblyImage("lobby_pattern.png");

            // reduce colors
            Log.Debug("reducing pattern colors");
            ColorReducer reducer = new ColorReducers.Lobby();
            pattern = reducer.reduceColors(pattern);
            screen = reducer.reduceColors(screen);

            // locate pattern
            Log.Fine("scanning lines ...");
            for (int y = 0; y < screen.height - pattern.height; y++)
            {
                Log.FineIf(y % 100 == 0, y + "/" + screen.height);
                for (int x = 0; x < screen.width - pattern.width; x++)
                {
                    Image sub = screen.crop(x, x + pattern.width, y, y + pattern.height);
                    if (ImageTools.match(sub, pattern))
                    {
                        Log.Info("found lobby pattern x=" + x + ", y=" + y);
                        return new Point(x, y);
                    }
                }
            }
            throw new Exception("Lobby pattern not found");
        }
	}
}
