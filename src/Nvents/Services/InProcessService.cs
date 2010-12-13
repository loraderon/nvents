using System;
using System.Linq;
using System.Threading;

namespace Nvents.Services
{
	public class InProcessService : ServiceBase
	{
		public override void Publish(IEvent e)
		{
			foreach (var handler in handlers
				.Where(x => ShouldEventBeHandled(x, e)))
			{
				ThreadPool.QueueUserWorkItem(s =>
					handler.Action(e));
			}
		}
	}
}