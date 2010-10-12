using System;
using System.ServiceModel;

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
			if (EventPublished != null)
				EventPublished(this, new EventPublishedEventArgs(@event));
		}
	}
}
