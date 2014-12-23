using System;

namespace PokerBot
{
	public class Log
	{
        public enum Level { FINEST, FINE, DEBUG, INFO, WARN, ERROR }

        public static Level loglevel = Level.INFO;

        public static void SetLevel(Level level)
        {
            loglevel = level;
        }

        public static void Finest(string text)
        {
            Write(Level.FINEST, text);
        }

        public static void Debug(int text)
        {
            Write(Level.DEBUG, text.ToString());
        }

        public static void Debug(string text)
        {
            Write(Level.DEBUG, text);
        }

        public static void DebugIf(bool condition, string text)
        {
            if(condition)
                Write(Level.DEBUG, text);
        }

		public static void Fine(int text)
		{
            Write(Level.FINE, text.ToString());
		}

        public static void FineIf(bool condition, int text)
        {
            if(condition)
                Write(Level.FINE, text.ToString());
        }

        public static void FineIf(bool condition, string text)
        {
            if (condition)
                Write(Level.FINE, text);
        }

        public static void Fine(string text)
        {
            Write(Level.FINE, text.ToString());
        }

        public static void Info(int text)
        {
            Write(Level.ERROR, text.ToString());
        }

        public static void Info(string text)
        {
            Write(Level.INFO, text);
        }

        public static void Warn(string text)
        {
            Write(Level.WARN, "######################## WARN ########################");
            Write(Level.WARN, text);
        }


        public static void Error(string text)
        {
            Write(Level.ERROR, "######################## ERROR ########################");
            Write(Level.ERROR, text);
        }

		public static void LogIf(bool condition, Level level, string text)
		{
			if(condition)
			{
                Write(level, text);
			}
		}

        public static void Write(Level level, string text)
        {
            if (level >= loglevel)
            {
                Console.WriteLine(text);
            }
        }
	}
}
