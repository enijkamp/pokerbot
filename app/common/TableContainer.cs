using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerBot
{
    public class TableContainer
    {
        public delegate void ActivateDelegate(TableContainer activated);
        public event ActivateDelegate Activated;
        public event ActivateDelegate ReActivated;

        public delegate void StateChangeDelegate();
        public event StateChangeDelegate WaitingForBlindToClose, SittingOut, SittingIn;

        public delegate void ClosedDelegate();
        public event ClosedDelegate Closed, TimedOut;

        private LobbyTable lobbyTable;

        private int number;
        private Image cornerTopLeft;
        private Image cornerBottomRight;
        private TableLayout layout;
        private TableRenderer renderer;

        // closing
        private bool isWaitingForBlind = false, isSittingOut = false;
        private bool isClosed = false, isTimedOut = false;

        // known table attributes
		private bool isFastTable = false;
		private int seat;
        private double money = 0;

        public TableContainer(int number, Image cornerTopLeft, Image cornerBottomRight, TableLayout layout, int seat, bool isFastTable, LobbyTable lobbyTable)
        {
            this.number = number;
            this.cornerTopLeft = cornerTopLeft;
            this.cornerBottomRight = cornerBottomRight;
            this.layout = layout;
            this.seat = seat;
            this.isFastTable = isFastTable;
            this.lobbyTable = lobbyTable;
        }

        private TableContainer()
        {
            this.number = -1;
            this.cornerTopLeft = null;
            this.cornerBottomRight = null;
            this.layout = null;
            this.seat = -1;
            this.isFastTable = true;
            this.lobbyTable = null;
        }

        public static TableContainer EmptyContainer()
        {
            return new TableContainer();
        }

        public LobbyTable LobbyTable
        {
            get { return lobbyTable; }
        }

        public int Number
        {
            get { return number; }
        }

        public int Seat
        {
            get { return seat; }
            set { seat = value; }
        }

        public double Money
        {
            get { return money; }
            set { money = value; }
        }
        
        public bool IsFastTable
        {
            get { return isFastTable; }
        }

        public bool IsSlowTable
        {
            get { return !isFastTable; }
        }

        public bool IsClosed
        {
            get { return isClosed; }
        }

        public bool IsTimedOut
        {
            get { return isTimedOut; }
        }

        public bool IsWaitingForBlind
        {
            get { return isWaitingForBlind; }
        }

        public void WaitForBlindToClose()
        {
            if (!isWaitingForBlind && WaitingForBlindToClose != null)
            {
                WaitingForBlindToClose();
            }
            isWaitingForBlind = true; 
        }

        public void ResetStatus()
        {
            isSittingOut = false;
            isWaitingForBlind = false;
            isTimedOut = false;
            isClosed = false;
        }

        public void SitIn()
        {
            if (isSittingOut && SittingIn != null)
            {
                SittingIn();
            }
            ResetStatus();
        }

        public void SitOut()
        {
            if (!isSittingOut && SittingOut != null)
            {
                SittingOut();
            }
            isSittingOut = true; 
        }

        public bool IsSittingOut
        {
            get { return isSittingOut; }
        }     

        public void Close()
        {
            isWaitingForBlind = false;
            isClosed = true;
            if (Closed != null)
            {
                Closed();
            }
        }

        public void TimeOut()
        {
            isWaitingForBlind = false;
            isTimedOut = true;
            if (TimedOut != null)
            {
                TimedOut();
            }
        }

        public Image CornerTopLeft
        {
            get { return cornerTopLeft; }
        }

        public Image CornerBottomRight
        {
            get { return cornerBottomRight; }
        }

        public TableLayout Layout
        {
            get { return layout; }
        }

        public TableRenderer Renderer
        {
            get { return renderer; }
            set { renderer = value; }
        }

        public void Active()
        {
            if (ReActivated != null && isTimedOut)
            {
                ReActivated(this);
                isTimedOut = false;
            }
            if (Activated != null)
            {
                Activated(this);
            }
        }
    }
}
