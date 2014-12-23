using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.IO;

namespace PokerBot
{
    public class BotAppLogic
    {
        private class TableSwitcher
        {
            private Mouse mouse;
            private Iterator<Image> screen;
            private DeviceControl device;
            private int activeColor;
            private ColorReducer taskbarReducer;

            // variables from previous iteration
            private int prevIterationsNoActionCounter = 0;
            private int prevIterationContainer = -1;
            private bool prevIterationAction = false;

            // variables for this iteration
            private bool iterationAction = false;

            public TableSwitcher(Mouse mouse, DeviceControl device, List<Color> colors)
            {
                this.mouse = mouse;
                this.screen = new ScreenImageIterator(device);
                this.device = device;
                this.activeColor = colors[0].ToArgb();
                this.taskbarReducer = new ColorPaletteReducer(colors.ToArray());
            }

            public void action()
            {
                iterationAction = true;
            }

            public bool isSameContainer(int container)
            {
                // is same table as previous iteration
                bool isSameTableAsPrevIter = (container == prevIterationContainer);
                prevIterationContainer = container;
                return isSameTableAsPrevIter;
            }

            public bool hasPrevIterationAction()
            {
                return prevIterationAction;
            }

            public void check(bool enabled)
            {
                if (!iterationAction)
                {
                    // move mouse if five times in a row nothing happens
                    prevIterationsNoActionCounter++;
                    if (prevIterationsNoActionCounter > 8)
                    {
                        // check for highlight
                        Point taskbarActive = checkTaskbarActivity();
                        if (enabled && taskbarActive != Point.Empty)
                        {
                            // move mouse to taskbar
                            mouse.MoveAndLeftClick(taskbarActive.X, taskbarActive.Y, 4, 4);
                        }
                        else
                        {
                            // random move mouse
                            int off = RandomInt(-150, +150);
                            if (off < +5 && off > -5) off = 10;
                            mouse.Move(mouse.Position.X + off, mouse.Position.Y + off);
                        }

                        // reset
                        prevIterationsNoActionCounter = 0;
                    }
                }
                else
                {
                    prevIterationsNoActionCounter = 0;
                }

                // previous actions
                prevIterationAction = iterationAction;

                // reset
                iterationAction = false;
            }

            private Point checkTaskbarActivity()
            {
                int tries = 0;
                while(true)
                {
                    // tries
                    tries++;
                    if (tries > 2)
                    {
                        return Point.Empty;
                    }
                    // crop
                    Image screenshot = screen.next();
                    int xStart = 250;
                    int xEnd = device.DisplayWidth - 100;

                    // scan fist row
                    {
                        int yStart = device.DisplayHeight - 36;
                        int yEnd = yStart + 2;
                        Image taskbarLine = screenshot.crop(xStart, xEnd, yStart, yEnd);
                        taskbarLine = taskbarReducer.reduceColors(taskbarLine);
                        int xPos = locateColor(taskbarLine, activeColor);
                        if (xPos != -1)
                        {
                            // log
                            ErrorHandler.ReportExceptionWithImage(new Exception("taskbar active"), "window seems to be blocked", screenshot);
                            // location
                            return new Point(xStart + xPos + 30, yStart - 5);
                        }
                    }

                    // scan second row
                    {
                        int yStart = device.DisplayHeight - 5;
                        int yEnd = yStart + 2;
                        Image taskbarLine = screenshot.crop(xStart, xEnd, yStart, yEnd);
                        taskbarLine = taskbarReducer.reduceColors(taskbarLine);
                        int xPos = locateColor(taskbarLine, activeColor);
                        if (xPos != -1)
                        {
                            // log
                            ErrorHandler.ReportExceptionWithImage(new Exception("taskbar active"), "window seems to be blocked", screenshot);
                            // location
                            return new Point(xStart + xPos + 30, yStart - 5);
                        }
                    }
                    Thread.Sleep(100);
                }
            }

