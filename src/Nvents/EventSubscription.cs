using System;
using Nvents.Services;

namespace Nvents
{
	public class EventSubscription<TEvent> where TEvent : class, IEvent
	{
		public event EventHandler<PublishedEventArgs> Published;

		public EventSubscription(Func<TEvent, bool> filter = null)
			: this(Events.Service, filter)
		{
			if (!Events.Service.IsStarted)
				Events.Service.Start();
		}

		public EventSubscription(ISubscriber subscriber, Func<TEvent, bool> filter = null)
		{
			subscriber.Subscribe<TEvent>(
				e => OnPublished(e),
				filter);
		}

		protected virtual void OnPublished(TEvent @event)
		{
			if (Published != null)
				Published(this, new PublishedEventArgs(@event));
		}

		public class PublishedEventArgs : EventArgs
		{
			public readonly TEvent Event;

			public PublishedEventArgs(TEvent @event)
			{
				Event = @event;
			}
		}
	}
}
