using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PokerBot
{
    public class AliveKeeper
    {
        public event IntEvent ContainersChangedEvent;
        public event IntEvent CloseTablesEvent;
        public event IntEvent OpenAdditionalTablesEvent;
        public event IntEvent ClosePokerApplicationNow;
        public event IntEvent ReplaceTableMinsLeft;
        public delegate void IntEvent(int num);

        private const long ONE_MIN_SECS = 60;
        private const int TIMEOUT_FOR_TABLE_MINS = 3;
        private const int FIRST_CHECK_MINS_IN_SECS = 10 * 60;
        private const int CLOSE_CHECK_MINS_IN_SECS = 6 * 60;

        private Dictionary<TableContainer, TableContainer.ActivateDelegate> attachedDelegates = new Dictionary<TableContainer, TableContainer.ActivateDelegate>();
        private Dictionary<TableContainer, DateTime> activeContainers = new Dictionary<TableContainer, DateTime>();
        private Timer timer;
        private long secs = 0, secsTotal = 0;
        private bool closeTablesOccured = false;

        private Settings settings;

        public AliveKeeper(Settings settings)
        {
            this.settings = settings;
        }

        ~AliveKeeper()
        {
            Stop();
        }

        public void Start(List<TableContainer> containers)
        {
            AttachDelegates(containers);
            Reset();
            timer = new Timer(new TimerCallback(Callback), "", 0, 1000);
        }

        public void Stop()
        {
            DetachDelegates();
            Reset();
            if(timer != null) timer.Dispose();
        }

        private void Reset()
        {
            activeContainers.Clear();
            secs = 0;
            closeTablesOccured = false;
        }

        private void Callback(Object state)
        {
            secs++;
            secsTotal++;

            // ###### TABLES ALIVE ######

            // check for dead tables
            Monitor.Enter(activeContainers);
            if (secs % ONE_MIN_SECS == 0)
            {
                // time-out or closed
                List<TableContainer> toBeRemoved = new List<TableContainer>();
                foreach(TableContainer current in activeContainers.Keys)
                {
                    TimeSpan diff = DateTime.Now.Subtract(activeContainers[current]);
                    if (diff.TotalMinutes > TIMEOUT_FOR_TABLE_MINS)
                    {
                        current.TimeOut();
                        toBeRemoved.Add(current);
                    }
                    else if (current.IsClosed)
                    {
                        toBeRemoved.Add(current);
                    }
                }
                foreach(TableContainer container in toBeRemoved)
                {
                    activeContainers.Remove(container);
                    ContainersChangedEvent(AvticeTables);
                }                
            }

            // check close tables reached
            bool firstCheckReached = !closeTablesOccured && secs > FIRST_CHECK_MINS_IN_SECS;
            bool minimumTablesReached = settings.MinTablesActivated && activeContainers.Count < settings.MinTablesNum;
            bool maximumTimeReached = settings.MaxTimeActived && secsTotal > (settings.MaxTime * 60);

            // ###### ADDITIONAL TABLES ######

            // open new tables
            if (firstCheckReached && minimumTablesReached && !closeTablesOccured)
            {
                // reset
                secs = 0;

                // open additional tables
                Log.Info("minimum tables reached -> opening additional tables");
                if (OpenAdditionalTablesEvent != null) OpenAdditionalTablesEvent(activeContainers.Count);

                // stop the alive keeper
                Stop();
            }

            // ###### MAX RESTART TIME ######

            // event
            if (secs % ONE_MIN_SECS == 0 && settings.MaxTimeActived)
            {
                if (ReplaceTableMinsLeft != null)
                    ReplaceTableMinsLeft((int)(settings.MaxTime - (secs / ONE_MIN_SECS)));
            }

            // close tables (replace tables)
            if (firstCheckReached && maximumTimeReached)
            {
                secs = 0;
                Log.Info("maximum time reached -> waiting for blinds");
                if (CloseTablesEvent != null) CloseTablesEvent(activeContainers.Count);
                closeTablesOccured = true;
            }

            // check close poker application
            bool timeIsUp = secs > CLOSE_CHECK_MINS_IN_SECS;
            bool allTablesClosed = activeContainers.Count == 0;

            // close poker application
            if (closeTablesOccured && (timeIsUp || allTablesClosed))
            {
                secsTotal = secs = 0;
                if (ClosePokerApplicationNow != null)
                    ClosePokerApplicationNow(activeContainers.Count);
            }

            Monitor.Exit(activeContainers);
        }

        private void AttachDelegates(List<TableContainer> containers)
        {
            foreach (TableContainer container in containers)
            {
                TableContainer.ActivateDelegate handler = delegate(TableContainer activated)
                {
                    Monitor.Enter(activeContainers);
                    if (!activeContainers.ContainsKey(activated))
                    {
                        activeContainers.Add(activated, DateTime.Now);
                        if (ContainersChangedEvent != null)
                        {
                            ContainersChangedEvent(AvticeTables);
                        }
                    }
                    else
                    {
                        activeContainers[activated] = DateTime.Now;
                    }
                    Monitor.Exit(activeContainers);
                };
                attachedDelegates.Add(container, handler);
                container.Activated += handler;
            }
        }

        private void DetachDelegates()
        {
            foreach (TableContainer container in attachedDelegates.Keys)
            {
                TableContainer.ActivateDelegate handler = attachedDelegates[container];
                container.Activated -= handler;
            }
            attachedDelegates.Clear();
        }

        public int AvticeTables
        {
            get { return activeContainers.Count; }
        }
    }
}
