using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace PokerBot
{
    public class Controller
    {
        private const int LEFT = 0, MIDDLE = 1, RIGHT = 2;

        private double betSlideTextLimit;
        private Mouse mouse;
        private Keyboard keyboard;
        private Random random = new Random();
        private TableIdentifier tableIdentifier;
        private Iterator<Image> screen;

        public Controller(Keyboard keyboard, Mouse mouse, TableIdentifier tableIdentifier, Iterator<Image> screen)
        {
            this.keyboard = keyboard;
            this.mouse = mouse;
            this.betSlideTextLimit = 0;
            this.tableIdentifier = tableIdentifier;
            this.screen = screen;
        }

        public Controller(Keyboard keyboard, Mouse mouse, double betSlideTextLimit, TableIdentifier tableIdentifier, Iterator<Image> screen)
        {
            this.keyboard = keyboard;
            this.mouse = mouse;
            this.betSlideTextLimit = betSlideTextLimit;
            this.tableIdentifier = tableIdentifier;
            this.screen = screen;
        }

        public Mouse Mouse
        {
            get { return mouse; }
        }

		public void CloseTable(TableLayout layout)
		{
            keyboard.pressAltF4();
		}
		
		public void CloseTableWithEnterSlow(TableLayout layout)
		{
		    int randomOff = (int)(random.NextDouble() * 4);
            mouse.MoveAndLeftClick(layout.Offset.X + layout.Close.X, layout.Offset.Y + layout.Close.Y, randomOff, randomOff);
			Thread.Sleep(800);
			keyboard.pressEnter();
		}

        public void CloseTableWithEnter(TableLayout layout)
        {
            keyboard.pressAltF4AndEnter();
        }
		
        public void PressSitOut(TableLayout table)
        {
            PressSitOut(mouse, table);
        }
        
        public void PressAutoBlind(TableLayout table)
        {
            mouse.MoveAndLeftClick(
                table.Offset.X + table.AutoBlindClick.X,
                table.Offset.Y + table.AutoBlindClick.Y,
                table.AutoBlindClick.Width,
                table.AutoBlindClick.Height);
        }
        
        public void PressWaitForBlind(TableLayout table)
        {
            mouse.MoveAndLeftClick(
                table.Offset.X + table.WaitForBlindClick.X,
                table.Offset.Y + table.WaitForBlindClick.Y,
                table.WaitForBlindClick.Width,
                table.WaitForBlindClick.Height);
        }

        public static void PressSitOut(Mouse mouse, TableLayout table)
        {
            mouse.MoveAndLeftClick(
                table.Offset.X + table.SitOutClick.X,
                table.Offset.Y + table.SitOutClick.Y,
                table.SitOutClick.Width,
                table.SitOutClick.Height);
        }

        public void PressFold(TableLayout layout)
        {
            clickRandomBigButton(layout, layout.ControlPoints[0]);
        }

        public void PressCheckOrFold(TableLayout layout, List<TableControl> controls)
        {
            foreach (TableControl control in controls)
            {
                if (control.Type == TableControl.ControlType.CHECK)
                {
                    clickRandomBigButton(layout, layout.ControlPoints[control.Position]);
                    return;
                }
            }
            PressFold(layout);
        }

        private Point LookupControl(Decision decision, TableLayout table, List<TableControl> controls)
        {
            // fold
            if (decision.DecisionType == Decision.Types.FOLD)
            {
                // check button exists?
                foreach (TableControl control in controls)
                {
                    if (control.Type == TableControl.ControlType.CHECK)
                    {
                        return table.ControlPoints[control.Position];
                    }
                }
                // fold
                return table.ControlPoints[LEFT];
            }
            // check
            if (decision.DecisionType == Decision.Types.CHECK)
            {
                // check button exists?
                foreach(TableControl control in controls)
                {
                    if (control.Type == TableControl.ControlType.CHECK)
                    {
                        return table.ControlPoints[control.Position];
                    }
                }
                // fold
                return table.ControlPoints[LEFT];
            }
            // bet
            if (decision.DecisionType == Decision.Types.BET)
            {
                return table.ControlPoints[RIGHT];
            }
            // call
            if(decision.DecisionType == Decision.Types.CALL)
            {
                // check button exists?
                foreach(TableControl control in controls)
                {
                    if (control.Type == TableControl.ControlType.CHECK)
                    {
                        return table.ControlPoints[control.Position];
                    }
                }
                // call if amount fits otherwise fold
                foreach (TableControl control in controls)
                {
                    if (control.Type == TableControl.ControlType.CALL)
                    {
                        // if call amount > maxbet then fold
                        if (!decision.HasAmount || control.Amount <= decision.Amount)
                        {
                            return table.ControlPoints[control.Position];
                        }
                        else
                        {
                            return table.ControlPoints[LEFT];
                        }
                    }
                }
                Log.Error("decision.DecisionType == Decision.Types.CALL -> Cannot find call or check button");
                return table.ControlPoints[LEFT];
            }
            // raise
            if (decision.DecisionType == Decision.Types.RAISE)
            {
                return table.ControlPoints[RIGHT];
            }

            throw new ArgumentException("Unknown decision");
        }

        private bool IsThereBetOrRaiseButton(List<TableControl> controls)
        {
            foreach (TableControl control in controls)
            {
                if (control.Type == TableControl.ControlType.BET || control.Type == TableControl.ControlType.RAISE)
                {
                    return true;
                }
            }
            return false;
        }

        public void Handle(Decision decision, TableLayout table, List<TableControl> controls)
        {
            Point pointToClick = LookupControl(decision, table, controls);
            if(decision.DecisionType == Decision.Types.BET)
            {
                Log.Info("pressing bet");
                if (IsThereBetOrRaiseButton(controls) && IsChangeAmountRequired(controls, decision.Amount))
                {
                    setAmountTextOrSlide(table, betSlideTextLimit, decision.Amount);
                    Thread.Sleep(RandomInt(150, 200));
                }
                clickRandomBigButton(table, pointToClick);
            }
            else if(decision.DecisionType == Decision.Types.RAISE)
            {
                Log.Info("pressing raise");
                if (IsThereBetOrRaiseButton(controls) && IsChangeAmountRequired(controls, decision.Amount))
                {
                    setAmountText(table, decision.Amount);
                    Thread.Sleep(RandomInt(150, 200));
                }
                clickRandomBigButton(table, pointToClick);
            }
            else if(decision.DecisionType == Decision.Types.CALL)
            {
                Log.Info("pressing call");
                clickRandomBigButton(table, pointToClick);
            }
            else if(decision.DecisionType == Decision.Types.CHECK)
            {
                Log.Info("pressing check");
                clickRandomBigButton(table, pointToClick);
            }
            else if(decision.DecisionType == Decision.Types.FOLD)
            {
                Log.Info("pressing fold");
                clickRandomBigButton(table, pointToClick);
            }
            else
            {
                throw new ArgumentException("Unknown decision type");
            }
        }

        private bool IsChangeAmountRequired(List<TableControl> controls, double amount)
        {
            // raise > decision amount -> we don't need to type in an amount
            if (getAmountFromButton(controls) >= amount)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void setAmountTextOrSlide(TableLayout table, double limit, double amount)
        {
            // click slider
            if (amount < limit)
            {
                clickSliderAmount(table, amount);
            }
            // type in
            else
            {
                setAmountText(table, amount);
            }
        }

        private void clickSliderAmount(TableLayout table, double amount)
        {
            double start = 0.02;
            double step = 0.02;
            int clicks = (int)(Math.Round((amount - start) / step));
            moveToRandomSlider(table, table.SliderClick);
            for (int i = 0; i < clicks; i++)
            {
                mouse.LeftClick();
                Thread.Sleep(RandomInt(80, 100));
            }
        }

        private void setAmountText(TableLayout table, double decisionAmount)
        {
            // text
            string amountText = String.Format("{0:0.00}", decisionAmount);
            amountText = amountText.Replace(',', '.');

            // type in and check
            int tries = 0;
            while (true)
            {
                // max tries
                tries++;
                if (tries > 2)
                {
                    ErrorHandler.ReportExceptionWithImage(new Exception("cannot type in amount"), "raise error", screen.next());
                    break;
                }

                // type in amount
                doubleClick(table, table.SliderText);
                Thread.Sleep(RandomInt(150, 250));
                keyboard.pressKeys(amountText);

                // get amount from button
                Thread.Sleep(RandomInt(50, 100));
                Image screenshot = screen.next();
                Image tableImage = TableOpener.CropTable(screenshot, table);
                List<TableControl> controls = tableIdentifier.identifyControls(tableImage);
                double amountButton = getAmountFromButton(controls);

                // double check
                double roundButton = Math.Round(amountButton, 2, MidpointRounding.AwayFromZero);
                double roundDecision = Math.Round(decisionAmount, 2, MidpointRounding.AwayFromZero);
                if (roundButton >= roundDecision)
                {
                    // ok
                    break;
                }
                else
                {
                    // error
                    Log.Error("cannot type in amount - try " + tries);
                }
            }
        }

        private double getAmountFromButton(List<TableControl> controls)
        {
            foreach (TableControl control in controls)
            {
                if (control.Type == TableControl.ControlType.RAISE || control.Type == TableControl.ControlType.BET)
                {
                    return control.Amount;
                }
            }
            throw new ArgumentException("cannot find raise button");
        }

        private int RandomInt(int begin, int end)
        {
            return (int)(random.NextDouble() * Math.Abs(end - begin)) + begin;
        }

        private void doubleClick(TableLayout table, Point position)
        {
            int plus = (int)(random.NextDouble() * 4.0);
            mouse.Move(table.Offset.X + position.X + plus, table.Offset.Y + position.Y);
            Thread.Sleep(RandomInt(200, 300));
            mouse.DoubleLeftClick();
        }

        private void moveToRandomSlider(TableLayout table, Point position)
        {
            mouse.Move(
                table.Offset.X + position.X + (int)(random.NextDouble() * 3),
                table.Offset.Y + position.Y + (int)(random.NextDouble() * 3));
        }

        private void clickRandomBigButton(TableLayout table, Point position)
        {
            mouse.MoveAndLeftClick(
                table.Offset.X + position.X, 
                table.Offset.Y + position.Y,
                60,
                29);
        }
    }
}
