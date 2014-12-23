using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.IO;

namespace PokerBot
{
    public class TableOpener
    {
        public delegate void TableFoundDelegate(TableContainer table);

        private static TableContainer TABLE_NOT_FOUND = TableContainer.EmptyContainer();
        private static TableContainer TABLE_ALREADY_JOINED = TableContainer.EmptyContainer();
        private static TableContainer CANNOT_FIND_SEAT = TableContainer.EmptyContainer();       

        private const double MIN_AVG_POT_SIZE = .3;
        private const int MIN_PLAYERS_ON_TABLE = 7;
        private const int MAX_OPEN_SEATS = 9 - MIN_PLAYERS_ON_TABLE;

        private Size cornerTopLeftSize = new Size(30, 15);
        private Rectangle cornerBottomRightRect = new Rectangle(785, 538, 5, 5);

        private List<TableContainer> knownTables = new List<TableContainer>();
        private DeviceControl deviceControl;
        private TableIdentifier tableIdentifier;
        private Mouse mouse;
        private Keyboard keyboard;
        private Random random = new Random();
        private Image seatOccupiedPattern;
        private Controller controller;
        private LobbyLayout lobbyLayout = new LobbyLayout();

        private static int globalTableNumber = 0;

        public TableOpener(DeviceControl deviceControl, Mouse mouse, Keyboard keyboard, TableIdentifier tableIdentifier)
        {
            this.deviceControl = deviceControl;
            this.mouse = mouse;
            this.keyboard = keyboard;
            this.tableIdentifier = tableIdentifier;
            this.controller = new Controller(keyboard, mouse, tableIdentifier, new ScreenImageIterator(deviceControl));

            // seat occupied pattern
            seatOccupiedPattern = AssemblyTools.getAssemblyImage("error.png");
        }

        private int RandomInt(int begin, int end)
        {
            return (int)(random.NextDouble() * Math.Abs(end - begin)) + begin;
        }

        public void StartPokerApplication(Settings settings)
        {
            Iterator<Image> screen = new ScreenImageIterator(deviceControl);
            int width = deviceControl.DisplayWidth;
            int height = deviceControl.DisplayHeight;

            ThreadStart bootup = delegate()
            {
                // start pokerstars
                {
                    Log.Info("## starting poker application ##");
                    TextToSpeech.SayAsnc(settings, "starting poker application");

                    int tries = 0;
                    while (true)
                    {
                        // tries
                        tries++;
                        if (tries > 10)
                        {
                            throw new Exception("cannot open application, lobby not found");
                        }

                        // click
                        Log.Info("double click poker application icon");
                        mouse.Move(lobbyLayout.Icon.X, lobbyLayout.Icon.Y);
                        mouse.LeftClick();
                        Thread.Sleep(500);
                        keyboard.pressEnter();

                        // check
                        mouse.Move(300, 10);
                        Thread.Sleep(10000);
                        if (PatternLocator.isNotLoggedInLobbyVisible(screen.next()))
                        {
                            break;
                        }
                    }
                }

                // news
                {
                    Log.Info("checking for news window");
                    Thread.Sleep(4000);
                    int xOff = (int)(width * .4), yOff = (int)(height * .5);
                    Image newsScreen = screen.next().crop(xOff, (int)(width * .8), yOff, (int)(height * .8));
                    Point newsClose = PatternLocator.locateLobbyNewsPattern(newsScreen);
                    if (newsClose != Point.Empty)
                    {
                        mouse.MoveAndLeftClick(xOff + newsClose.X + 5, yOff + newsClose.Y + 5, 4, 4);
                    }
                }

                // login
                {
                    Thread.Sleep(2000);
                    Log.Info("pressing login");
                    int xOff = (int)(width * .7), yOff = (int)(height * .6);
                    Image loginScreen = screen.next().crop(xOff, width, yOff, height);
                    Point login = PatternLocator.locateLobbyLogin(loginScreen);
                    if (login != Point.Empty)
                    {
                        mouse.MoveAndLeftClick(xOff + login.X + 5, yOff + login.Y + 5, 10, 10);
                    }
                    else
                    {
                        throw new Exception("cannot find login");
                    }
                }

                // ok
                {
                    Thread.Sleep(3000);
                    Log.Info("pressing login ok");
                    int xOff = (int)(width * .4), yOff = (int)(height * .4);
                    Image okScreen = screen.next().crop(xOff, (int)(width * .7), yOff, height);
                    Point ok = PatternLocator.locateLobbyLoginOk(okScreen);
                    if (ok != Point.Empty)
                    {
                        mouse.MoveAndLeftClick(xOff + ok.X, yOff + ok.Y, 4, 4);
                    }
                    else
                    {
                        throw new Exception("cannot find login ok");
                    }
                }

                // news
                {
                    Log.Info("checking for news window");
                    Thread.Sleep(3000);
                    int xOff = (int)(width * .4), yOff = (int)(height * .5);
                    Image newsScreen = screen.next().crop(xOff, (int)(width * .8), yOff, (int)(height * .8));
                    Point newsClose = PatternLocator.locateLobbyNewsPattern(newsScreen);
                    if (newsClose != Point.Empty)
                    {
                        mouse.MoveAndLeftClick(xOff + newsClose.X + 5, yOff + newsClose.Y + 5, 4, 4);
                    }
                }

                // wait for layout (VIP status)
                {
                    Log.Info("waiting for layout (VIP status)");
                    Thread.Sleep(1000);
                }
            };

            // main logic
            for(int bootups = 0; bootups < 10; bootups++)
            {
                // boot
                bootup();                

                // check network connection
                int tries = 0;
                while (true)
                {
                    // tries
                    tries++;
                    if (tries > 10)
                    {
                        Log.Error("poker application is not connected - restarting");
                        keyboard.pressAltF4();
                        break;
                    }

                    // check
                    Thread.Sleep(1000);
                    Image screenshot = screen.next();
                    int xOff = (int)(width * .4), yOff = (int)(height * .4);
                    Image network = screen.next().crop(xOff, (int)(width * .6), yOff, (int)(height * .7));
                    if (!PatternLocator.isConnectingVisible(network))
                    {
                        return;
                    }
                }
            }

            // error
            throw new Exception("cannot start poker application");
        }

