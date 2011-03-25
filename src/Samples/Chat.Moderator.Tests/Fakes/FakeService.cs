using System;
using Nvents;
using Nvents.Services;

namespace Chat.Moderator.Tests.Fakes
{
	public class FakeService : IService
	{
		public Type LastEventTypeSubscription { get; private set; }

		public bool IsStarted
		{
			get { return true; }
		}

		public void Start()
		{ }

		public void Stop()
		{ }

		public void Unsubscribe<TEvent>() where TEvent : class, IEvent
		{ }

		public void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{
			LastEventTypeSubscription = typeof(TEvent);
		}

		public void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
		{ }
	}
}
