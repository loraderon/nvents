using System;
using System.Linq;
using System.Threading;

namespace Nvents.Services
{
	public class InMemoryService : ServiceBase
	{
		public override void Publish<TEvent>(TEvent e)
		{
			foreach (var handler in handlers
				.Where(x => ShouldEventBeHandled(x, e)))
			{
				ThreadPool.QueueUserWorkItem(s =>
					((EventHandler)s).Action(e), handler);
			}
		}
	}
}