            private int locateColor(Image image, int color)
            {
                for (int y = 0; y < image.lines.Length; y++)
                {
                    for (int x = 0; x < image.lines[y].Length; x++)
                    {
                        if (image.lines[y][x] == color)
                        {
                            return x;
                        }
                    }
                }
                return -1;
            }
        }

        // # events
        public event ResetEvent ResetTablesEvent;
        public delegate void ResetEvent();

        public event SessionEvent StartSessionEvent;
        public delegate void SessionEvent();

        public event ContainersEvent ContainersChangedEvent;
        public delegate void ContainersEvent(int num);

        public event TableFoundDelegate TableFoundEvent;
        public delegate void TableFoundDelegate(TableContainer table);

        public event RemoveTableDelegate RemoveTableEvent;
        public delegate void RemoveTableDelegate(TableContainer table);

        public event MoneyEvent TotalMoneyChangedEvent;
        public delegate void MoneyEvent(double money);

        public event ReplaceMinsEvent ReplaceMinsLeftEvent;
        public delegate void ReplaceMinsEvent(int mins);

        // # static
        private static Random random = new Random();

        // # members
        private DeviceControl deviceControl;
        private Thread thread;
        private TableOpener tableOpener;
        private TableIdentifier tableIdentifier = new TableIdentifier(new TableLayout9());
        private AliveKeeper aliveKeeper;
        private Settings settings;
        private int rulesCount = RulesReader.readRules().Count;
        private bool isStarted = false;
        private bool waitForBlinds = false, sitOutNextHand = false;

        public BotAppLogic(DeviceControl deviceControl)
        {
            this.deviceControl = deviceControl;
            this.tableOpener = new TableOpener(deviceControl, 
                new HumanMouse(deviceControl, HumanMouse.Speed.Fast),
                new Keyboard(deviceControl), tableIdentifier);
        }

        public BotAppLogic(DeviceControl deviceControl, Settings settings)
        {
            this.settings = settings;
            this.deviceControl = deviceControl;
            this.tableOpener = new TableOpener(deviceControl,
                new HumanMouse(deviceControl, settings.FastMouse ? HumanMouse.Speed.Super : HumanMouse.Speed.Fast),
                new Keyboard(deviceControl), tableIdentifier);

            // alive keeper
            SetupAliveKeeper();
        }

        private void SetupAliveKeeper()
        {
            // alive keeper
            aliveKeeper = new AliveKeeper(settings);

            // number of active tables changed (-> UI)
            aliveKeeper.ContainersChangedEvent += delegate(int num)
            {
                if (ContainersChangedEvent != null)
                    ContainersChangedEvent(num);
            };

            // minimum tables reached
            aliveKeeper.OpenAdditionalTablesEvent += delegate(int num)
            {
                OpenAdditionalTables(num);
            };

            // max restart time reached (replace tables)
            aliveKeeper.CloseTablesEvent += delegate(int num)
            {
                WaitForBlinds();
            };

            // restart application (replace tables)
            aliveKeeper.ClosePokerApplicationNow += delegate(int nim)
            {
                Restart();
            };

            // replace time left
            aliveKeeper.ReplaceTableMinsLeft += delegate(int mins)
            {
                ReplaceMinsLeftEvent(mins);
            };
        }

        public int RulesCount
        {
            get { return rulesCount; }
        }

        public bool IsStarted
        {
            get { return isStarted; }
        }

        public DeviceControl DeviceControl
        {
            get { return deviceControl; }
        }

        public void ResetTables()
        {
            Log.Info("## reset tables ##");
            tableOpener.ResetTables();
        }

        public void SitIn()
        {
            Log.Info("## sit-in ##");
            tableOpener.SitIn();
        }

        public void SitOut()
        {
            tableOpener.SitOut();
        }

        public void ResetMouse()
        {
            deviceControl.ResetMouse();
        }

