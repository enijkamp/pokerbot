using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PokerBot
{
    public class Keyboard
    {
        private DeviceControl input;

        public Keyboard(DeviceControl input)
        {
            this.input = input;
        }

        public void pressKeys(string keys)
        {
            input.KeyboardSend(keys);
        }
        
        public void pressEnter()
        {
            input.KeyboardSendEnter();
        }

        public void pressCursorDown()
        {
            input.KeyboardSendCursorDown();
        }

        public void pressCursorRight()
        {
            input.KeyboardSendCursorRight();
        }

        public void pressPageDown()
        {
            input.KeyboardSendPageDown();
        }

        public void pressPageUp(int times)
        {
            input.KeyboardSendPageUp(times);
        }

        public void pressMinimizeWindow()
        {
            input.KeyboardSendMinimize();
        }

        public void pressAltF4()
        {
            input.KeyboardSendAltF4();
        }

        public void pressAltF4AndEnter()
        {
            input.KeyboardSendAltF4();
            Thread.Sleep(100);
            input.KeyboardSendEnter();
        }
    }
}