        public void StopPokerApplication(Settings settings)
        {
            Log.Info("## stopping poker application ##");
            TextToSpeech.SayAsnc(settings, "stopping poker application");

            // select lobby
            {
                Iterator<Image> screen = new ScreenImageIterator(deviceControl);
                FocusLobby(lobbyLayout, screen);
            }

            // close window
            {
                Thread.Sleep(2000);
                keyboard.pressAltF4AndEnter();
            }
        }

        private Point TaskBarLobby(LobbyLayout layout)
        {
            return new Point(layout.Taskbar.X + 25, layout.Taskbar.Y + 10);
        }

        private List<Point> TaskBarPrograms(LobbyLayout lobby, Image screenshot)
        {
            // screenshot
            Log.Debug("device control display dimensions {W=" + deviceControl.DisplayWidth + ",H=" 
                + deviceControl.DisplayHeight + "} crop area {X="+lobby.Taskbar.X+",Y="+lobby.Taskbar.Y+"}");
            Image taskbar = screenshot.crop(lobby.Taskbar.X, deviceControl.DisplayWidth,
                lobby.Taskbar.Y, deviceControl.DisplayHeight - 2);
            List<Point> icons = PatternLocator.locateTaskBarPrograms(taskbar, lobby.Taskbar.X, lobby.Taskbar.Y);
            Log.Info("found "+icons.Count+" taskbar icons");
            return icons;
        }

        public void SitOut()
        {            
            // taskbar position and remove lobby
            Iterator<Image> screen = new ScreenImageIterator(deviceControl);
            List<Point> taskBarPositions = TaskBarPrograms(lobbyLayout, screen.next());            
            taskBarPositions.RemoveAt(0);

            // tables
            List<TableContainer> tablesNotFocused = new List<TableContainer>(knownTables);

            // sit out
            foreach (Point position in taskBarPositions)
            {
                TableContainer table = FocusTable(new LobbyLayout(), tablesNotFocused, position);
                if (table != null)
                {
                    tablesNotFocused.Remove(table);
                    Controller.PressSitOut(mouse, table.Layout);
                }
            }
        }

        public void MinimizeAllTables()
        {
            deviceControl.KeyboardSendMinimizeAllWindows();
        }

        public void SitIn()
        {
            // move mouse to safe location
            mouse.Move(10, deviceControl.DisplayHeight - 300);

            // minimize tables
            Iterator<Image> screen = new ScreenImageIterator(deviceControl);
            MinimizeAllTables();

            // focus tables            
            List<TableContainer> tablesNotFocused = new List<TableContainer>(knownTables);
            int lastTaskBarPosition = 1;
            while (true)
            {
                // next
                List<Point> taskBarPositions = TaskBarPrograms(lobbyLayout, screen.next());

                // break
                if (lastTaskBarPosition == taskBarPositions.Count) break;

                // icon
                Point taskBarPosition = taskBarPositions[lastTaskBarPosition];

                // focus
                TableContainer foundTable = FocusTable(lobbyLayout, tablesNotFocused, taskBarPosition);

                // process
                if (foundTable != TABLE_NOT_FOUND)
                {
                    // sit-in
                    Log.Info("table " + (foundTable.Number+1) + " found -> sit-in");
                    tablesNotFocused.Remove(foundTable);
                    PressSitIn(foundTable, screen);

                    // next icon
                    lastTaskBarPosition++;
                }
                else
                {
                    // close if we cannot focus this table
                    Log.Error("table not found -> closing table");
                    CloseLastTable(taskBarPosition);
                    mouse.Move(taskBarPosition.X, taskBarPosition.Y - 300);
                }
            }
            // remove tables
            foreach (TableContainer table in tablesNotFocused)
            {
                Log.Error("removing offset with no associated table -> x=" + table.Layout.Offset.X + " y=" + table.Layout.Offset.Y);
                knownTables.Remove(table);
                table.Close();
            }
        }