        public TableContainer LocateNewTable(Settings settings)
        {
            TableContainer foundTable = tableOpener.LocateNewTable(settings);

            // delegate (-> UI)
            if (TableFoundEvent != null)
                TableFoundEvent(foundTable);

            return foundTable;
        }

        public void StartPokerApplication(Settings settings)
        {
            // start
            tableOpener.StartPokerApplication(settings);
        }

        public void StopPokerApplication(Settings settings)
        {
            // stop
            tableOpener.StopPokerApplication(settings);
        }

        public List<TableContainer> OpenNewTables()
        {
            // open tables
            List<TableContainer> tables;
            if (TableFoundEvent != null)
            {
                TableOpener.TableFoundDelegate tableDelegate = delegate(TableContainer table) { TableFoundEvent(table); };
                tables = tableOpener.OpenNewTables(settings, tableDelegate);
            }
            else
            {
                tables = tableOpener.OpenNewTables(settings);
            }

            // log
            StreamWriter log = new StreamWriter(settings.Name + ".txt", true);
            log.WriteLine("## OpenNewTables (player, pot, flop, score) -> " + DateTime.Now + " ##");
            foreach (TableContainer table in tables)
            {
                log.WriteLine(table.LobbyTable.ToCsvString());
            }
            log.Close();

            return tables;
        }

        private void OpenAdditionalTables(int currentActiveTables)
        {
            ThreadStart concurrent = delegate()
            {
                try
                {
                    // sit-out
                    WaitForSitOuts();

                    // wait
                    Log.Info("waiting for sit-outs");
                    int waited = 0;
                    while (true)
                    {
                        // sleep
                        Thread.Sleep(10 * 1000);
                        waited += 10;

                        // done
                        if (waited > 180) break;

                        // all tables in sit-out
                        bool sitOutOnAllTables = true;
                        foreach (TableContainer table in tableOpener.KnownTables)
                        {
                            if (!table.IsSittingOut && !table.IsClosed && !table.IsTimedOut)
                            {
                                sitOutOnAllTables = false;
                            }
                        }
                        if (sitOutOnAllTables)
                        {
                            Thread.Sleep(60 * 1000);
                            break;
                        }
                    }

                    // stop main thread
                    Stop();

                    // check table status
                    Log.Info("checking all tables for sit-out");
                    foreach (TableContainer table in tableOpener.KnownTables)
                    {
                        if (table.IsClosed || table.IsSittingOut)
                        {
                            Log.Info("table " + table.Number + " is fine");
                        }
                        else if (table.IsTimedOut)
                        {
                            Log.Error("table " + table.Number + " timed out");
                        }
                        else
                        {
                            Log.Error("table " + table.Number + " seems to be still active or dead");
                        }
                    }

                    // remove old tables
                    List<TableContainer> toBeRemoved = new List<TableContainer>();
                    foreach (TableContainer table in tableOpener.KnownTables)
                    {
                        if (!table.IsSittingOut) toBeRemoved.Add(table);
                    }
                    foreach (TableContainer table in toBeRemoved)
                    {
                        tableOpener.KnownTables.Remove(table);
                        RemoveTableEvent(table);
                    }

                    // minimize additional tables
                    tableOpener.MinimizeAllTables();

                    // open additional tables
                    TextToSpeech.SayAsnc(settings, "opening additional tables");
                    OpenNewTables();

                    // sit-in
                    SitIn();

                    // start
                    Start(false);
                }
                catch (Exception ex)
                {
                    ErrorHandler.ReportException(ex, "error while opening additional tables");
                }
            };

            new Thread(concurrent).Start();
        }

        public void WaitForBlinds()
        {
            waitForBlinds = true;
        }

        public void WaitForSitOuts()
        {
            sitOutNextHand = true;
        }

        public void Start(bool resetMouse)
        {
            Monitor.Enter(this);
            {
                Log.Info("## start ##");

                // sanity check
                if (isStarted)
                {
                    throw new Exception("bot logic is already started");
                }

                // alive keeper
                aliveKeeper.Start(tableOpener.KnownTables);

                // main thread
                thread = new Thread(delegate() { ProcessTables(settings, resetMouse); });
                thread.Start();
                isStarted = true;

                // event
                StartSessionEvent();
            }
            Monitor.Exit(this);
        }

