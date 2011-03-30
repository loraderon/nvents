using System;
using Nvents.Services;

namespace Nvents.Tests
{
	public class DummyService : IService
	{
		public void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{ }

		public void Unsubscribe<TEvent>() where TEvent : class, IEvent
		{ }

		public bool IsStarted { get; private set; }

		public void Publish<TEvent>(TEvent e) where TEvent : class, IEvent
		{
			PublishWasCalled = true;
		}
		public bool PublishWasCalled { get; private set; }

		public void Start()
		{ }

		public void Stop()
		{ }
	}
}
