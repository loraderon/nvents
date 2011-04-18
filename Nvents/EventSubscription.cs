using System;
using Nvents.Services;

namespace Nvents
{
	/// <summary>
	/// Wraps subscriptions using standard event handling
	/// </summary>
	/// <typeparam name="TEvent">The type of event to handle</typeparam>
	public class EventSubscription<TEvent> where TEvent : class, IEvent
	{
		/// <summary>
		/// The event that will be raised when an corresponding event is published
		/// </summary>
		public event EventHandler<PublishedEventArgs> Published;

		/// <summary>
		/// Wraps subsciptions from a the default subscriber using standard event handling 
		/// </summary>
		/// <param name="filter">An optional filter action for filtering the events</param>
		public EventSubscription(Func<TEvent, bool> filter = null)
			: this(Events.Service, filter)
		{ }

		/// <summary>
		/// Wraps subsciptions from a subscriber using standard event handling 
		/// </summary>
		/// <param name="subscriber">The subscriber to subscribe to.</param>
		/// <param name="filter">An optional filter action for filtering the events</param>
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
			/// <summary>
			/// The event that was published
			/// </summary>
			public readonly TEvent Event;

			public PublishedEventArgs(TEvent @event)
			{
				Event = @event;
			}
		}
	}
}
