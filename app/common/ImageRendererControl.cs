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
    public class ImageRendererControl : Control, ImageRenderer
    {
        // # public

        private List<RenderImage> renderImagesList = new List<RenderImage>();

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

        public Control Control
        {
            get { return this; }
        }


        // ## internal

        private List<RenderImage> images = new List<RenderImage>();

        public ImageRendererControl()
        {
            Size = new Size(800, 700);
            DoubleBuffered = true;
            ResizeRedraw = true;
            Paint += new PaintEventHandler(OnPaint);
        }

        private void repaint()
        {
            this.Update();
            this.Refresh();
        }

        private void setImages(List<RenderImage> images)
        {
            lock (this)
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