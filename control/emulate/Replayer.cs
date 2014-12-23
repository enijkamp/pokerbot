using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;

namespace PokerBot
{
    public class Replayer
    {
        private List<Replay> replays = new List<Replay>()
        {
            new ReplayCircle(),
            new ReplayTriangle(),
            new ReplayWave(),
            new ReplayHorizontal(),
            new ReplayBubbles(),
            new ReplayEight(),
            new ReplayFish(),
            new ReplayFive(),
            new ReplayHouse(),
            new ReplayZickZack(),
        };

        private Mouse mouse;
        private Random random = new Random();

        public Replayer(DeviceControl control)
        {
             mouse = new BeamMouse(control);
        }

        public void Replay(Replay replay)
        {
            Point offset = new Point(mouse.Position.X, mouse.Position.Y);
            foreach (Point pt in replay.getReplay())
            {
                mouse.Move(offset.X + pt.X, offset.Y + pt.Y);
                Thread.SpinWait(replay.getSpinWait());
            }
        }

        public void ReplayShort(Replay replay)
        {
            Point offset = new Point(mouse.Position.X, mouse.Position.Y);
            int steps = (int)(replay.getReplay().Count * .5);
            for (int i = 0; i < steps; i++)
            {
                Point pt = replay.getReplay()[i];
                mouse.Move(offset.X + pt.X, offset.Y + pt.Y);
                Thread.SpinWait(replay.getSpinWait());
            }
        }

        public void ReplayRandom()
        {
            int position = (int)(replays.Count * random.NextDouble());
            Replay(replays[position]);
        }

        public void ReplayRandomShort()
        {
            int position = (int)(replays.Count * random.NextDouble());
            ReplayShort(replays[position]);
        }
    }
}
