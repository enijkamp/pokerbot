using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;

namespace PokerBot
{
    public class ImageTools
    {
        public static Image rectImage()
        {
            int[] pixels = new int[3 * 3];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.Red.ToArgb();
            }
            return new Image(pixels, 3, 3);
        }

        public static Bitmap toBitmap(Image image, PixelFormat format)
        {
            Bitmap bmp = new Bitmap(image.width, image.height, format);
            BitmapData bmpData = bmp.LockBits(
                               new Rectangle(0, 0, bmp.Width, bmp.Height),
                               ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(image.pixels, 0, bmpData.Scan0, image.pixels.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
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

        public static bool match(Image image1, Image image2)
        {
            for (int i = 0; i < image1.pixels.Length; i++)
            {
                if (image1.pixels[i] != image2.pixels[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static Color[] getSignificantColorRange(Image image)
        {
            // count colors
            Dictionary<int, int> colors = new Dictionary<int, int>();
            foreach (int color in image.pixels)
            {
                if (!colors.ContainsKey(color))
                {
                    colors[color] = 0;
                }
                colors[color]++;
            }
            // identify midrange
            int R = 0, G = 0, B = 0, count = 0;
            foreach (int color in colors.Keys)
            {
                if (colors[color] > 50)
                {
                    count++;
                    int r = (color >> 16) & 0xff;
                    int g = (color >> 8) & 0xff;
                    int b = color & 0xff;
                    R += r;
                    G += g;
                    B += b;
                }
            }
            R /= count;
            G /= count;
            B /= count;
            // define range
            Color min = Color.FromArgb(R - 10, G - 10, B - 10);
            Color max = Color.FromArgb(R + 10, G + 10, B + 10);
            return new Color[] { min, max };
        }

        public static int countPixels(Image image, Color[] range)
        {
            // range
            int minR = range[0].R, minG = range[0].G, minB = range[0].B;
            int maxR = range[1].R, maxG = range[1].G, maxB = range[1].B;
            // count
            int count = 0;
            for (int i = 0; i < image.pixels.Length; i++)
            {
                int pixel = image.pixels[i];
                int r = (pixel >> 16) & 0xff;
                int g = (pixel >> 8) & 0xff;
                int b = pixel & 0xff;

                bool min = r >= minR && g >= minG && b >= minB;
                bool max = r <= maxR && g <= maxG && b <= maxB;

                if (min && max) count++;
            }
            return count;
        }
    }
}
