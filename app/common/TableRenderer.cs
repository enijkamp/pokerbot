using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;

namespace PokerBot
{
    public interface TableRenderer : ImageRenderer
    {
        void render(Table table, TableLayout layout);

        void render(Table table, TableLayout layout, Situation situation, Rule rule, Decision decision, List<TableControl> controls);
    }
}
