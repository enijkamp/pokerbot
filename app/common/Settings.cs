using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerBot
{
    public class Settings
    {
        public bool PlayMoney = false;
        public bool AutoMoveMouse = true;
        public bool AutoClick = true;
        public bool ReplayMouseMoves = true;
        public bool Sleep = true;
        public bool Beep = false;
        public bool TableTabs = false;
        public bool FastMouse = false;

        public string Name = string.Empty;

        public bool PreCheckRules = true;

        public int AutoLocateTablesNum = 21;

        public bool MinTablesActivated = true;
        public int MinTablesNum = 14;

        public bool Speech = false;

        public bool MaxTimeActived = true;
        public int MaxTime = 75;

        public bool AutoStopActivated = true;
        public int AutoStopMins = 240;

        public double CloseTableMoneyMin = 1;
        public double CloseTableMoneyMax = 3;
        public bool CloseTableActivated = true;

        public bool WindowSwitcherActivated = true;

        public List<Color> TaskbarColors = new List<Color>();

        public enum StakesEnum { OneTwo, TwoFive }

        private StakesEnum stakes = StakesEnum.OneTwo;

        public StakesEnum Stakes
        {
            set { stakes = value; }
            get { return stakes; }
        }

        public double SmallBlind
        {
            get { return stakes == StakesEnum.OneTwo ? 0.01 : 0.02; }
        }

        public double BigBlind
        {
            get { return stakes == StakesEnum.OneTwo ? 0.02 : 0.05; }
        }

        public void StakesByString(string stakes)
        {
            if (stakes == "1/2")
            {
                Stakes = StakesEnum.OneTwo;
            }
            else if (stakes == "2/5")
            {
                Stakes = StakesEnum.TwoFive;
            }
            else
            {
                throw new ArgumentException("unknown stakes '"+stakes+"'");
            }
        }
    }
}
