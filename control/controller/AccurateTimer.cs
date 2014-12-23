using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace PokerBot
{
    public class AccurateTimer
    {
        // ### WIN32 ###
        [DllImport("Winmm.dll")]
        private static extern int timeGetTime();

        [DllImport("winmm.dll")]
        private static extern uint timeGetDevCaps(out TimeCaps timeCaps, int size);

        struct TimeCaps
        {
            public uint minimum;
            public uint maximum;

            public TimeCaps(uint minimum, uint maximum)
            {
                this.minimum = minimum;
                this.maximum = maximum;
            }
        }

        [DllImport("WinMM.dll", SetLastError = true)]
        private static extern uint timeSetEvent(int msDelay, int msResolution,
                    TimerEventHandler handler, ref int userCtx, int eventType);

        [DllImport("WinMM.dll", SetLastError = true)]
        static extern uint timeKillEvent(uint timerEventId);

        public delegate void TimerEventHandler(uint id, uint msg, ref int userCtx,
            int rsv1, int rsv2);

        private void GetCapabilities(out uint minimum, out uint maximum)
        {
            TimeCaps timeCaps = new TimeCaps(0, 0);
            uint result = timeGetDevCaps(out timeCaps, Marshal.SizeOf(timeCaps));
            //if (result != 0) log("timeGetDevCaps result=" + result);
            minimum = timeCaps.minimum;
            maximum = timeCaps.maximum;
        }

        // ### PUBLIC ###

        public delegate bool TimerCallback();

        private TimerCallback callback;
        private uint fastTimer;
        private bool isDone = false;
        private TimerEventHandler handler;

        public AccurateTimer()
        {
            this.handler = new TimerEventHandler(tickHandler);
        }

        ~AccurateTimer()
        {
            timeKillEvent(fastTimer);
        }

        public void Start(TimerCallback callback, int interval)
        {
            // fix: CallbackOnCollectedDelegate was detected
            GC.KeepAlive(handler);

            // start
            isDone = false;
            this.callback = callback;
            int myData = 0;	// dummy data
            fastTimer = timeSetEvent(interval, interval, handler, ref myData, 1); // type=periodic
            while (!isDone)
            {
                Thread.Sleep(15);
            }
        }

        private void tickHandler(uint id, uint msg, ref int userCtx, int rsv1, int rsv2)
        {
            bool continueLoop = callback();
            if (!continueLoop)
            {
                isDone = true;
                timeKillEvent(fastTimer);
            }
        }
    }
}
