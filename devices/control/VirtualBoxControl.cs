using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PokerBot
{
    public class VirtualBoxControl : DeviceControl
    {
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

            [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
            public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        }

        private VirtualBox.IMachine vm = null;
        private VirtualBox.Session session = null;
        private VirtualBox.IFramebuffer buffer = null;

        private int mouseX = 0, mouseY = 0;

        public static Random random = new Random();

        public VirtualBoxControl(string name) : this(name, 1024, 750)
        {
        }

        public VirtualBoxControl(string name, int width, int height)
        {
            // boot
            Console.WriteLine("# booting vm");
            VirtualBox.VirtualBoxClass vbox = new VirtualBox.VirtualBoxClass();
            vm = vbox.FindMachine(name);
            session = new VirtualBox.SessionClass();
            VirtualBox.IProgress progress = vbox.OpenRemoteSession(session, vm.Id, "gui", null);
            progress.WaitForCompletion(-1);
            vm.ShowConsoleWindow();

            // Wait
            while (vm.State != VirtualBox.MachineState.MachineState_Running)
            {
                Thread.Sleep(1000);
            }

            // buffer
            int x = 0, y = 0;
            session.Console.Display.GetFramebuffer((uint)0, out buffer, out x, out y);

            // disable mouse integration            
            Thread.Sleep(3000);
            Console.WriteLine("disabling mouse integration");
            DisableMouseIntegration(name, width, height);
        }

        ~VirtualBoxControl()
        {
            Suspend();
        }

        private void DisableMouseIntegration(string name, int screenWidth, int screenHeight)
        {
            IntPtr hwnd = User32.FindWindowByCaption(IntPtr.Zero, name + " [Running] - Oracle VM VirtualBox");
            if (hwnd == IntPtr.Zero)
            {
                hwnd = User32.FindWindowByCaption(IntPtr.Zero, name + " [wird ausgeführt] - Oracle VM VirtualBox");
            }
            if (hwnd == IntPtr.Zero)
            {
                throw new Exception("Cannot locate VM window");
            }
            User32.MoveWindow(hwnd, 0, 0, screenWidth, screenHeight, false);
            Thread.Sleep(1 * 1000);
            Win32Control control = new Win32Control();
            control.MouseMoveTo(45, 45);
            control.MouseLeftClick();
            Thread.Sleep(1 * 1000);
            control.MouseMoveTo(45, 160);
            control.MouseLeftClick();
        }

        public void ResetMouse()
        {
            Console.WriteLine("resetting mouse");
            try
            {
                if (random.NextDouble() > .5)
                {
                    // reset mouse to top left corner
                    for (int i = 15000; i > 0; i--)
                    {
                        session.Console.Mouse.PutMouseEvent((random.NextDouble() > .5) ? -1 : 0, (random.NextDouble() > .5) ? -1 : 0, 0, 0, 0);
                        Thread.SpinWait(80000);
                    }
                    mouseY = mouseX = 0;
                }
                else
                {
                    // reset mouse to top right corner
                    for (int i = 0; i < 15000; i++)
                    {
                        try
                        {
                            session.Console.Mouse.PutMouseEvent((random.NextDouble() > .5) ? +1 : 0, (random.NextDouble() > .5) ? -1 : 0, 0, 0, 0);
                        }
                        catch (Exception)
                        {
                            // ignore: some events dont make it
                        }
                        Thread.SpinWait(80000);
                    }
                    mouseY = 0;
                    mouseX = DisplayWidth;
                }
            }
            catch (Exception)
            {
                /* ignore vm event error */
            }
            Log.Info("reset done");
        }

        public static int RandomInt(int begin, int end)
        {
            return (int)(random.NextDouble() * Math.Abs(end - begin)) + begin;
        }

        public void Suspend()
        {
            try
            {
                if (vm.State != VirtualBox.MachineState.MachineState_Saved || vm.State != VirtualBox.MachineState.MachineState_PoweredOff)
                    session.Console.SaveState();
            }
            catch (Exception)
            {
            }
        }

        public int DisplayWidth
        {
            get
            {
                uint screen = 0;
                uint height = 0;
                uint width = 0;
                uint bpp = 0;
                session.Console.Display.GetScreenResolution(screen, out width, out height, out bpp);
                return (int) width;
            }
        }

        public int DisplayHeight
        {
            get 
            {
                uint screen = 0;
                uint height = 0;
                uint width = 0;
                uint bpp = 0;
                session.Console.Display.GetScreenResolution(screen, out width, out height, out bpp);
                return (int) height; 
            }
        }

        public void MouseMoveTo(int x, int y)
        {
            try
            {
                session.Console.Mouse.PutMouseEventAbsolute(x, y, 0, 0, 0);
                mouseX = x;
                mouseY = y;
            }
            catch (Exception ex)
            {
                // cannot move mouse?
                if(ex.Message.Contains("VERR_PDM_NO_QUEUE_ITEMS"))
                {
                    Log.Warn("Cannot move virtualbox mouse (VERR_PDM_NO_QUEUE_ITEMS)");
                }
                else
                {
                    throw ex;
                }
            }
        }

        public void MouseLeftClick()
        {            
            // move to send click events
            Thread.Sleep(50);
            session.Console.Mouse.PutMouseEvent(-1, -1, 0, 0, 0);
            session.Console.Mouse.PutMouseEvent(+1, +1, 0, 0, 0);

            // click
            Thread.Sleep(50);
            session.Console.Mouse.PutMouseEvent(0, 0, 0, 0, (int)VirtualBox.MouseButtonState.MouseButtonState_LeftButton);
            
            // move to send click events
            Thread.Sleep(50);
            session.Console.Mouse.PutMouseEvent(-1, -1, 0, 0, 0);
            session.Console.Mouse.PutMouseEvent(+1, +1, 0, 0, 0);

            // move to send click events
            Thread.Sleep(50);
            session.Console.Mouse.PutMouseEvent(-1, -1, 0, 0, 0);
            session.Console.Mouse.PutMouseEvent(+1, +1, 0, 0, 0);
        }

        public void MouseRightClick()
        {
            // click
            Thread.Sleep(50);
            session.Console.Mouse.PutMouseEvent(0, 0, 0, 0, (int)VirtualBox.MouseButtonState.MouseButtonState_RightButton);

            // move to send click events
            Thread.Sleep(50);
            session.Console.Mouse.PutMouseEvent(-1, -1, 0, 0, 0);
            session.Console.Mouse.PutMouseEvent(+1, +1, 0, 0, 0);

            // move to send click events
            Thread.Sleep(50);
            session.Console.Mouse.PutMouseEvent(-1, -1, 0, 0, 0);
            session.Console.Mouse.PutMouseEvent(+1, +1, 0, 0, 0);
        }

        public void MouseDoubleLeftClick()
        {
            Thread.Sleep(20);
            session.Console.Mouse.PutMouseEvent(0, 0, 0, 0, (int)VirtualBox.MouseButtonState.MouseButtonState_LeftButton);
            // move to send click events
            session.Console.Mouse.PutMouseEvent(-1, -1, 0, 0, 0);
            session.Console.Mouse.PutMouseEvent(+1, +1, 0, 0, 0);
            session.Console.Mouse.PutMouseEvent(0, 0, 0, 0, (int)VirtualBox.MouseButtonState.MouseButtonState_LeftButton);
            // move to send click events
            session.Console.Mouse.PutMouseEvent(-1, -1, 0, 0, 0);
            session.Console.Mouse.PutMouseEvent(+1, +1, 0, 0, 0);
            Thread.Sleep(100);
        }

        public Point MousePosition
        {
            get { return new Point(mouseX, mouseY); }
        }

        // http://www.win.tue.nl/~aeb/linux/kbd/scancodes-1.html
        // order is implied by keyboard layout
        private static Dictionary<char, int> scancode = new Dictionary<char, int>()
        {
            {'1',2},
            {'2',3},
            {'3',4},
            {'4',5},
            {'5',6},
            {'6',7},
            {'7',8},
            {'8',9},
            {'9',10},
            {'0',11},
            {',',51},
            {'.',52},
            {'c',46},
            {'n',49},
            {'m',50}
        };

        private const int SCANCODE_CURSOR_DOWN = 80;
        private const int SCANCODE_CURSOR_RIGHT = 77;
        private const int SCANCODE_ENTER = 28;
        private const int SCANCODE_F4 = 62;
        private const int SCANCODE_ALT = 56;
        private const int SCANCODE_SPACE = 57;
        private const int SCANCODE_PAGE_DOWN = 81;
        private const int SCANCODE_PAGE_UP = 73;
        private const int SCANCODE_ALT_BREAK = SCANCODE_ALT + 128;

        private const int SCANCODE_PRESS = 0xE0;
        private const int SCANCODE_WINDOWS_PRESS = 0x5B;
        private const int SCANCODE_WINDOWS_BREAK = 0xDB;

        private Array SCANCODES_ALT_F4 = new ArrayList() { SCANCODE_ALT, SCANCODE_F4 }.ToArray(typeof(int));
        private Array SCANCODES_ALT_SPACE = new ArrayList() { SCANCODE_ALT, SCANCODE_SPACE }.ToArray(typeof(int));
        private Array SCANCODES_WIN_PRESS = new ArrayList() { SCANCODE_PRESS, SCANCODE_WINDOWS_PRESS }.ToArray(typeof(int));
        private Array SCANCODES_WIN_BREAK = new ArrayList() { SCANCODE_PRESS, SCANCODE_WINDOWS_BREAK }.ToArray(typeof(int));

        public void KeyboardSend(string keys)
        {
            foreach (char chr in keys)
            {
                Thread.Sleep(RandomInt(100, 150));
                session.Console.Keyboard.PutScancode(scancode[chr]);
            }
        }

        public void KeyboardSendMinimize()
        {
            session.Console.Keyboard.PutScancodes(ref SCANCODES_ALT_SPACE);
            Thread.Sleep(150);
            session.Console.Keyboard.PutScancode(SCANCODE_ALT_BREAK);
            KeyboardSend("n");
        }

        public void KeyboardSendMinimizeAllWindows()
        {
            session.Console.Keyboard.PutScancodes(ref SCANCODES_WIN_PRESS);
            KeyboardSend("m");
            Thread.Sleep(150);
            session.Console.Keyboard.PutScancodes(ref SCANCODES_WIN_BREAK);
        }        

        public void KeyboardSendPageDown()
        {
            session.Console.Keyboard.PutScancode(SCANCODE_PAGE_DOWN);
        }

        public void KeyboardSendPageUp(int times)
        {
            for (int i = 0; i < times; i++)
            {
                session.Console.Keyboard.PutScancode(SCANCODE_PAGE_UP);
            }
        }

        public void KeyboardSendEnter()
        {
            session.Console.Keyboard.PutScancode(SCANCODE_ENTER);
        }

        public void KeyboardSendCursorRight()
        {
            session.Console.Keyboard.PutScancode(SCANCODE_CURSOR_RIGHT);
        }

        public void KeyboardSendCursorDown()
        {
            session.Console.Keyboard.PutScancode(SCANCODE_CURSOR_DOWN);
        }

        public void KeyboardSendAltF4()
        {
            session.Console.Keyboard.PutScancodes(ref SCANCODES_ALT_F4);
            Thread.Sleep(150);
            session.Console.Keyboard.PutScancode(SCANCODE_ALT_BREAK);
        }

        public Bitmap ScreenCapture(Rectangle region)
        {
            return CaptureWindow((IntPtr)buffer.WinId, region);
        }

        private static System.Drawing.Bitmap CaptureWindow(IntPtr handle, Rectangle region)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = region.Width;
            int height = region.Height;

            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, region.X, region.Y, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            System.Drawing.Bitmap img = System.Drawing.Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }
    }
}
