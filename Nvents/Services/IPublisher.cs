using System;

namespace Nvents.Services
{
	public interface IPublisher
	{
		/// <summary>
		/// Publishes an event to all subscribers of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to publish.</typeparam>
		/// <param name="event">The event to publish.</param>
		void Publish<TEvent>(TEvent @event) where TEvent : class;
	}
}
