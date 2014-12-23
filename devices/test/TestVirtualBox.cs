using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace PokerBot
{
    public class TestVirtualBox
    {

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
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

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
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
        }


        /*public class FrameBuffer : VirtualBox.IFramebuffer
        {
            private VirtualBox.IFramebuffer buffer;

            public FrameBuffer(VirtualBox.IFramebuffer buffer)
            {
                this.buffer = buffer;
            }

            public IntPtr Address { get { return buffer.Address; } }

            public uint BitsPerPixel { get { return buffer.BitsPerPixel; } }

            public uint BytesPerLine { get { return buffer.BytesPerLine; } }

            public uint Height { get { return buffer.Height; } }

            public uint HeightReduction { get { return buffer.HeightReduction; } }

            public VirtualBox.IFramebufferOverlay Overlay { get { return buffer.Overlay; } }

            public uint PixelFormat { get { return buffer.PixelFormat; } }

            public int UsesGuestVRAM { get { return buffer.UsesGuestVRAM; } }

            public uint Width { get { return buffer.Width; } }

            public ulong WinId { get { return buffer.WinId; } }

            public uint GetVisibleRegion(ref byte aRectangles, uint aCount)
            {
                return buffer.GetVisibleRegion(ref aRectangles, aCount);
            }

            public void Lock()
            {
                buffer.Lock();
            }

            public void NotifyUpdate(uint aX, uint aY, uint aWidth, uint aHeight)
            {
                buffer.NotifyUpdate(aX, aY, aWidth, aHeight);
            }

            public void ProcessVHWACommand(ref byte aCommand)
            {
                buffer.ProcessVHWACommand(ref aCommand);
            }

            public int RequestResize(uint aScreenId, uint aPixelFormat, ref byte aVRAM, uint aBitsPerPixel, uint aBytesPerLine, uint aWidth, uint aHeight)
            {
                return buffer.RequestResize(aScreenId, aPixelFormat, ref aVRAM, aBitsPerPixel, aBytesPerLine, aWidth, aHeight);
            }

            public void SetVisibleRegion(ref byte aRectangles, uint aCount)
            {
                buffer.SetVisibleRegion(ref aRectangles, aCount);
            }

            public void Unlock()
            {
                buffer.Unlock();
            }

            public int VideoModeSupported(uint aWidth, uint aHeight, uint aBpp)
            {
                return buffer.VideoModeSupported(aWidth, aHeight, aBpp);
            }
        }*/


        public static System.Drawing.Bitmap CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
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

        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        public unsafe static void Main(string[] args)
        {
            VirtualBox.VirtualBoxClass vbox = new VirtualBox.VirtualBoxClass();
            VirtualBox.IMachine vm = vbox.FindMachine("Poker Niklas");
            VirtualBox.Session session = new VirtualBox.SessionClass();

            VirtualBox.IProgress progress = vbox.OpenRemoteSession(session, vm.Id, "gui", null);


            progress.WaitForCompletion(-1);


            //int x = 0, y = 0;
            //VirtualBox.IFramebuffer ori = null;
            //session.Console.Display.GetFramebuffer((uint)0, out ori, out x, out y);
            //VirtualBox.IFramebuffer buffer = new FrameBuffer(ori);
            //session.Console.Display.SetFramebuffer(0, buffer);

            vm.ShowConsoleWindow();

            Thread.Sleep(10 * 1000);

            IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, "Poker Niklas [Running] - Sun VirtualBox");
            MoveWindow(hwnd, 0, 0, 1024+100, 768+100, false);
            Thread.Sleep(3 * 1000);
            /*Win32Control control = new Win32Control();
            control.MouseMoveTo(45, 45);
            control.MouseLeftClick();
            control.MouseMoveTo(45, 160);
            control.MouseLeftClick();

            Thread.Sleep(20 * 1000);
            Console.WriteLine("reset");
            control.ResetMouse();

            Console.WriteLine("reset done");
            Thread.Sleep(20 * 1000);*/

            
            /*int x = 0, y = 0;
            VirtualBox.IFramebuffer buf = null;
            session.Console.Display.GetFramebuffer((uint)0, out buf, out x, out y);




            CaptureWindow((IntPtr)buf.WinId).Save("test.bmp");*/

            Console.WriteLine("events");

            //session.Console.Mouse.PutMouseEvent(100, 100, 0, 0);
            /*Thread.Sleep(30 * 1000);
            for (int i = 51; i <= 52; i++)
            {
                Console.WriteLine(i);
                session.Console.Keyboard.PutScancode(i);
                Thread.Sleep(2000);
            }*/
            Thread.Sleep(5*1000);
            for (int i = 20; i <= 40; i++)
            {
                Console.WriteLine(i);
                session.Console.Keyboard.PutScancode(i);
                Thread.Sleep(2000);
            }
            Thread.Sleep(30*1000);
            session.Console.SaveState();



    



            /*
            int[] pixels = new int[128*128];
            int pos = 0;
            {
                buf.Lock();
                byte* ptr = (byte*)buf.Address.ToPointer();
                for (int i = 0; i < pixels.Length; i++)
                 {
        
                          byte b1 = (*ptr);
                           ptr++;
                           byte b2 = (*ptr);
                           ptr++;
                           byte b3 = (*ptr);
                           ptr++;
                           byte b4 = (*ptr);
                           ptr++;
                           int pixel = (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
                           pixels[pos++] = BitConverter.ToInt32(new byte[] {b1, b2, b3, b4}, 0);
                    
                 }
                buf.Unlock();

            }

            ImageTools.toBitmap(new Image(pixels, 128, 128), PixelFormat.Format32bppRgb).Save("test.bmp");
            Console.WriteLine("done");
            */
            //Console.ReadKey();

            //session.Console.Mouse.PutMouseEvent(100, 100, 0, 0);

            //Console.ReadKey();


            //byte[] buf = new byte[session.Console.Display.Width * session.Console.Display.Height * 1000];
            //session.Console.Display.TakeScreenShot(ref buf[0], (uint)session.Console.Display.Width, (uint)session.Console.Display.Height);
            
            //Console.ReadKey();
        }
    }
}
