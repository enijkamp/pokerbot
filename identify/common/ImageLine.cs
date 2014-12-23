using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerBot
{
    public class ImageLine : List<Image>
    {
        private int y;

        public ImageLine(int y, List<Image> images)
        {
            base.AddRange(images);
            this.y = y;
        }

        public int Y { get { return y; } }
    }
}
