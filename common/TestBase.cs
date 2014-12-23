using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PokerBot
{	
	public class TestBase
	{		
		delegate void AddImageDelegate(System.Drawing.Image value);
		
		public static void addImage(ImagesRenderer renderer, System.Drawing.Image image) {
            if (renderer.IsHandleCreated)
                renderer.BeginInvoke(new AddImageDelegate(renderer.addImage), new object[] {image});
		}
		
		delegate void SetTextDelegate(string value);
		
		public static void setText(ImagesRenderer renderer, String text) 
		{
            if (renderer.IsHandleCreated)
			    renderer.BeginInvoke(new SetTextDelegate(renderer.setText), new object[] {text});
		}
		
		delegate void SetImageDelegate(System.Drawing.Image value);
		
		public static void setImage(ImagesRenderer renderer, System.Drawing.Image image) 
		{
            if(renderer.IsHandleCreated)
                renderer.BeginInvoke(new SetImageDelegate(renderer.setImage), new object[] {image});
		}

        delegate void Repaint();

        public static void repaint(ImagesRenderer renderer)
        {
            if (renderer.IsHandleCreated)
                renderer.BeginInvoke(new Repaint(renderer.repaint), new object[] { });
        }	

		public static Image toImage(Bitmap bitmap) 
		{
			int width = bitmap.Size.Width;
			int height = bitmap.Size.Height;
			int[] pixels = new int[width * height];

			BitmapData bData = bitmap.LockBits(
			                                   new Rectangle(new Point(), bitmap.Size),
			                                   ImageLockMode.ReadOnly, 
			                                   PixelFormat.Format32bppArgb);
			Marshal.Copy(bData.Scan0, pixels, 0, pixels.Length);
			bitmap.UnlockBits(bData);

			return new Image(pixels, width, height);
		}
		
		public static Bitmap toBitmap(Image image) 
		{
			Bitmap bmp = new Bitmap(image.width, image.height, PixelFormat.Format32bppArgb);
			BitmapData bmpData = bmp.LockBits(
		                       new Rectangle(0, 0, bmp.Width, bmp.Height),   
		                       ImageLockMode.WriteOnly, bmp.PixelFormat);
			Marshal.Copy(image.pixels, 0, bmpData.Scan0, image.pixels.Length);
			bmp.UnlockBits(bmpData);
			return bmp;
		}

		public static void saveBitmap(string name, int id, System.Drawing.Image image)
		{
			image.Save(name + "_" + id + ".png", ImageFormat.Png);
		}
		
		public static ICollection<Image> toImageCollection(ICollection<HashImage> images) {
			List<Image> output = new List<Image>();
			foreach(Image image in images)
			{
				output.Add(image);
			}
			return output;		
		}
		
		public static ImagesRenderer newImageRenderer()
		{
			// lock
			EventWaitHandle wait = new AutoResetEvent(false);
			
			// ui
			ImagesRenderer renderer = null;
			Thread threadUi = new Thread(delegate()
			{				
				renderer = new ImagesRenderer();
				wait.Set();
				Application.Run(renderer);
			});
			threadUi.Start();
			wait.WaitOne();
			
			// updater
			Thread threadUpdate = new Thread(delegate()
			{				
				while(true)
				{
					Thread.Sleep(500);
					repaint(renderer);
				}
			});
			threadUpdate.Start();
			
			return renderer;
	    }		
				
		public static int uniqueId(HashImage image)
		{
			return Math.Abs(image.GetHashCode());
		}
	}
}
