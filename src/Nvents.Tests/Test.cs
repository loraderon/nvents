using System;
using System.Threading;

namespace Nvents.Tests
{
	public static class Test
	{
		public static void WaitFor(Func<bool> predicate, TimeSpan timeout, Action action)
		{
			var started = DateTime.Now;
			action();
			while (!predicate() && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}
		}
	}
}