        private void Restart()
        {
            Monitor.Enter(this);
            {
                try
                {
                    // sanity check
                    if (!isStarted)
                    {
                        Log.Warn("app logic stopped - ignoring restart");
                        return;
                    }

                    // log
                    Log.Info("restarting poker application");

                    // stop thread
                    Stop();

                    // kill application
                    tableOpener.StopPokerApplication(settings);

                    // remove tables
                    ResetTables();

                    // invoke delegate (-> UI)
                    if (ResetTablesEvent != null)
                        ResetTablesEvent();

                    // start
                    Thread.Sleep(5000);
                    tableOpener.StartPokerApplication(settings);

                    // open tables
                    Thread.Sleep(5000);
                    OpenNewTables();

                    // sit-in
                    SitIn();

                    // start
                    Start(false);
                }
                catch (Exception ex)
                {
                    ErrorHandler.ReportException(ex, "error while restarting");
                }
            }
            Monitor.Exit(this);
        }

        public void Stop()
        {
            Monitor.Enter(this);
            {
                Log.Info("## stop ##");
                // not yet started
                isStarted = false;

                // stop
                aliveKeeper.Stop();
                thread.Abort();

                // reset wait for blinds and sit-out
                waitForBlinds = false;
                sitOutNextHand = false;
            }
            Monitor.Exit(this);
        }

