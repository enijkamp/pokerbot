using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace PokerBot
{
    public class ImageRendererForm : Form, ImageRenderer
    {
        // # public

        private List<RenderImage> renderImagesList = new List<RenderImage>();

        public static ImageRendererForm newImageRendererForm()
        {
            // lock
            EventWaitHandle wait = new AutoResetEvent(false);

            // ui
            ImageRendererForm form = null;
            Thread threadUi = new Thread(delegate()
            {
                form = new ImageRendererForm();
                wait.Set();
                Application.Run(form);
            });
            threadUi.Start();
            wait.WaitOne();

            return form;
        }

        public Control Control { get { return this; } }

        public void renderImage(Image image, int x, int y)
        {
            renderImagesList.Add(new RenderImage(ImageTools.toBitmap(image), new Point(x, y)));
            if (IsHandleCreated)
            {
                BeginInvoke(new SetImagesDelegate(setImages), new object[] { renderImagesList });
                BeginInvoke(new Repaint(repaint), new Object[] { });
            }
        }

        public void renderImage(Image image, Point point)
        {
            renderImagesList.Add(new RenderImage(ImageTools.toBitmap(image), point));
            if (IsHandleCreated)
            {
                BeginInvoke(new SetImagesDelegate(setImages), new object[] { renderImagesList });
                BeginInvoke(new Repaint(repaint), new Object[] { });
            }
        }

        public void clearImages()
        {
            renderImagesList.Clear();
        }

        delegate void SetImagesDelegate(List<RenderImage> value);
        delegate void Repaint();

        // # internal

        private List<RenderImage> images = new List<RenderImage>();

        private ImageRendererForm()
        {
            BackColor = Color.White;
            Size = new Size(800, 800);
            DoubleBuffered = true;
            ResizeRedraw = true;
            Paint += new PaintEventHandler(OnPaint);
            Closed += delegate(object o, EventArgs e) { Environment.Exit(0); };
            CenterToScreen();
        }
						
		private void repaint()
		{
			this.Update();
			this.Refresh();
		}

        private void setImages(List<RenderImage> images)
		{
			lock(this)
			{
				this.images.Clear();
				this.images.AddRange(images);
			}
		}

        private void OnPaint(object sender, PaintEventArgs e)
	    {
            Graphics g = e.Graphics;
            lock (this)
            {
                // images
                foreach (RenderImage image in images)
                {
                    g.DrawImage(image.Image, image.Position.X, image.Position.Y);
                }
            }
	    }
    }
}
