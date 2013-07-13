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

		public void Unsubscribe<TEvent>() where TEvent : class
		{ }

		public void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class
		{
			LastEventTypeSubscription = typeof(TEvent);
		}

		public void Publish<TEvent>(TEvent @event) where TEvent : class
		{ }

        public void UnregisterHandler(object handler)
        { }

        public void UnregisterHandler<TEvent>(IHandler<TEvent> handler) where TEvent : class
        { }

        public void RegisterHandler(object handler)
        {
            LastEventTypeSubscription = handler.GetType().GetMethod("Handle").GetParameters()[0].ParameterType;
        }

        public void RegisterHandler<TEvent>(IHandler<TEvent> handler, Func<TEvent, bool> filter = null) where TEvent : class
        {
            LastEventTypeSubscription = typeof(TEvent);
        }

        public event EventHandler<PublishErrorEventArgs> PublishError;
    }
}