        private void CloseLastTable(Point taskbarIcon)
        {
            mouse.Move(taskbarIcon.X, taskbarIcon.Y);
            Thread.Sleep(500);
            mouse.RightClick();
            Thread.Sleep(500);
            keyboard.pressKeys("c");
            Thread.Sleep(500);
        }

        private void CloseLastTable(Iterator<Image> screen)
        {
            // move mouse to safe location
            mouse.Move(500, deviceControl.DisplayHeight - 300);

            // taskbar icons
            List<Point> taskBarPositions = TaskBarPrograms(lobbyLayout, screen.next());

            // close last
            if (taskBarPositions.Count > 1)
            {
                CloseLastTable(taskBarPositions[taskBarPositions.Count - 1]);
            }
        }

        private void PressSitIn(TableContainer table, Iterator<Image> screen)
        {
            int tries = 0;
            while (true)
            {
                // move mouse
                int xOff = RandomInt(15, table.Layout.SitOutClick.Width);
                mouse.Move(table.Layout.Offset.X + table.Layout.SitOutClick.X + xOff, table.Layout.Offset.Y + table.Layout.SitOutClick.Y);

                // shot
                Image shot = screen.next();
                bool isChecked = PatternLocator.isSitOutBoxChecked(CropCheckImage(shot, table.Layout));

                // sanity
                if (tries == 0 && !isChecked)
                {
                    Log.Error("Sit-Out box is not checked");
                }

                // press buttons
                if (PressCheckOrFold(shot))
                {
                    mouse.Move(table.Layout.Offset.X + table.Layout.SitOutClick.X, table.Layout.Offset.Y + table.Layout.SitOutClick.Y);
                    MinimizeAllTables();
                    continue;
                }

                // sit-in
                if (isChecked)
                {
                    // click
                    mouse.LeftClick();

                    // double check
                    bool isUnchecked = PatternLocator.isSitOutBoxUnchecked(CropCheckImage(screen.next(), table.Layout));

                    // continue
                    if (isUnchecked)
                    {
                        table.SitIn();
                        break;
                    }
                }

                // tries
                if (tries > 5)
                {
                    ErrorHandler.ReportExceptionWithImage(new Exception("cannot sit in on table"), "sit-in", shot);
                    table.Close();
                    break;
                }
                tries++;
            }
        }

        private TableContainer FocusTable(LobbyLayout layout, List<TableContainer> candidates, Point taskBarPosition)
        {
            Iterator<Image> screen = new ScreenImageIterator(deviceControl);
            int tries = 0;
            while (true)
            {
                // tries
                if (tries > 2)
                {
                    ErrorHandler.ReportException(new Exception("cannot focus table"), "focus table");
                    return TABLE_NOT_FOUND;
                }

                // check for other window
                Image screenshot = screen.next();

                // press buttons
                if (PressCheckOrFold(screenshot))
                {
                    continue;
                }

                // click task bar
                int randomOff = (int)(random.NextDouble() * 3);
                mouse.Move(taskBarPosition.X + randomOff, taskBarPosition.Y + randomOff);
                Thread.Sleep(150);
                mouse.LeftClick();
                Thread.Sleep(350);

                // check for other window
                screenshot = screen.next();

                // check focus
                foreach (TableContainer table in candidates)
                {
                    if (IsTableVisible(table, CropTable(screenshot, table.Layout)))
                    {
                        return table;
                    }
                }

                // move somewhere else
                mouse.Move(taskBarPosition.X - 100, taskBarPosition.Y - 50);
                Thread.Sleep(200 + tries * 100);

                // tries
                tries++;
            }
        }

        private Image CropCheckImage(Image screenshot, TableLayout layout)
        {
            Image tableImage = CropTable(screenshot, layout);
            Image checkImage = tableImage.crop(layout.SitOutPattern.X, layout.SitOutPattern.X + layout.SitOutPattern.Width,
                layout.SitOutPattern.Y, layout.SitOutPattern.Y + layout.SitOutPattern.Width);
            return checkImage;
        }

        private bool PressCheckOrFold(Image screenshot)
        {
            // tables
            bool action = false;
            foreach (TableContainer container in KnownTables)
            {
                // table image				
                Image unsafeTableImage = CropTable(screenshot, container.Layout);

                // handling table
                bool isTableVisible = IsTableVisible(container, unsafeTableImage);
                bool areControlsActive = isTableVisible ? tableIdentifier.identifyMove(unsafeTableImage) : false;
                Log.Debug("# table" + container.Number + ": vis=" + isTableVisible + " buttons=" + areControlsActive);
                if (isTableVisible && areControlsActive)
                {
                    Log.Info("table " + (container.Number + 1) + " pressing check or fold");
                    List<TableControl> controls = tableIdentifier.identifyControls(unsafeTableImage);
                    controller.PressCheckOrFold(container.Layout, controls);
                    action = true;
                }
            }
            return action;
        }

