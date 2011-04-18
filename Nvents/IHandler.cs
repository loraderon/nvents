using System;

namespace Nvents
{
	/// <summary>
	/// Interface for registering event handlers
	/// </summary>
	/// <typeparam name="TEvent">The type of event to handle</typeparam>
	public interface IHandler<TEvent> where TEvent : class, IEvent
	{
		void Handle(TEvent @event);
	}
}
