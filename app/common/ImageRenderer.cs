using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PokerBot
{
    public interface ImageRenderer
    {
        Control Control { get; }

        void renderImage(Image image, int x, int y);

        void renderImage(Image image, Point point);

        void clearImages();
    }
}
