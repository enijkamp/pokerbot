using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace PokerBot
{
    public class Win32Control : DeviceControl
    {
        private class Win32
        {
            [Flags]
            public enum MouseEventFlags
            {
                LEFTDOWN = 0x00000002,
                LEFTUP = 0x00000004,
                MIDDLEDOWN = 0x00000020,
                MIDDLEUP = 0x00000040,
                MOVE = 0x00000001,
                ABSOLUTE = 0x00008000,
                RIGHTDOWN = 0x00000008,
                RIGHTUP = 0x00000010
            }

            [DllImport("user32.dll")]
            public static extern bool SetCursorPos(int x, int y);

            [DllImport("user32.dll")]
            public static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);
        }

        public Win32Control()
        {
        }

        public void ResetMouse()
        {
            Cursor.Position = new Point(0, 0);
        }

        public void Suspend()
        {
        }

        public int DisplayWidth
        {
            get { return SystemInformation.PrimaryMonitorSize.Width; }
        }

        public int DisplayHeight
        {
            get { return SystemInformation.PrimaryMonitorSize.Height; }
        }

        public void MouseMoveTo(int x, int y)
        {
            Win32.SetCursorPos((int)x, (int)y);
        }

        public void MouseLeftClick()
        {
            Win32.mouse_event((uint)Win32.MouseEventFlags.LEFTDOWN, 0, 0, 0, new System.IntPtr());
            Win32.mouse_event((uint)Win32.MouseEventFlags.LEFTUP, 0, 0, 0, new System.IntPtr());
        }

        public void MouseRightClick()
        {
            Win32.mouse_event((uint)Win32.MouseEventFlags.RIGHTDOWN, 0, 0, 0, new System.IntPtr());
            Win32.mouse_event((uint)Win32.MouseEventFlags.RIGHTUP, 0, 0, 0, new System.IntPtr());
        }

        public void MouseDoubleLeftClick()
        {
            Thread.Sleep(20);
            MouseLeftClick();
            Thread.Sleep(50);
            MouseLeftClick();
        }

        public Point MousePosition
        {
            get { return Cursor.Position; }
        }

        public void KeyboardSend(string keys)
        {
            SendKeys.SendWait(keys);
        }

        public void KeyboardSendMinimizeAllWindows()
        {
        }        

        public void KeyboardSendPageDown()
        {
            SendKeys.SendWait("{PAGEDOWN}");
        }

        public void KeyboardSendPageUp(int times)
        {
            SendKeys.SendWait("{PAGEUP}");
        }

        public void KeyboardSendMinimize()
        {
            SendKeys.SendWait("%({SPACE})");
            SendKeys.SendWait("n");
        }

        public void KeyboardSendEnter()
        {
            SendKeys.SendWait("{Enter}");
        }

        public void KeyboardSendCursorDown()
        {
            SendKeys.SendWait("{Down}");
        }

        public void KeyboardSendCursorRight()
        {
            SendKeys.SendWait("{Right}");
        }

        public void KeyboardSendAltF4()
        {
            SendKeys.SendWait("%({F4})");
        }

        public Bitmap ScreenCapture(Rectangle region)
        {
            return readPixels(region);
        }
        
        private System.Drawing.Bitmap readPixels(Rectangle region) 
		{
            Bitmap bitmap = new Bitmap(region.Width, region.Height);
			using(Graphics graphics = Graphics.FromImage(bitmap))
			{
 				graphics.CopyFromScreen(region.X, region.Y, 0, 0, region.Size);
				graphics.Dispose();
			}
			return bitmap;
		}
    }
}