        private void ProcessTables(Settings settings, bool resetMouse)
        {
            // screen
            Iterator<Image> screen = new ScreenImageIteratorFast(deviceControl);

            // evaluator
            List<Rule> rules = RulesReader.readRules();
            RuleEvaluator evaluator = new RuleEvaluator(rules);
            RuleInterpreter interpreter = new RuleInterpreter(settings.SmallBlind, settings.BigBlind);
            if (settings.PreCheckRules)
            {
                DateTime startCheck = DateTime.Now;
                interpreter.precheck(rules);
                Log.Info("# prechecking rules took " + DateTime.Now.Subtract(startCheck).TotalMilliseconds + " ms");
            }

            // controller
            double betSlideTextLimit = settings.PlayMoney ? 100 : 0.2;
            Mouse mouse = new HumanMouse(deviceControl, settings.FastMouse ? HumanMouse.Speed.Fast : HumanMouse.Speed.Normal);
            Keyboard keyboard = new Keyboard(deviceControl);
            Controller controller = new Controller(keyboard, mouse, betSlideTextLimit, tableIdentifier, new ScreenImageIterator(deviceControl));

            // clicker
            RandomClicker clicker = new RandomClicker(new Point(deviceControl.DisplayWidth, 0), mouse);

            // replayer
            Replayer replayer = new Replayer(deviceControl);

            // table switcher
            TableSwitcher switcher = new TableSwitcher(mouse, deviceControl, settings.TaskbarColors);

            // reset mouse
            if(resetMouse) deviceControl.ResetMouse();

            // loop
            while (screen.hasNext())
            {
                // start
                Log.Debug("## iteration -> start ##");
                DateTime start = DateTime.Now;

                // screenshot
                DateTime startScreen = DateTime.Now;
                Image unsafeScreenshot = screen.next();
                Log.Debug("screenshot took " + DateTime.Now.Subtract(startScreen).TotalMilliseconds + " ms");

                // tables
                foreach (TableContainer container in tableOpener.KnownTables)
                {
                    // table image				
                    Log.Debug("# processing table " + (container.Number + 1));
                    Image unsafeTableImage = TableOpener.CropTable(unsafeScreenshot, container.Layout);

                    // render table
                    if (container.Renderer != null)
                    {
                        DateTime startRender = DateTime.Now;
                        container.Renderer.clearImages();
                        container.Renderer.renderImage(unsafeTableImage, 0, 0);
                        tableIdentifier.Renderer = container.Renderer;
                        Log.Debug("rendering took " + DateTime.Now.Subtract(startRender).TotalMilliseconds + " ms");
                    }

                    // table status
                    bool isTableVisible = tableOpener.IsTableVisible(container, unsafeTableImage);
                    bool areControlsActive = isTableVisible ? tableIdentifier.identifyMove(unsafeTableImage) : false;
                    Log.Debug("# table" + container.Number + ": vis=" + isTableVisible + " buttons=" + areControlsActive);

                    // HANDLE ERRORS: table window should not be visible, but it is
                    if (isTableVisible)
                    {
                        // "you have been removed"
                        if (!areControlsActive && YouHaveBeenRemovedVisible(container.Layout, unsafeTableImage))
                        {
                            // close it
                            switcher.action();
                            ErrorHandler.ReportExceptionWithImage(new Exception("You have been removed"), "handle errors", unsafeTableImage);
                            keyboard.pressEnter();
                            controller.CloseTableWithEnter(container.Layout);
                            container.Close();
                            // wait, otherwise table is visible on next screenshot
                            Thread.Sleep(500);
                            break;
                        }

                        // "I'm back"
                        if (!sitOutNextHand && !areControlsActive && !container.IsWaitingForBlind && IsImBackVisible(container.Layout, unsafeTableImage))
                        {
                            // close it
                            switcher.action();
                            ErrorHandler.ReportExceptionWithImage(new Exception("I'm back visible"), "handle errors", unsafeTableImage);
                            controller.CloseTableWithEnter(container.Layout);
                            container.Close();
                            // wait, otherwise table is visible on next screenshot
                            Thread.Sleep(500);
                            break;
                        }

                        // "Post Blind"
                        if (!container.IsWaitingForBlind && areControlsActive && IsPostBlindVisible(container.Layout, unsafeTableImage))
                        {
                            // close it
                            switcher.action();
                            ErrorHandler.ReportExceptionWithImage(new Exception("Post Blind visible"), "handle errors", unsafeTableImage);
                            controller.CloseTableWithEnter(container.Layout);
                            container.Close();
                            // wait, otherwise table is visible on next screenshot
                            Thread.Sleep(500);
                            break;
                        }

                        // "Check or Fold"
                        if (areControlsActive && IsCheckOrFoldMsgBoxVisible(container.Layout, unsafeTableImage))
                        {
                            // press check
                            switcher.action();
                            ErrorHandler.ReportExceptionWithImage(new Exception("Check or Fold visible"), "handle errors", unsafeTableImage);
                            keyboard.pressEnter();
                            // wait
                            Thread.Sleep(500);
                            break;
                        }
                    }

                    // HANDLE TABLE: processing table and handling controls (if the corner image matches)
                    if (isTableVisible && (areControlsActive || container.IsWaitingForBlind))
                    {
                        // move mouse if required
                        if (switcher.isSameContainer(container.Number) && !switcher.hasPrevIterationAction() && !areControlsActive)
                        {
                            // same table and nothing happend since previous iteration
                            Sleep(settings, RandomInt(300, 500));
                        }
                        else
                        {
                            // move mouse to safe location (screenshot)
                            MoveMouseToSafePosition(controller.Mouse, container.Layout);
                            Sleep(settings, RandomInt(50, 100));
                        }

                        // take clean screenshot
                        Image safeScreenshot = screen.next();
                        Image safeTableImage = TableOpener.CropTable(safeScreenshot, container.Layout);
                        if (container.Renderer != null)
                        {
                            container.Renderer.clearImages();
                            container.Renderer.renderImage(safeTableImage, 0, 0);
                        }

                        // CLOSE TABLE: check if table has to be closed                    
                        if (container.IsWaitingForBlind)
                        {
                            Log.Debug("table " + (container.Number + 1) + " checking for big blind indication");
                            
                            // detect
                            bool fastTableToClose = container.IsFastTable && IsImBackVisible(container.Layout, safeTableImage);
                            bool slowTableToClose = container.IsSlowTable && IsPostBlindVisible(container.Layout, safeTableImage);

                            // close it
                            if (fastTableToClose || slowTableToClose)
                            {
                                // click on close and press enter
                                CloseTable(container, controller);

                                // wait, otherwise table is visible on next screenshot
                                Thread.Sleep(500);

                                // done with this table
                                break;
                            }
                        }

                        // HANDLE CONTROLS: identify controls, table, press butto´n
                        if (areControlsActive)
                        {
                            Log.Info("table " + (container.Number + 1) + " handling controls");

                            // active container (-> UI)
                            container.Active();

                            // identify controls
                            List<TableControl> controls = new List<TableControl>();
                            try
                            {
                                controls = tableIdentifier.identifyControls(safeTableImage);
                            }
                            catch (Exception ex)
                            {
                                // error beep
                                ErrorHandler.BeepError();

                                // press fold
                                PressFold(container.Layout, controller);

                                // report
                                ErrorHandler.ReportTableException(ex, safeTableImage, "Identify controls");

                                continue;
                            }

                            // process table actions
                            try
                            {
                                // identify table
                                DateTime startIdentify = DateTime.Now;
                                Log.Debug("table " + (container.Number + 1) + " identifying");
                                Table table = tableIdentifier.identifyTable(safeTableImage, container.Seat);
                                Log.Debug("identify took " + DateTime.Now.Subtract(startIdentify).TotalMilliseconds + " ms");

                                // remember money
                                if(table.HasSeat && table.MyPlayer.HasMoney) container.Money = table.MyPlayer.Money;

                                // fire money changed event
                                MoneyChanged();

                                // identify max bet
                                table.MaxBet = GetMaxBet(controls);
                                Log.Debug("max bet on table " + (container.Number + 1) + " is " + table.MaxBet);

                                // press buttons
                                Log.Debug("table " + (container.Number + 1) + " pressing buttons");
                                ProcessTable(settings, safeTableImage, container.Renderer, table, container, evaluator, interpreter,
                                    controller, replayer, clicker, controls);

                                // done something
                                switcher.action();
                            }
                            catch (Exception ex)
                            {
                                // done something
                                switcher.action();

                                // error beep
                                ErrorHandler.BeepError();

                                // press fold
                                PressCheckOrFold(controls, container.Layout, controller);

                                // move mouse to taskbar
                                MoveMouseToTaskBar(controller.Mouse);

                                // report
                                ErrorHandler.ReportTableException(ex, safeTableImage, "Identify table");

                                continue;
                            }
                        }
                    }
                }

                // check table highlights
                switcher.check(settings.WindowSwitcherActivated);

                // sleep
                Thread.Sleep(RandomInt(200, 300));

                // end
                double time = DateTime.Now.Subtract(start).TotalMilliseconds;
                Log.Debug("## iteration -> end -> " + time + " ms ##");
            }
        }

