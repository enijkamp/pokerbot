using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.IO;

namespace PokerBot
{
    public class MouseRecorder
    {            
        // id
        private static string ID = "Five";
        private static int SPIN = 1000000;

        private static string[] header =
        {
            "using System;",
            "using System.Collections.Generic;",
            "using System.Text;",
            "using System.Drawing;",
            "",
            "namespace PokerBot",
            "{",
            "    public class Replay" + ID + " : Replay",
            "    {",
            "        private static List<Point> points = new List<Point>()",
            "        {",
        };

        private static string[] footer =
        {
            "        };",
            "",
            "        public List<Point> getReplay()",
            "        {",
            "             return points;",
            "        }",
            "",
            "        public int getSpinWait()",
            "        {",
            "             return " + SPIN + ";",
            "        }",
            "    }",
            "}",
        };

        public static void Main(string[] args)
        {
            // record
            Console.WriteLine("record"); 
            Mouse mouse = new BeamMouse(new Win32Control());
            Point offset = new Point(mouse.Position.X, mouse.Position.Y);
            List<Point> points = new List<Point>();
            for (int i = 0; i < 1000; i++)
            {
                points.Add(new Point(mouse.Position.X - offset.X, mouse.Position.Y - offset.Y));
                Thread.SpinWait(SPIN);
            }

            // sleep
            Console.WriteLine(points.Count);
            Thread.Sleep(2000);

            // replay
            Console.WriteLine("replay");            
            foreach (Point pt in points)
            {
                mouse.Move(offset.X + pt.X, offset.Y + pt.Y);
                Thread.SpinWait(SPIN);
            }

            // code
            Console.WriteLine("code");
            StreamWriter file = File.CreateText("Replay"+ID+".cs");
            foreach (string line in header)
            {
                file.WriteLine(line);
            }
            foreach (Point pt in points)
            {
                file.WriteLine("                 new Point("+pt.X+","+pt.Y+"),");            
            }
            foreach (string line in footer)
            {
                file.WriteLine(line);
            }
            file.Close();
        }
    }
}