        private void FocusLobby(LobbyLayout layout, Iterator<Image> screen)
        {
            Log.Info("selecting lobby");
            for (int tries = 0; tries < 5; tries++)
            {
                // check
                {
                    Image lobbyCorner = screen.next().crop(0, 300, 0, 300);
                    if (PatternLocator.isLoggedInLobbyVisible(lobbyCorner))
                    {
                        return;
                    }
                }

                // focus
                {
                    Point taskbarLobby = TaskBarLobby(layout);
                    mouse.MoveAndLeftClick(taskbarLobby.X, taskbarLobby.Y, 10, 10);
                    Thread.Sleep(700);
                }

                // check
                {
                    Image lobbyCorner = screen.next().crop(0, 300, 0, 300);
                    if (PatternLocator.isLoggedInLobbyVisible(lobbyCorner))
                    {
                        return;
                    }
                }
            }
            throw new Exception("cannot focus lobby");
        }

        public void ResetTables()
        {
            knownTables.Clear();
        }

        public List<TableContainer> KnownTables
        {
            get { return knownTables; }
        }

        public TableContainer LocateNewTable(Settings settings)
        {
            Log.Info("## scanning for tables ##");

            // find offset
            Iterator<Image> screen = new ScreenImageIterator(deviceControl);
            Image screenshot = screen.next();
            Point foundOffset = PatternLocator.locateUnknownTable(screenshot, Offsets(knownTables), new TableLayout9());
            if (foundOffset == Point.Empty)
            {
                Log.Error("No table found");
                throw new ArgumentException("No table found");
            }

            // check for new table and find seat
            Image cornerTopLeft = CropTableTopLeftCorner(screenshot, foundOffset);
            Image cornerBottomRight = CropTableBottomRightCorner(screenshot, foundOffset); 
            if (!IsOffsetKnown(foundOffset))
            {
                // corner
                Log.Info("unknown table found at x=" + foundOffset.X + " y=" + foundOffset.Y);
                TableLayout layout = new TableLayout9(foundOffset);

                // find seat
                string player = settings.Name;
                Log.Info("looking for '" + player + "' on table" + (knownTables.Count + 1));
                int seat = -1;
                for (int tries = 0; tries < 3; tries++)
                {
                    Table previousTable = tableIdentifier.identifyTableForLocation(CropTable(screen.next(), layout));
                    seat = BotAppLogic.IdentifySeat(previousTable, player);
                    if (seat != -1) break;
                    Thread.Sleep(500);
                }
                if (seat == -1)
                {
                    Log.Error("Cannot find player position on table " + (knownTables.Count + 1));
                    throw new ArgumentException("Cannot find player position on table " + (knownTables.Count + 1));
                }
                Log.Info("my seat on new table " + (knownTables.Count + 1) + " is " + (seat + 1));

                // blinds
                Image tableImage = CropTable(screenshot, layout);
                Image blindsImage = tableImage.crop(layout.AutoBlindPattern.X, layout.AutoBlindPattern.X + layout.AutoBlindPattern.Width,
                    layout.AutoBlindPattern.Y, layout.AutoBlindPattern.Y + layout.AutoBlindPattern.Height);
                bool isFast = !PatternLocator.locateAutoPostBlinds(blindsImage);

                // container
                TableContainer table = new TableContainer(knownTables.Count, cornerTopLeft, cornerBottomRight, layout, seat, isFast, LobbyTable.Empty);
                knownTables.Add(table);
                return table;
            }
            else
            {
                throw new Exception("No unknown table found");
            }
        }
        
        private List<Point> Offsets(List<TableContainer> tables)
        {
            List<Point> offsets = new List<Point>();
            foreach (TableContainer container in tables)
            {
                offsets.Add(container.Layout.Offset);
            }
            return offsets;
        }

        public static Image CropTable(Image screen, TableLayout layout)
        {
            return screen.crop(layout.Offset.X, layout.Offset.X + layout.Size.Width, layout.Offset.Y, layout.Offset.Y + layout.Size.Height);
        }

        public bool IsTableVisible(TableContainer container, Image image)
        {
            Image screenTopLeftCorner = CropTableTopLeftCorner(image, new Point(0, 0));
            Image screenBottomRightCorner = CropTableBottomRightCorner(image, new Point(0, 0));
            bool areBothCornersVisible = ImageTools.match(screenTopLeftCorner, container.CornerTopLeft)
                && ImageTools.match(screenBottomRightCorner, container.CornerBottomRight);
            return areBothCornersVisible;
        }

        private Image CropTableTopLeftCorner(Image Image, Point offset)
        {
            return Image.crop(offset.X, offset.X + cornerTopLeftSize.Width, offset.Y, offset.Y + cornerTopLeftSize.Height);
        }

        private Image CropTableBottomRightCorner(Image Image, Point offset)
        {
            int xStart = offset.X + cornerBottomRightRect.X;
            int xEnd = xStart + cornerBottomRightRect.Width;
            int yStart = offset.Y + cornerBottomRightRect.Y;
            int yEnd = yStart + cornerBottomRightRect.Width;
            return Image.crop(xStart, xEnd, yStart, yEnd);
        }