        public void ProcessTable(Settings settings,
            Image tableImage,
            TableRenderer renderer,
            Table table,
            TableContainer container,
            RuleEvaluator evaluator,
            RuleInterpreter interpreter,
            Controller controller,
            Replayer replayer,
            RandomClicker clicker,
            List<TableControl> controls)
        {
            // hand visible?
            if (table.Hand.Count != 0)
            {
                // identify situation
                Situation situation = SituationEvaluator.evaluateSituation(tableImage, table, controls, settings.BigBlind);

                // wait for blinds activatd or not enough players or money reached
                if (!container.IsWaitingForBlind && situation.Street == StreetTypes.Preflop)
                {
                    // override blinds?
                    if (waitForBlinds)
                    {
                        Log.Info("table " + (container.Number + 1) + " override blinds");
                        WaitForBlind(controller, container);
                    }
                    // out of players?
                    else if (table.IsOutOfPlayers)
                    {
                        Log.Info("table " + (container.Number + 1) + " ran out of players");
                        CloseTable(container, controller);
                        return;
                    }
                    // reached money?
                    else if (settings.CloseTableActivated && table.HasSeat && table.MyPlayer.HasMoney)
                    {
                        if (table.MyPlayer.Money > settings.CloseTableMoneyMax)
                        {
                            Log.Info("table " + (container.Number + 1) + " reached money limit " + table.MyPlayer.Money);
                            CloseTable(container, controller);
                            return;
                        }
                    }
                }

                // evaluate rule
                Rule rule = evaluator.findRule(situation.Street,
                                               situation.Hand,
                                               situation.Chance,
                                               situation.Opponents,
                                               situation.OpponentAction,
                                               situation.Position,
                                               table.MaxBet,
                                               table.Pot);
                // decision
                Decision decision = interpreter.interpret(table, rule.Decision);

                // render table
                Log.Debug("# rendering table");
                if (renderer != null)
                {
                    renderer.render(table, container.Layout, situation, rule, decision, controls);
                }

                // beep
                if (decision.DecisionType == Decision.Types.BET 
                    || decision.DecisionType == Decision.Types.CALL
                    || decision.DecisionType == Decision.Types.RAISE)
                {
                    if (situation.Hand != HandTypes.None)
                    {
                        Beep(settings);
                        TextToSpeech.SayAsnc(settings, situation.Hand);
                    }
                }

                // random click on table
                if (RandomBool(0.05) && settings.AutoClick)
                {
                    Log.Debug("# auto click on table");
                    clicker.click(container.Layout);
                }

                // sit out
                if (!container.IsSittingOut && sitOutNextHand)
                {
                    SitOut(controller, container);
                }

                // press buttons
                Log.Debug("# press controls");
                Thread.Sleep(RandomInt(100, 500));
                controller.Handle(decision, container.Layout, controls);

                // random mouse moves
                if (RandomBool(0.05) && settings.AutoMoveMouse)
                {
                    Log.Debug("# auto move mouse");
                    Sleep(settings, RandomInt(100, 300));
                    AutoMoveMouse(controller.Mouse);
                }
                if (situation.Opponents > 2 && RandomBool(0.05) && settings.ReplayMouseMoves)
                {
                    Log.Debug("# replay mouse");
                    Sleep(settings, RandomInt(100, 300));
                    replayer.ReplayRandomShort();
                }
            }
            else
            {
                // render table
                Log.Debug("# rendering table");
                renderer.render(table, container.Layout);
            }
        }

