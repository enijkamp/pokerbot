using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;


namespace PokerBot
{
    /// <summary>
    /// Standard Keyboard Shortcuts used by most applications
    /// </summary>
    public enum StandardShortcut
    {
        Copy,
        Cut,
        Paste,
        SelectAll,
        Save,
        Open,
        New,
        Close,
        Print
    }

    /// <summary>
    /// Simulate keyboard key presses
    /// </summary>
    public static class KeyboardSimulator
    {

        #region Windows API Code

        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte key, byte scan, int flags, int extraInfo);

        #endregion

        #region Methods

        public static void KeyDown(Keys key)
        {
            keybd_event(ParseKey(key), 0, 0, 0);
        }

        public static void KeyUp(Keys key)
        {
            keybd_event(ParseKey(key), 0, KEYEVENTF_KEYUP, 0);
        }

        public static void KeyPress(Keys key)
        {
            KeyDown(key);
            KeyUp(key);
        }

        public static void SimulateStandardShortcut(StandardShortcut shortcut)
        {
            switch (shortcut)
            {
                case StandardShortcut.Copy:
                    KeyDown(Keys.Control);
                    KeyPress(Keys.C);
                    KeyUp(Keys.Control);
                    break;
                case StandardShortcut.Cut:
                    KeyDown(Keys.Control);
                    KeyPress(Keys.X);
                    KeyUp(Keys.Control);
                    break;
                case StandardShortcut.Paste:
                    KeyDown(Keys.Control);
                    KeyPress(Keys.V);
                    KeyUp(Keys.Control);
                    break;
                case StandardShortcut.SelectAll:
                    KeyDown(Keys.Control);
                    KeyPress(Keys.A);
                    KeyUp(Keys.Control);
                    break;
                case StandardShortcut.Save:
                    KeyDown(Keys.Control);
                    KeyPress(Keys.S);
                    KeyUp(Keys.Control);
                    break;
                case StandardShortcut.Open:
                    KeyDown(Keys.Control);
                    KeyPress(Keys.O);
                    KeyUp(Keys.Control);
                    break;
                case StandardShortcut.New:
                    KeyDown(Keys.Control);
                    KeyPress(Keys.N);
                    KeyUp(Keys.Control);
                    break;
                case StandardShortcut.Close:
                    KeyDown(Keys.Alt);
                    KeyPress(Keys.F4);
                    KeyUp(Keys.Alt);
                    break;
                case StandardShortcut.Print:
                    KeyDown(Keys.Control);
                    KeyPress(Keys.P);
                    KeyUp(Keys.Control);
                    break;
            }
        }

        static byte ParseKey(Keys key)
        {

            // Alt, Shift, and Control need to be changed for API function to work with them
            switch (key)
            {
                case Keys.Alt:
                    return (byte)18;
                case Keys.Control:
                    return (byte)17;
                case Keys.Shift:
                    return (byte)16;
                default:
                    return (byte)key;
            }

        }

        #endregion

    }

    public class TestSendKeys
    {
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,
           UIntPtr dwExtraInfo);


        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        static extern byte VkKeyScan(char ch);

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        static void PressKey(Keys key)
        {
            const uint KEYEVENTF_EXTENDEDKEY = 0x1;
            const uint KEYEVENTF_KEYUP = 0x2;
            // I had some Compile errors until I Casted the final 0 to UIntPtr like this...
            keybd_event((byte)key, (byte) 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event((byte)key, (byte) 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
        }

        public static void Main(string[] args)
        {
            string msg = "greetings notepad...";

            IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, "Poker Niklas [Running] - Sun VirtualBox");
            if (hwnd.Equals(IntPtr.Zero))
                Console.WriteLine("no hwnd");
            SetForegroundWindow(hwnd);
            Thread.Sleep(10000);
            for (int i = 0; i < msg.Length; i++)
            {
                PostMessage(hwnd, WM_KEYDOWN, VkKeyScan(msg[i]), 0);
                PostMessage(hwnd, WM_KEYUP, VkKeyScan(msg[i]), 0);
                PressKey(Keys.A);
            }

            SendKeys.SendWait("123");
            KeyboardSimulator.KeyPress(Keys.A);

            Process[] processes = Process.GetProcessesByName("VirtualBox");
            foreach (Process p in processes)
            {
                Console.WriteLine("test");
                IntPtr pFoundWindow = p.MainWindowHandle;
                for (int i = 0; i < msg.Length; i++)
                {
                    PostMessage(pFoundWindow, WM_KEYDOWN, VkKeyScan(msg[i]), 0);
                }
                
            }


            
            IntPtr noteWnd = FindWindow("Poker Niklas [Running] - Sun VirtualBox", null);
            if (!noteWnd.Equals(IntPtr.Zero))
            {
                if (!noteWnd.Equals(IntPtr.Zero))
                {
                    for (int i = 0; i < msg.Length; i++)
                    {
                        PostMessage(noteWnd, WM_KEYDOWN, VkKeyScan(msg[i]), 0);
                    }
                }
            }
            else
            {
                Console.WriteLine("argh");
                Console.ReadKey();
            }

        }

    }
}