        private bool IsOffsetKnown(Point offset)
        {
            foreach (TableContainer table in knownTables)
            {
                if (table.Equals(offset))
                {
                    return true;
                }
            }
            return false;
        }
        
        public List<TableContainer> OpenNewTables(Settings settings)
        {
            return OpenNewTables(settings, null, null);
        }

        public List<TableContainer> OpenNewTables(Settings settings, ImageRenderer imageRenderer)
        {
            return OpenNewTables(settings, imageRenderer, null);
        }

        public List<TableContainer> OpenNewTables(Settings settings, TableFoundDelegate tableDelegate)
        {
            return OpenNewTables(settings, null, tableDelegate);
        }

        public List<TableContainer> OpenNewTables(Settings settings, ImageRenderer imageRenderer, TableFoundDelegate tableDelegate)
        {
            Log.Info("## scanning for lobby ##");

            // identifier
            LobbyIdentifier lobbyIdentifier = new LobbyIdentifier();

            // set renderer
            tableIdentifier.Renderer = imageRenderer;
            lobbyIdentifier.Renderer = imageRenderer;

            // screen
            Iterator<Image> screen = new ScreenImageIterator(deviceControl);            
            LobbyLayout layout = new LobbyLayout();

            // find lobby
            FocusLobby(layout, screen);
            Image screenshot = screen.next();
            Point lobbyOffset = PatternLocator.locateLobby(screenshot);            

            // loop until we got enough tables
            List<TableContainer> tables = new List<TableContainer>();
            while (knownTables.Count < settings.AutoLocateTablesNum)
            {
                // check for tables
                PressCheckOrFold(screenshot);

                // scan lobby
                List<LobbyTable> lobbyTables = IdentifyLobbyTables(lobbyIdentifier, layout, screen, lobbyOffset);

                // open tables?
                if (HasOpenTables(lobbyTables))
                {
                    // select table
                    LobbyTable bestTable = SelectBestTable(lobbyTables);

                    // render
                    if (imageRenderer != null) imageRenderer.renderImage(ImageTools.rectImage(), new Point(bestTable.RelX, bestTable.RelY));

                    // open table
                    Log.Info("opening table " + bestTable.ToString() + " with score " + String.Format("{0:0.00}", bestTable.Score));
                    TableContainer container = OpenTable(bestTable, settings, screen, bestTable);
                    if (container != TABLE_ALREADY_JOINED && container != TABLE_NOT_FOUND && container != CANNOT_FIND_SEAT)
                    {
                        // add to known tables
                        tables.Add(container);
                        knownTables.Add(container);
                        Log.Info("added new table " + tables.Count);

                        // invoke delegate
                        if (tableDelegate != null)
                        {
                            tableDelegate(container);
                        }
                    }

                    // reselect lobby (otherwise automatically selected by pokerstars -> close table)
                    if (container == TABLE_NOT_FOUND || knownTables.Count > 0)
                    {
                        FocusLobby(layout, screen);
                        keyboard.pressPageUp(3);
                    }
                    // move up for "better" tables
                    else
                    {
                        keyboard.pressPageUp(2);
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    // scroll
                    keyboard.pressPageDown();
                    Thread.Sleep(1000);
                }
            }

            // minimize lobby
            Thread.Sleep(1000);

            return tables;
        }

        private List<LobbyTable> IdentifyLobbyTables(LobbyIdentifier lobbyIdentifier, LobbyLayout layout, Iterator<Image> screen, Point offset)
        {
            for (int tries = 0; tries < 10; tries++)
            {
                // time
                DateTime startLobby = DateTime.Now;

                // select last row
                mouse.MoveAndLeftClick(offset.X + layout.LastRow.X, offset.Y + layout.LastRow.Y, 20, 5);

                // move mouse
                mouse.Move(offset.X + layout.Mouse.X, offset.Y + layout.Mouse.Y);
                Thread.Sleep(100);

                // shot
                Image screenshot = screen.next();

                // crop                
                Image window = screenshot.crop(offset.X, screenshot.width, offset.Y, screenshot.height);
                Image tableList = window.crop(layout.TableList.X, layout.TableList.X + layout.TableList.Width,
                    layout.TableList.Y, layout.TableList.Y + layout.TableList.Height);
                try
                {
                    List<LobbyTable> lobbyTables = lobbyIdentifier.identifyLobbyTables(tableList, offset);

                    // log
                    Log.Info("identified " + lobbyTables.Count + " joinable lobby tables");

                    // nothing found?
                    if (lobbyTables.Count == 0)
                    {
                        throw new Exception("identified 0 lobby tables, trying again");
                    }

                    return lobbyTables;
                }
                catch (Exception ex)
                {
                    // handle error
                    ErrorHandler.ReportExceptionWithImage(ex, "lobby identifier error", tableList);

                    // sleep
                    Thread.Sleep(1000);

                    // focus lobby
                    FocusLobby(layout, screen);

                    continue;
                }
            }
            throw new Exception("cannot identify lobby table list");
        }

        private TableContainer OpenTable(LobbyTable table, Settings settings, Iterator<Image> screen, LobbyTable lobby)
        {
            // count programs
            List<Point> taskBarBefore = TaskBarPrograms(lobbyLayout, screen.next());

            // move mouse to row
            mouse.MoveAndLeftClick(table.AbsX, table.AbsY + 4, 10, 2);
            keyboard.pressEnter();
            Thread.Sleep(450);

            // locate table
            Point tableOffset = Point.Empty;
            Image screenshot = screen.next();
            int tries = 0;
            while (true)
            {
                // find
                tableOffset = PatternLocator.locateUnknownTable(screenshot, Offsets(knownTables), new TableLayout9());
                if (tableOffset != Point.Empty)
                {
                    // found table
                    break;
                }
                else if (tries++ > 4)
                {
                    // cannot locate
                    break;
                }
                else
                {
                    // next try
                    Thread.Sleep(500);
                    screenshot = screen.next();
                }
            }
            
            // not found?
            if(tableOffset == Point.Empty)
            {
                // safe location
                mouse.Move(500, deviceControl.DisplayHeight - 300);

                // close if not found (but is actually there)
                List<Point> taskBarAfter = TaskBarPrograms(lobbyLayout, screen.next());
                if (taskBarAfter.Count > taskBarBefore.Count)
                {
                    // log
                    Log.Info("table not found - validating known offsets");

                    // check known offsets
                    ValidateKnownTables();

                    // error -> table not found
                    ErrorHandler.ReportExceptionWithImage(new Exception("table not found"), "open table failed", screenshot);
                    return TABLE_NOT_FOUND;
                }
                else
                {
                    // error -> table already joined
                    ErrorHandler.ReportExceptionWithImage(new Exception("table already joined"), "open table failed", screenshot);
                    return TABLE_ALREADY_JOINED;
                }                
            }

            // check for 'you have just left this table'
            if (YouHaveJustLeftThisTable(new TableLayout9(tableOffset), screenshot))
            {
                Log.Info("'you have just left this table' is visible -> closing table");
                keyboard.pressEnter();
                Thread.Sleep(500);
                CloseLastTable(screen);
                return TABLE_NOT_FOUND;
            }

            // find open seat
            Thread.Sleep(200);
            return SeatPlayer(tableOffset, screen, lobby);
        }

        private void ValidateKnownTables()
        {
            // log
            Log.Info("checking known table offsets");

            // screenshots
            Iterator<Image> screen = new ScreenImageIterator(deviceControl);

            // move mouse to safe location
            mouse.Move(10, deviceControl.DisplayHeight - RandomInt(300, 350));

            // focus tables            
            List<TableContainer> tablesNotFocused = new List<TableContainer>(knownTables);
            int lastTaskBarPosition = 1;
            while (true)
            {
                // minimize tables
                MinimizeAllTables();

                // sleep
                Thread.Sleep(300);

                // next
                List<Point> taskBarPositions = TaskBarPrograms(lobbyLayout, screen.next());

                // break
                if (lastTaskBarPosition >= taskBarPositions.Count) break;

                // icon
                Point taskBarPosition = taskBarPositions[lastTaskBarPosition];

                // focus
                TableContainer foundTable = FocusTable(lobbyLayout, tablesNotFocused, taskBarPosition);

                // process
                if (foundTable != TABLE_NOT_FOUND)
                {
                    // ## 1 : table found ##
                    // found
                    Log.Info("table " + (foundTable.Number + 1) + " found");
                    tablesNotFocused.Remove(foundTable);

                    // next icon
                    lastTaskBarPosition++;
                }
                else
                {
                    // ## 2 : table unknown but has taskbar icon ##
                    // close if we cannot focus this table
                    Log.Error("closing table -> no known offset found");
                    CloseLastTable(taskBarPosition);
                    Thread.Sleep(500);
                    keyboard.pressEnter();
                    Thread.Sleep(500);
                }

                // move mouse to safe location
                mouse.Move(taskBarPosition.X, deviceControl.DisplayHeight - RandomInt(300, 350));
            }

            // remove offsets with no associated table
            foreach (TableContainer table in tablesNotFocused)
            {
                // ## 3 : table offset with no taskbar icon ##
                Log.Error("removing offset with no associated table -> x=" + table.Layout.Offset.X + " y=" + table.Layout.Offset.Y);
                knownTables.Remove(table);
                table.Close();
            }

            // restore state
            MinimizeAllTables();
        }

        private TableContainer SeatPlayer(Point offset, Iterator<Image> screenIter, LobbyTable lobby)
        {
            TableLayout layout = new TableLayout9(offset);

            // crop
            Image screenshot = screenIter.next();
            Image tableImage = CropTable(screenshot, layout);

            // find empty seat
            TableIdentifier.OpenSeat openSeat = tableIdentifier.identifyOpenSeat(tableImage);

            // no open seat found or wrong table (too many open seats)
            if (openSeat == TableIdentifier.OpenSeat.NOT_FOUND || openSeat.TotalSeatsOpen > MAX_OPEN_SEATS)
            {
                Log.Info("table full or too many open seats");

                // close window
                CloseLastTable(screenIter);
                Thread.Sleep(250);
                MinimizeAllTables();
                Thread.Sleep(250);
                return CANNOT_FIND_SEAT;
            }
            else
            {
                // seat player
                int x = offset.X + openSeat.Offset.X + openSeat.Offset.Width / 2;
                int y = offset.Y + openSeat.Offset.Y + openSeat.Offset.Height / 2;
                int rx = (int)(openSeat.Offset.Width / 2.5);
                int ry = (int)(openSeat.Offset.Height / 2.5);
                mouse.MoveAndLeftClick(x, y, rx, ry);

                // buy in and seat occupied
                int tries = 0;
                while (true)
                {
                    // tries
                    tries++;
                    if (tries > 25)
                    {
                        ErrorHandler.ReportExceptionWithImage(new Exception("cannot find buy-in or seat-occupied"), "buy in", screenshot);
                        return CANNOT_FIND_SEAT;
                    }

                    // wait
                    Thread.Sleep(250);
                    screenshot = screenIter.next();

                    // check 'buy in'
                    if (IsBuyInVisible(layout, screenshot))
                    {
                        // buy in
                        keyboard.pressEnter();
                        break;
                    }

                    // check for 'seat is occupied'                    
                    if (IsSeatOccupied(layout, screenshot))
                    {
                        Log.Info("seat is occupied");

                        // click ok
                        mouse.MoveAndLeftClick(offset.X + layout.Occupied.X, offset.Y + layout.Occupied.Y, 4, 4);
                        Thread.Sleep(RandomInt(500, 1000));

                        // close window
                        CloseLastTable(screenIter);
                        Thread.Sleep(RandomInt(1500, 2000));
                        return CANNOT_FIND_SEAT;
                    }
                }

                // move mouse near to checkboxes
                mouse.Move(offset.X + layout.SitOutClick.X + RandomInt(0, 15), offset.Y + layout.SitOutClick.Y - RandomInt(40, 60));
                
                // is "sit out next hand" visible?
                bool isFastTable = true;
                tries = 0;
                while (true)
                {
                    // tries
                    tries++;
                    if (tries > 25)
                    {
                        ErrorHandler.ReportExceptionWithImage(new Exception("cannot find checkboxes after buy-in"), "buy in", screenshot);
                        return CANNOT_FIND_SEAT;
                    }

                    // screen
                    screenshot = screenIter.next();

                    // check sit-out pattern
                    if (IsSitOutVisible(layout, screenshot))
                    {
                        break;
                    }

                    // sleep
                    Thread.Sleep(RandomInt(300, 700));
                }

                // wait
                Thread.Sleep(RandomInt(100, 200));
                screenshot = screenIter.next();

                // auto-post blinds
                if (IsAutoPostBlindsVisible(layout, screenshot))
                {
                    // click auto-post
                    controller.PressAutoBlind(layout);

                	// is slow table
                	isFastTable = false;

                    // move mouse to safe location
                    mouse.Move(offset.X + layout.SitOutClick.X + RandomInt(0,15), offset.Y + layout.SitOutClick.Y);
                }

                // double-check auto-post blinds
                screenshot = screenIter.next();
                if (IsAutoPostBlindsVisible(layout, screenshot))
                {
                    tries = 0;
                    while (true)
                    {
                        // tries
                        tries++;
                        if (tries > 3)
                        {
                            ErrorHandler.ReportExceptionWithImage(new Exception("cannot select auto-post blinds"), "blinds", screenshot);
                            Log.Info("Closing table");
                            CloseLastTable(screenIter);
                            Thread.Sleep(400);
                            keyboard.pressEnter();
                            return CANNOT_FIND_SEAT;
                        }

                        // screen
                        mouse.Move(offset.X + layout.SitOutClick.X + RandomInt(0, 15), offset.Y + layout.SitOutClick.Y); 
                        Thread.Sleep(400);
                        screenshot = screenIter.next();

                        // check
                        if (!IsAutoPostBlindsChecked(layout, screenshot))
                        {
                            // click auto-post
                            controller.PressAutoBlind(layout);

                            // is slow table
                            isFastTable = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // sit out
                Thread.Sleep(RandomInt(50, 300));                
                Controller.PressSitOut(mouse, layout);

                // check for "I'm back button" button
                tries = 0;
                while (true)
                {
                    // tries
                    tries++;
                    if (tries > 10)
                    {
                        // close window
                        ErrorHandler.ReportExceptionWithImage(new Exception("I'm back button not visible while opening table"), "Open table", tableImage);
                        Log.Info("Closing table");
                        CloseLastTable(screenIter);
                        Thread.Sleep(250);
                        keyboard.pressEnter();
                        Thread.Sleep(RandomInt(1500, 2000));
                        return CANNOT_FIND_SEAT;
                    }

                    // look for button
                    Thread.Sleep(500);
                    screenshot = screenIter.next();
                    tableImage = CropTable(screenshot, layout);
                    if (IsImBackVisible(layout, tableImage))
                    {
                        break;
                    }
                }

                // container
                Image cornerTopLeft = CropTableTopLeftCorner(screenshot, offset);
                Image cornerBottomRight = CropTableBottomRightCorner(screenshot, offset);
                TableContainer table = new TableContainer(globalTableNumber++, cornerTopLeft, cornerBottomRight, layout, openSeat.Num, isFastTable, lobby);
                return table;
            }
        }

        private static bool IsImBackVisible(TableLayout layout, Image table)
        {
            Image imBackImage = table.crop(layout.ImBack.X, layout.ImBack.X + layout.ImBack.Width,
                layout.ImBack.Y, layout.ImBack.Y + layout.ImBack.Height);
            return PatternLocator.isImBackVisible(imBackImage);
        }

        private static bool IsSitOutVisible(TableLayout layout, Image screenshot)
        {
            Image tableImage = CropTable(screenshot, layout);
            Image sitOutImage = tableImage.crop(layout.SitOutPattern.X, layout.SitOutPattern.X + layout.SitOutPattern.Width,
                layout.SitOutPattern.Y, layout.SitOutPattern.Y + layout.SitOutPattern.Height);
            return PatternLocator.isSitOutVisible(sitOutImage);
        }

        private static bool IsAutoPostBlindsChecked(TableLayout layout, Image screenshot)
        {
            Image tableImage = CropTable(screenshot, layout);
            Image blindsImage = tableImage.crop(layout.AutoBlindPattern.X, layout.AutoBlindPattern.X + layout.AutoBlindPattern.Width,
                layout.AutoBlindPattern.Y, layout.AutoBlindPattern.Y + layout.AutoBlindPattern.Height);
            return PatternLocator.isAutoPostBlindsChecked(blindsImage);
        }

        private static bool IsAutoPostBlindsVisible(TableLayout layout, Image screenshot)
        {
            Image tableImage = CropTable(screenshot, layout);
            Image blindsImage = tableImage.crop(layout.AutoBlindPattern.X, layout.AutoBlindPattern.X + layout.AutoBlindPattern.Width,
                layout.AutoBlindPattern.Y, layout.AutoBlindPattern.Y + layout.AutoBlindPattern.Height);
            return PatternLocator.locateAutoPostBlinds(blindsImage);
        }

        private static bool IsBuyInVisible(TableLayout layout, Image screen)
        {
            Point offset = layout.Offset;
            Image table = CropTable(screen, layout);
            Image buyIn = table.crop(layout.BuyInIcon.X, layout.BuyInIcon.X + layout.BuyInIcon.Width,
                layout.BuyInIcon.Y, layout.BuyInIcon.Y + layout.BuyInIcon.Height);
            return PatternLocator.isBuyInVisible(buyIn);
        }

        private bool IsSeatOccupied(TableLayout layout, Image screen)
        {
            Point offset = layout.Offset;
            Image table = CropTable(screen, layout);
            Image msgbox = table.crop(layout.MessageBox.X, layout.MessageBox.X + layout.MessageBox.Width,
                layout.MessageBox.Y, layout.MessageBox.Y + layout.MessageBox.Height);
            return PatternLocator.locateError(msgbox);
        }

        private bool YouHaveJustLeftThisTable(TableLayout layout, Image screen)
        {
            Point offset = layout.Offset;
            Image table = CropTable(screen, layout);
            Image msgbox = table.crop(layout.YouHaveJustLeftThisTable.X, layout.YouHaveJustLeftThisTable.X + layout.YouHaveJustLeftThisTable.Width,
                layout.YouHaveJustLeftThisTable.Y, layout.YouHaveJustLeftThisTable.Y + layout.YouHaveJustLeftThisTable.Height);
            return PatternLocator.locateError(msgbox);
        }

        private bool IsTableGood(LobbyTable table)
        {
            return !table.IsJoined && table.Players >= MIN_PLAYERS_ON_TABLE && table.PotSize >= MIN_AVG_POT_SIZE;
        }

        private bool HasOpenTables(List<LobbyTable> lobbyTables)
        {
            foreach (LobbyTable table in lobbyTables)
            {
                if (IsTableGood(table))
                {
                    return true;
                }
            }
            return false;
        }

        private LobbyTable SelectBestTable(List<LobbyTable> lobbyTables)
        {
            LobbyTable bestTable = null;
            foreach (LobbyTable table in lobbyTables)
            {
                if (IsTableGood(table))
                {
                    bestTable = table;
                    break;
                }
            }
            if (bestTable == null)
            {
                throw new Exception("cannot select best table");
            }
            foreach (LobbyTable table in lobbyTables)
            {
                if (IsTableGood(table))
                {
                    if (table.Score > bestTable.Score)
                    {
                        bestTable = table;
                    }
                }
            }
            return bestTable;
        }
    }
}
