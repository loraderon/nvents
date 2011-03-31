using System;

namespace Nvents.Services
{
	public interface IService : ISubscriber, IPublisher
	{
		void Unsubscribe<TEvent>()
				where TEvent : class, IEvent;
		void UnregisterHandler<TEvent>(IHandler<TEvent> handler)
				where TEvent : class, IEvent;
		void UnregisterHandler(object handler);
		bool IsStarted { get; }
		void Start();
		void Stop();
	}
}