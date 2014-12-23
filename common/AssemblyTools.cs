using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;

namespace PokerBot
{
    public class AssemblyTools
    {
        public static Stream getAssemblyStream(string file)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            string[] names = ass.GetManifestResourceNames();
            foreach (string name in names)
            {
                if (name.Contains(file))
                {
                    return ass.GetManifestResourceStream(name);
                }
            }
            throw new Exception("cannot find resource '" + file + "'");
        }

        public static Image getAssemblyImage(string file)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            string[] names = ass.GetManifestResourceNames();
            foreach (string name in names)
            {
                if (name.Contains(file))
                {
                    Stream stream = ass.GetManifestResourceStream(name);
                    Bitmap bitmap = Bitmap.FromStream(stream) as Bitmap;
                    Image Image = ImageTools.toImage(bitmap);
                    stream.Close();
                    return Image;
                }
            }
            throw new Exception("cannot find resource '" + file + "'");
        }
    }
}