        private void CloseTable(TableContainer container, Controller controller)
        {
            Log.Info("closing " + (container.IsFastTable ? "fast" : "slow") + " table " + (container.Number + 1));
            controller.CloseTableWithEnter(container.Layout);
            container.Close();
        }

        private void MoneyChanged()
        {
            double money = 0;
            foreach (TableContainer container in tableOpener.KnownTables)
            {
                money += container.Money;
            }
            TotalMoneyChangedEvent(money);
        }

        private void WaitForBlind(Controller controller, TableContainer container)
        {
            if (container.IsFastTable)
            {
                controller.PressWaitForBlind(container.Layout);
            }
            else
            {
                controller.PressAutoBlind(container.Layout);
            }
            container.WaitForBlindToClose();
        }

        private void SitOut(Controller controller, TableContainer container)
        {
            controller.PressSitOut(container.Layout);
            container.SitOut();
        }

        public static double GetMaxBet(List<TableControl> controls)
        {
            foreach (TableControl control in controls)
            {
                if (control.Type == TableControl.ControlType.CALL)
                {
                    return control.Amount;
                }
            }
            return 0;
        }

        private static bool YouHaveBeenRemovedVisible(TableLayout layout, Image table)
        {
            Point offset = layout.Offset;
            Image msgbox = table.crop(layout.YouHaveBeenRemoved.X, layout.YouHaveBeenRemoved.X + layout.YouHaveBeenRemoved.Width,
                layout.YouHaveBeenRemoved.Y, layout.YouHaveBeenRemoved.Y + layout.YouHaveBeenRemoved.Height);
            return PatternLocator.locateError(msgbox);
        }

