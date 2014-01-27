using System;

namespace Nvents.Services
{
	/// <summary>
	/// Internal event registration for storing subscriptions and event handlers
	/// </summary>
	public class EventRegistration
	{
		public void SetHandler<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter) where TEvent : class
		{
		    SourceAction = action;
			Action = e =>
			{
				var @event = e as TEvent;
				if (filter != null && !filter(@event))
					return;
				action(@event);
			};
			EventType = typeof(TEvent);
		}

		public void SetHandler<TEvent>(IHandler<TEvent> handler, Func<TEvent, bool> filter) where TEvent : class
		{
			Action = e =>
			{
				var @event = e as TEvent;
				if (filter != null && !filter(@event))
					return;
				handler.Handle(@event);
			};
            EventType = typeof(TEvent);
            Handler = handler;
		}

		public Type EventType { get; private set; }
		public Action<object> Action { get; private set; }
        public object SourceAction { get; private set; }
        public object Handler { get; private set;}
	}
}
