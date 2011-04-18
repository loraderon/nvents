using System;
using System.ServiceModel;
using System.Threading;

namespace Nvents.Services.Wcf
{
	[ServiceBehavior(
		InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class WcfEventService : IEventService
	{
		public event EventHandler<EventPublishedEventArgs> EventPublished;

		[EncryptableNetDataContractFormatAttribute]
		public void Publish(IEvent @event)
		{
			if (EventPublished == null)
				return;
			ThreadPool.QueueUserWorkItem(s =>
				EventPublished(null, new EventPublishedEventArgs(@event)));
		}
	}
}
