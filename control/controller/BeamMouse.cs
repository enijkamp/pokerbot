using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace PokerBot
{
    public class BeamMouse : Mouse
    {
        private DeviceControl control;

        public BeamMouse(DeviceControl control)
        {
            this.control = control;
        }

        public void Move(int x, int y)
        {
            control.MouseMoveTo(x, y);
        }

        public void MoveAndLeftClick(int x, int y, int rx, int ry)
        {
            Move(x, y);
            LeftClick();
        }

        public void LeftClick()
        {
            control.MouseLeftClick();
        }

        public void RightClick()
        {
            control.MouseRightClick();
        }

        public void DoubleLeftClick()
        {
            control.MouseDoubleLeftClick();
        }

        public Point Position
        {
            get { return control.MousePosition; }
        }
    }
}
