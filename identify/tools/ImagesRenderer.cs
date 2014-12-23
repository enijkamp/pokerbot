using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PokerBot
{
	public class ImagesRenderer : Form
	{
		public System.Drawing.Image image = null;
		public List<System.Drawing.Image> images = new List<System.Drawing.Image>();
		public String text = "";

        public ImagesRenderer(System.Drawing.Image image) : this()
        {
            this.image = image;
        }

		public ImagesRenderer()
		{
			if(Environment.OSVersion.Platform.ToString() == "Unix")
			{
				Debug.WriteLine("using double buffering");
				DoubleBuffered = true;
			}
	        Size = new Size(300, 150);
	        ResizeRedraw = true;
	        Paint += new PaintEventHandler(OnPaint);
			Closed += delegate(object o, EventArgs e) { Application.Exit(); };
	        CenterToScreen();
		}
						
		public void repaint()
		{
			this.Update();
			this.Refresh();
		}
		
		public void setText(String text)
		{
			this.text = text;
		}
		
		public void setImage(System.Drawing.Image image) 
		{
			this.image = image;
			repaint();
		}
		
		public void addImage(System.Drawing.Image image) 
		{
			this.images.Add(image);
			repaint();
		}
		
		public void setImages(ICollection<System.Drawing.Image> images) 
		{
			this.images.Clear();				
			foreach(System.Drawing.Image image in images) 
			{
				this.images.Add(image);
			}
			repaint();
		}
		
		void OnPaint(object sender, PaintEventArgs e)
	    {      
	        using(Graphics g = e.Graphics) {
				if(this.image != null)
				{
					g.DrawImage(this.image, 0, 0);
				}
				int y = 100;
				int x = 0;
				foreach(System.Drawing.Image image in images) 
				{
					g.DrawImage(image, x, y);
					x += image.Width + 20;
					if(x > 700) 
					{
						x = 0;
						y += 80;
					}
				}
				g.DrawString(text, new Font("Verdana", 8), new SolidBrush(Color.Black), x, y);
			}
	    }
		
	}
}
