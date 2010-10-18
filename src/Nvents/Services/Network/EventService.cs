using System;
using System.ServiceModel;
using System.Threading;

namespace Nvents.Services.Network
{
	[ServiceBehavior(
		InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class EventService : IEventService
	{
		public event EventHandler<EventPublishedEventArgs> EventPublished;

		public void Publish(IEvent @event)
		{
			if (EventPublished == null)
				return;
			ThreadPool.QueueUserWorkItem(s =>
				EventPublished(null, new EventPublishedEventArgs(@event)));
		}
	}
}
