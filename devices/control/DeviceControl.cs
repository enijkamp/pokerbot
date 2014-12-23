using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerBot
{
    public interface DeviceControl
    {
        void ResetMouse();

        int DisplayWidth { get; }

        int DisplayHeight { get; }

        void MouseMoveTo(int x, int y);

        void MouseLeftClick();

        void MouseRightClick();

        void MouseDoubleLeftClick();

        Point MousePosition { get; }

        void KeyboardSend(string keys);

        void KeyboardSendEnter();

        void KeyboardSendAltF4();

        void KeyboardSendCursorRight();

        void KeyboardSendCursorDown();

        void KeyboardSendMinimize();

        void KeyboardSendMinimizeAllWindows();

        void KeyboardSendPageDown();

        void KeyboardSendPageUp(int times);

        Bitmap ScreenCapture(Rectangle region);

        void Suspend();
    }
}
