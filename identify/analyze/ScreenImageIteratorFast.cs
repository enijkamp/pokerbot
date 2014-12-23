using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PokerBot
{	
	public class ScreenImageIteratorFast : Iterator<Image>
	{
		private readonly Rectangle region;
        private DeviceControl control;
        private int[] pixels;
		
		public ScreenImageIteratorFast(DeviceControl control)
		{
            this.control = control;
            int width = control.DisplayWidth;
            int height = control.DisplayHeight;
			this.region = new Rectangle(0, 0, width, height);
            pixels = new int[width * height];
			Log.Debug("display size {"+width+","+height+"}");
		}

		public bool hasNext() 
		{
			return true;
		}
		
		public Image next() 
		{
            return toImage(control.ScreenCapture(region));
		}
			
		private Image toImage(Bitmap bitmap) {
			int width = bitmap.Size.Width;
			int height = bitmap.Size.Height;

			BitmapData bData = bitmap.LockBits(
			                                   new Rectangle(new Point(), bitmap.Size),
			                                   ImageLockMode.ReadOnly, 
			                                   PixelFormat.Format32bppArgb);
			Marshal.Copy(bData.Scan0, pixels, 0, pixels.Length);
			bitmap.UnlockBits(bData);
            bitmap.Dispose();

			return new Image(pixels, width, height);
		}
	}
}
