using System;

namespace Nvents.Services
{
	public interface IService : ISubscriber, IPublisher
	{
		void Unsubscribe<TEvent>()
				where TEvent : class, IEvent;
		bool IsStarted { get; }
		void Start();
		void Stop();
	}
}