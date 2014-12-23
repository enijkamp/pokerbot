using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;

namespace PokerBot.App.Gui
{
    public partial class BotForm : Form
    {
        private class RendererTab : TabPage
        {
            private TableContainer container;

            public RendererTab(TableContainer container, TableRendererControl control)
            {
                this.container = container;
                Location = new System.Drawing.Point(4, 22);
                Name = "tabPage" + container.Number;
                Padding = new System.Windows.Forms.Padding(3);
                Size = new System.Drawing.Size(827, 674);
                TabIndex = container.Number;
                ResetTitle();
                Controls.Add(control);
            }

            public TableContainer TableContainer
            {
                get { return container; }
            }

            private delegate void VoidDelegate();

            public void Highlight()
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new VoidDelegate(Highlight), new object[] { });
                }
                else
                {
                    (Parent as TabControl).SelectedTab = this;
                }
            }

            private delegate void TitleDelegate(string text);

            public void SetTitle(string text)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new TitleDelegate(SetTitle), new object[] { text });
                }
                else
                {
                    this.Text = text;
                }
            }

            public void ResetTitle()
            {
                SetTitle((container.IsFastTable ? "Fast" : "Slow") + (container.Number + 1));
            }
        }

        private BotAppLogic app;
        private bool isLoaded = false;
        private Settings settings = new Settings();
        private List<RendererTab> tabPages = new List<RendererTab>();
        private Dictionary<string, string> config = BotAppLogic.ReadConfig();
        private long runtimeSecs = 0;
        private bool autoStopOccured = false;
        private Thread autoStartThread;

        public BotForm()
        {            
            InitializeComponent();
        }

        private void BotForm_Load(object sender, EventArgs e)
        {
            // login
            if (!Security.CheckLogin(config["user"], config["password"]))
            {
                MessageBox.Show("incorrect login");
                Application.Exit();
            }

            // tabs
            BotForm_Resize(sender, e);

            // caption
            this.Text = "PokerBot Version " + Security.VERSION;

            // combo
            this.comboBoxLog.SelectedIndex = (int) Log.loglevel;
            this.comboBoxTables.SelectedIndex = settings.AutoLocateTablesNum - 1;
            this.comboBoxMin.SelectedIndex = settings.MinTablesNum - 1;
            this.comboBoxStakes.Text = config["stakes"];

            // text
            this.textBoxAutoStop.Text = settings.AutoStopMins.ToString();
            this.textBoxReplace.Text = settings.MaxTime.ToString();

            // config
            this.textBoxVM.Text = config["vm"];
            this.textBoxPlayer.Text = config["name"];
            this.textBoxCloseMax.Text = config["closeIfReachedMoneyMax"];
            this.textBoxCloseMin.Text = config["closeIfReachedMoneyMin"];

            // apply settings
            this.checkBoxPlay.Checked = settings.PlayMoney;
            this.checkBoxReplay.Checked = settings.ReplayMouseMoves;
            this.checkBoxSleep.Checked = settings.Sleep;
            this.checkBoxBeep.Checked = settings.Beep;
            this.checkBoxClick.Checked = settings.AutoClick;
            this.checkBoxMove.Checked = settings.AutoMoveMouse;
            this.checkBoxRules.Checked = settings.PreCheckRules;
            this.checkBoxSpeech.Checked = settings.Speech;
            this.checkBoxMinTables.Checked = settings.MinTablesActivated;
            this.checkBoxTime.Checked = settings.MinTablesActivated;
            this.checkBoxClose.Checked = settings.CloseTableActivated;
            this.checkBoxWindowSwitcher.Checked = settings.WindowSwitcherActivated;
            this.checkBoxAutoStop.Checked = settings.AutoStopActivated;
            this.checkBoxTableTabs.Checked = settings.TableTabs;
            this.checkBoxFastMouse.Checked = settings.FastMouse;

            // update settings
            settings.TaskbarColors = getTaskbarColors();
            settings.StakesByString(config["stakes"]);
            settings.CloseTableMoneyMin = Double.Parse(config["closeIfReachedMoneyMin"]);
            settings.CloseTableMoneyMax = Double.Parse(config["closeIfReachedMoneyMax"]);
        }

        private List<Color> getTaskbarColors()
        {
            string[] colorStrings = config["taskbarColors"].Split(',');
            List<Color> colors = new List<Color>();
            foreach (string color in colorStrings)
            {
                colors.Add(Color.FromName(color));
            }
            return colors;
        }

        private void BotForm_Resize(object sender, EventArgs e)
        {
            this.tabMain.Width = this.Width - 16;
        }

        private void BotForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (app != null && app.IsStarted) app.Stop();
        }

        private void checkBoxAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoStart.Checked)
            {
                timerAutoStart.Start();
            }
            else
            {
                timerAutoStart.Stop();
            }
        }

        private void checkBoxPlay_CheckedChanged(object sender, EventArgs e)
        {
            settings.PlayMoney = checkBoxPlay.Checked;
        }

        private void checkBoxMove_CheckedChanged(object sender, EventArgs e)
        {
            settings.AutoMoveMouse = checkBoxMove.Checked;
        }

        private void checkBoxClick_CheckedChanged(object sender, EventArgs e)
        {
            settings.AutoClick = checkBoxClick.Checked;
        }

        private void checkBoxReplay_CheckedChanged(object sender, EventArgs e)
        {
            settings.ReplayMouseMoves = checkBoxReplay.Checked;
        }

        private void checkBoxSleep_CheckedChanged(object sender, EventArgs e)
        {
            settings.Sleep = checkBoxSleep.Checked;
        }

        private void checkBoxBeep_CheckedChanged(object sender, EventArgs e)
        {
            settings.Beep = checkBoxBeep.Checked;
        }

        private void textBoxPlayer_TextChanged(object sender, EventArgs e)
        {
            settings.Name = textBoxPlayer.Text;
        }

        private void checkBoxSpeech_CheckedChanged(object sender, EventArgs e)
        {
            settings.Speech = checkBoxSpeech.Checked;
        }

        private void checkBoxFastMouse_CheckedChanged(object sender, EventArgs e)
        {
            settings.FastMouse = checkBoxFastMouse.Checked;
        }
        
        private void checkBoxCloseTop_CheckedChanged(object sender, EventArgs e)
        {
            settings.WindowSwitcherActivated = checkBoxWindowSwitcher.Checked;
        }

        private void checkBoxClose_CheckedChanged(object sender, EventArgs e)
        {
            settings.CloseTableActivated = checkBoxClose.Checked;
        }

        private void textBoxClose_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.CloseTableMoneyMax = TextTools.ParseDouble(textBoxCloseMax.Text);
            }
            catch (Exception)
            {
            }
        }

        private void textBoxCloseMin_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.CloseTableMoneyMin = TextTools.ParseDouble(textBoxCloseMin.Text);
            }
            catch (Exception)
            {
            }
        }

        private void checkBoxRules_CheckedChanged(object sender, EventArgs e)
        {
            settings.PreCheckRules = checkBoxRules.Checked;
        }

        private void checkBoxMinTables_CheckedChanged(object sender, EventArgs e)
        {
            settings.MinTablesActivated = checkBoxMinTables.Checked;
        }

        private void comboBoxStakes_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.StakesByString(comboBoxStakes.Text);
        }

        private void comboBoxLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLog.Text == "Fine")
                Log.SetLevel(Log.Level.FINE);
            else if (comboBoxLog.Text == "Debug")
                Log.SetLevel(Log.Level.DEBUG);
            else if (comboBoxLog.Text == "Info")
                Log.SetLevel(Log.Level.INFO);
            else if (comboBoxLog.Text == "Warn")
                Log.SetLevel(Log.Level.WARN);
            else if (comboBoxLog.Text == "Error")
                Log.SetLevel(Log.Level.ERROR);
        }

        private void comboBoxMin_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.MinTablesNum = int.Parse(comboBoxMin.Text);
        }

        private void comboBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.AutoLocateTablesNum = int.Parse(comboBoxTables.Text);
        }

        private void checkBoxTime_CheckedChanged(object sender, EventArgs e)
        {
            settings.MaxTimeActived = checkBoxTime.Checked;
        }

        private void checkBoxAutoStop_CheckedChanged(object sender, EventArgs e)
        {
            settings.AutoStopActivated = checkBoxAutoStop.Checked;
        }

        private void checkBoxTableTabs_CheckedChanged(object sender, EventArgs e)
        {
            settings.TableTabs = checkBoxTableTabs.Checked;
        }


        private void textReplace_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.MaxTime = int.Parse(this.textBoxReplace.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxAutoStop_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.AutoStopMins = int.Parse(textBoxAutoStop.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error auto-stop", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDevice_Click(object sender, EventArgs e)
        {
            loadDevice();
        }

        private void loadDevice()
        {
            try
            {
                if (!isLoaded)
                {
                    // logic
                    int vmWidth = int.Parse(config["vmWidth"]);
                    int vmHeight = int.Parse(config["vmHeight"]);
                    app = new BotAppLogic(new VirtualBoxControl(this.textBoxVM.Text, vmWidth, vmHeight), settings);
                    // callbacks
                    app.TableFoundEvent += delegate(TableContainer table)
                    {
                        addTableToUI(table);
                    };
                    app.RemoveTableEvent += delegate(TableContainer table)
                    {
                        removeTableFromUI(table);
                    };
                    app.ContainersChangedEvent += delegate(int tables)
                    {
                        setTableLabelText(tables);
                    };
                    app.ResetTablesEvent += delegate()
                    {
                        resetTablesUI();
                    };
                    app.StartSessionEvent += delegate()
                    {
                        randomReplaceTime();
                    };
                    app.TotalMoneyChangedEvent += delegate(double totalMoney)
                    {
                        setMoneyLabelText(totalMoney);
                    };
                    app.ReplaceMinsLeftEvent += delegate(int mins)
                    {
                        setReplaceMinsLabelText(mins);
                    };
                    // ui
                    buttonDevice.Text = "Unload Device";
                    isLoaded = true;
                    buttonScan.Enabled = true;
                    buttonAuto.Enabled = true;
                    buttonAutoStart.Enabled = true;
                    // log
                    TextToSpeech.SayAsnc(settings, "Virtual machine started");
                }
                else
                {
                    app.DeviceControl.Suspend();
                    buttonDevice.Text = "Load Device";
                    isLoaded = false;
                    buttonScan.Enabled = false;
                    buttonAuto.Enabled = false;
                    buttonAutoStart.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "device");
                MessageBox.Show(ex.Message, "Error load device", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                // sanity check
                sanityCheck();

                startLogic(true);
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "start");
                MessageBox.Show(ex.Message, "Error Start", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private delegate void StartLogicDelegate(bool resetMouse);
        private void startLogic(bool resetMouse)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StartLogicDelegate(startLogic), new object[] { resetMouse });
            }
            else
            {
                // auto stop
                autoStopOccured = false;

                // replace time
                randomReplaceTime();

                // stop time
                randomAutoStopTime();

                // runtime timer
                timerRuntime.Start();

                // logic
                app.Start(resetMouse);

                // log
                Security.StartSession(config["user"], settings.Name, settings.AutoLocateTablesNum, app.RulesCount);

                // ui
                buttonStart.Enabled = false;
                buttonStartBottom.Enabled = false;
                buttonStop.Enabled = true;
                buttonStopButtom.Enabled = true;

                // log
                TextToSpeech.SayAsnc(settings, "Bot started");
            }
        }

        private delegate void SetLabelMoneyDelegate(double money);
        private void setMoneyLabelText(double money)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetLabelMoneyDelegate(setMoneyLabelText), new object[] { money });
            }
            else
            {
                this.labelMoney.Text = money.ToString() + "$";
            }
        }

        private delegate void SetLabelTextDelegate(int number);
        private void setTableLabelText(int tables)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetLabelTextDelegate(setTableLabelText), new object[] { tables });
            }
            else
            {
                this.labelTables.Text = tables.ToString();
            }
        }

        private delegate void SetReplaceMinsLabelTextDelegate(int mins);
        private void setReplaceMinsLabelText(int mins)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetReplaceMinsLabelTextDelegate(setReplaceMinsLabelText), new object[] { mins });
            }
            else
            {
                this.labelReplaceLeft.Text = mins.ToString();
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
                stopLogic();
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "stop");
                MessageBox.Show(ex.Message, "Error Stop", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopLogic()
        {
            timerRuntime.Stop();
            app.Stop();
            buttonStart.Enabled = true;
            buttonStartBottom.Enabled = true;
            buttonStop.Enabled = false;
            buttonStopButtom.Enabled = false;
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            try
            {
                // table (-> table container added by delegate)
                TableContainer foundTable = app.LocateNewTable(settings);

                // log
                MessageBox.Show("found " +(foundTable.IsFastTable ? "fast" : "slow")+ " table at [" + foundTable.Layout.Offset.X + "," + foundTable.Layout.Offset.Y + "]", "table found");
  
                // flag
                buttonStart.Enabled = true;
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "table scan");
                MessageBox.Show(ex.Message, "Error Table Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAuto_Click(object sender, EventArgs e)
        {
            try
            {
                autoLocateTables(true);
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "table scan");
                MessageBox.Show(ex.Message, "Error Table Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sanityCheck()
        {
            // min tables < tables
            if (settings.MinTablesActivated && settings.MinTablesNum >= settings.AutoLocateTablesNum)
            {
                throw new ArgumentException("minimum tables >= auto-locate tables");
            }
        }

        private void sitIn()
        {
            TextToSpeech.SayAsnc(settings, "Sit in");
            app.SitIn();
        }        

        private void autoLocateTables(bool resetMouse)
        {
            TextToSpeech.SayAsnc(settings, "Locating tables");

            // update UI
            autoLocateTablesUI();

            // table
            if(resetMouse) app.ResetMouse();

            // open tables (-> table container added by delegate)
            app.OpenNewTables();
        }

        private delegate void AutoLocateTablesDelegate();

        private void autoLocateTablesUI()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AutoLocateTablesDelegate(autoLocateTablesUI), new object[] { });
            }
            else
            {
                // flag
                buttonStart.Enabled = true;
                buttonStartBottom.Enabled = true;
            }
        }

        private delegate void AddTableToUIDelegate(TableContainer container);
        private void addTableToUI(TableContainer foundTable)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AddTableToUIDelegate(addTableToUI), new object[] {foundTable});
            }
            else
            {
                // add to listbox
                string num = String.Format("{0:0}", listBoxTables.Items.Count+1);
                listBoxTables.Items.Add(num + ":  " + foundTable.LobbyTable.ToShortString());

                if (settings.TableTabs)
                {
                    // control and table
                    TableRendererControl control = new TableRendererControl();
                    foundTable.Renderer = control;
                    RendererTab tab = new RendererTab(foundTable, control);

                    // callbacks
                    foundTable.Activated += delegate(TableContainer activated) { tab.Highlight(); };
                    foundTable.WaitingForBlindToClose += delegate() { tab.SetTitle("Closing" + (foundTable.Number + 1)); };
                    foundTable.SittingOut += delegate() { tab.SetTitle("SitOut" + (foundTable.Number + 1)); };
                    foundTable.SittingIn += delegate() { tab.ResetTitle(); };
                    foundTable.Closed += delegate() { tab.SetTitle("Closed" + (foundTable.Number + 1)); };
                    foundTable.TimedOut += delegate() { tab.SetTitle("TimeOut" + (foundTable.Number + 1)); };
                    foundTable.ReActivated += delegate(TableContainer activated) { tab.ResetTitle(); };

                    // add to form
                    tabMain.Controls.Add(tab);
                    tabPages.Add(tab);
                }
            }
        }

        private delegate void RemoveTableFromUIDelegate(TableContainer tableContainer);
        private void removeTableFromUI(TableContainer tableContainer)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new RemoveTableFromUIDelegate(removeTableFromUI), new object[] { tableContainer });
            }
            else if (settings.TableTabs)
            {
                // find tab
                List<RendererTab> toBeRemoved = new List<RendererTab>();
                foreach (RendererTab tab in tabPages)
                {
                    if (tab.TableContainer == tableContainer)
                    {
                        toBeRemoved.Add(tab);
                    }
                }
                foreach (RendererTab tab in toBeRemoved)
                {
                    tabMain.Controls.Remove(tab);
                    tabPages.Remove(tab);
                }
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            app.ResetTables();
            resetTablesUI();
        }

        private delegate void ResetTablesUIDelegate();
        private void resetTablesUI()
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new ResetTablesUIDelegate(resetTablesUI), new Object[] { });
            }
            else
            {
                // listbox
                listBoxTables.Items.Clear();
                // tabs
                foreach (TabPage tab in tabPages)
                {
                    tabMain.Controls.Remove(tab);
                }
                tabPages.Clear();
            }
        }

        private void buttonPrecheck_Click(object sender, EventArgs e)
        {
            try
            {
                List<Rule> rules = RulesReader.readRules();
                RuleEvaluator evaluator = new RuleEvaluator(rules);
                RuleInterpreter interpreter = new RuleInterpreter(settings.SmallBlind, settings.BigBlind);
                DateTime startCheck = DateTime.Now;
                interpreter.precheck(rules);
                Log.Info("# prechecking rules took " + DateTime.Now.Subtract(startCheck).TotalMilliseconds + " ms");
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "precheck rules");
                MessageBox.Show(ex.Message, "Error Precheck", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSitIn_Click(object sender, EventArgs e)
        {
            try
            {
                sitIn();
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "sit-in");
                MessageBox.Show(ex.Message, "Error Sit-in", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSitOut_Click(object sender, EventArgs e)
        {
            try
            {
                app.SitOut();
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "sit-out");
                MessageBox.Show(ex.Message, "Error Sit-out", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonMouse_Click(object sender, EventArgs e)
        {
            try
            {
                app.ResetMouse();
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "reset mouse");
                MessageBox.Show(ex.Message, "Error reset mouse", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonBlinds_Click(object sender, EventArgs e)
        {
            app.WaitForBlinds();
        }

        private void autoStartLogic()
        {
            try
            {
                // sanity check
                sanityCheck();

                // mouse
                app.ResetMouse();

                // start
                app.StartPokerApplication(settings);

                // tables
                autoLocateTables(false);

                // sit-in
                sitIn();

                // start
                startLogic(false);

                // done
                resetAutoStart();
            }
            catch (Exception ex)
            {
                // UI
                resetAutoStart();

                // report
                ErrorHandler.ReportException(ex, "auto-start");
                MessageBox.Show(ex.Message, "Error Auto-Start", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private delegate void ResetAutoStartDelegate();

        private void resetAutoStart()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ResetAutoStartDelegate(resetAutoStart), new object[] { });
            }
            else
            {
                buttonAutoStart.Text = "Auto-Start";
                autoStartThread.Abort();
            }
        }

        private void buttonAutoStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (autoStartThread == null || !autoStartThread.IsAlive)
                {
                    buttonAutoStart.Text = "Stop Auto-Start";
                    autoStartThread = new Thread(new ThreadStart(autoStartLogic));
                    autoStartThread.Start();
                }
                else
                {
                    buttonAutoStart.Text = "Auto-Start";
                    autoStartThread.Abort();
                }                
            }
            catch (Exception ex)
            {
                resetAutoStart();
                ErrorHandler.ReportException(ex, "auto-start");
                MessageBox.Show(ex.Message, "Error Auto-Start", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonStartBottom_Click(object sender, EventArgs e)
        {
            try
            {
                // sanity check
                sanityCheck();

                // start logic
                startLogic(true);
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "start");
                MessageBox.Show(ex.Message, "Error Start", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonStopButtom_Click(object sender, EventArgs e)
        {
            try
            {
                stopLogic();
            }
            catch (Exception ex)
            {
                ErrorHandler.ReportException(ex, "stop");
                MessageBox.Show(ex.Message, "Error Stop", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private delegate void RandomReplaceTimeDelegate();
        private void randomReplaceTime()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new RandomReplaceTimeDelegate(randomReplaceTime), new object[] { });
            }
            else
            {
                if (checkBoxReplaceRandom.Checked)
                {
                    settings.MaxTime = int.Parse(textBoxReplace.Text) + (int)(new Random().NextDouble() * int.Parse(textBoxReplaceRandom.Text));
                }
                labelReplace.Text = labelReplaceLeft.Text = settings.MaxTime.ToString();
            }
        }

        private delegate void RandomAutoStopTimeDelegate();
        private void randomAutoStopTime()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new RandomAutoStopTimeDelegate(randomAutoStopTime), new object[] { });
            }
            else
            {
                if (checkBoxStopRandom.Checked)
                {
                    settings.AutoStopMins = int.Parse(textBoxAutoStop.Text) + (int)(new Random().NextDouble() * int.Parse(textBoxStopRandom.Text));
                }
                labelRuntime.Text = settings.AutoStopMins.ToString();
            }
        }

        private void timerRuntime_Tick(object sender, EventArgs e)
        {
            // counters
            runtimeSecs++;
            int runtimeMins = (int)(runtimeSecs / 60);

            // add one minute
            if (runtimeSecs % 60 == 0)
            {
                // labels
                labelTime.Text = runtimeMins.ToString();
                labelRuntimeLeft.Text = (settings.AutoStopMins - runtimeMins).ToString();
            }

            // auto stop -> wait for blinds
            if (!autoStopOccured && settings.AutoStopActivated && runtimeMins > settings.AutoStopMins)
            {
                // blinds
                Log.Info("auto-stop -> waiting for blinds");
                app.WaitForBlinds();
                autoStopOccured = true;

                // do not open new tables
                settings.MinTablesActivated = false;
                settings.MaxTimeActived = false;

                // UI
                checkBoxMinTables.Checked = false;
                checkBoxTime.Checked = false;
            }

            // auto stop -> kill application
            if (autoStopOccured && settings.AutoStopActivated && runtimeMins > (settings.AutoStopMins + 10))
            {
                Log.Info("auto-stop -> closing poker application");
                this.stopLogic();
                app.StopPokerApplication(settings);
            }

            // log time
            if (runtimeSecs % 600 == 0)
            {
                Security.AddTenMinutes(config["user"]);
            }
        }

        private void timerAutoStart_Tick(object sender, EventArgs e)
        {
            DateTime start = DateTime.ParseExact(this.textBoxAutoStart.Text, "HH:mm:ss", CultureInfo.InvariantCulture);
            if (DateTime.Now > start)
            {
                // vm
                loadDevice();
                // logic
                autoStartThread = new Thread(new ThreadStart(autoStartLogic));
                autoStartThread.Start();
                // timer
                timerAutoStart.Stop();
            }
        }
    }
};