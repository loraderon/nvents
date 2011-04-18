using System;

namespace Nvents
{
	public interface IHandler<TEvent> where TEvent : class, IEvent
	{
		void Handle(TEvent @event);
	}
}
