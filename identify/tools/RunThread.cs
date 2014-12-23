using System;
using System.Threading;

namespace PokerBot
{
	public delegate void ParameterizedThreadStart<T>(T value);

	public static class RunThread
	{
		public static Thread Start<T>(ParameterizedThreadStart<T> start, T value)
		{
			if (start == null)
				throw new ArgumentNullException("start");

			Thread thread = new Thread(delegate()
			{
				start(value);
			});
			thread.Start();
			return thread;
		}
	}
}
