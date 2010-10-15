using System;

namespace Nvents.Services
{
	public interface IService
	{
		void Subscribe<TEvent>(Action<TEvent> action)
				where TEvent : class, IEvent;
		void Unsubscribe<TEvent>()
				where TEvent : class, IEvent;
		bool IsStarted { get; }
		void Publish(IEvent e);
		void Start();
		void Stop();
	}
}