        private static bool IsImBackVisible(TableLayout layout, Image table)
        {
            Image imBackImage = table.crop(layout.ImBack.X, layout.ImBack.X + layout.ImBack.Width,
                layout.ImBack.Y, layout.ImBack.Y + layout.ImBack.Height);
            return PatternLocator.isImBackVisible(imBackImage);
        }
        
        private static bool IsPostBlindVisible(TableLayout layout, Image table)
        {
            Image postBlindImage = table.crop(layout.PostBlind.X, layout.PostBlind.X + layout.PostBlind.Width,
                layout.PostBlind.Y, layout.PostBlind.Y + layout.PostBlind.Height);
            return PatternLocator.isPostBlindVisible(postBlindImage);
        }

        private static bool IsCheckOrFoldMsgBoxVisible(TableLayout layout, Image table)
        {
            Image msgboxImage = table.crop(layout.CheckOrFold.X, layout.CheckOrFold.X + layout.CheckOrFold.Width,
                layout.CheckOrFold.Y, layout.CheckOrFold.Y + layout.CheckOrFold.Height);
            return PatternLocator.isCheckOrFoldMsgBoxVisible(msgboxImage);
        }

        public void MoveMouseToSafePosition(Mouse mouse, TableLayout layout)
        {
            int count = layout.SafeMousePositions.Length;
            Rectangle rect = layout.SafeMousePositions[random.Next() % count];
            int plusX = (int)(rect.Width * random.NextDouble());
            int plusY = (int)(rect.Height * random.NextDouble());
            mouse.Move(layout.Offset.X + rect.X + plusX, layout.Offset.Y + rect.Y + plusY);
        }

        public void MoveMouseToTaskBar(Mouse mouse)
        {
            mouse.Move((int)(100 + random.NextDouble() * 500), deviceControl.DisplayHeight - (int)(random.NextDouble() * 15));
        }

        public static int RandomInt(int begin, int end)
        {
            return (int)(random.NextDouble() * Math.Abs(end - begin)) + begin;
        }

        public static bool RandomBool()
        {
            return random.NextDouble() < .4;
        }

        public static bool RandomBool(double limit)
        {
            return random.NextDouble() < limit;
        }

        public static Dictionary<string, string> ReadConfig()
        {
            StreamReader reader = File.OpenText("config");
            Dictionary<string, string> config = new Dictionary<string, string>();
            string input = null;
            while ((input = reader.ReadLine()) != null)
            {
                string key = input.Split('=')[0].Trim();
                string value = input.Split('=')[1].Trim();
                config.Add(key, value);
            }
            reader.Close();
            return config;
        }

        public static int IdentifySeat(Table table, string name)
        {
            foreach (Player player in table.Players)
            {
                if (player.Name == name)
                    return player.Position;
            }
            return -1;
        }

        public static void Sleep(Settings settings, int ms)
        {
            if (settings.Sleep) Thread.Sleep(ms);
        }

        public static void Beep(Settings settings)
        {
            if (settings.Beep) Console.Beep(300, 100);
        }

        public static void AutoMoveMouse(Mouse mouse)
        {
            Point position = mouse.Position;
            int moveX = RandomInt(-400, +100);
            int moveY = RandomInt(-400, +150);
            Point target = new Point(position.X + moveX, position.Y + moveY);
            mouse.Move(target.X, target.Y);
        }

        public static void PressFold(TableLayout layout, Controller controller)
        {
            // move
            Log.Info("# press fold button");
            controller.PressFold(layout);
        }

        public static void PressCheckOrFold(List<TableControl> controls, TableLayout layout, Controller controller)
        {
            // move
            Log.Info("# press check or fold button");
            controller.PressCheckOrFold(layout, controls);
        }
    }
}
