using System;

namespace Nvents.Services
{
	public class EventHandler
	{
		public void SetHandler<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter) where TEvent : class, IEvent
		{
			Action = e =>
			{
				var @event = e as TEvent;
				if (filter != null && !filter(@event))
					return;
				action(@event);
			};
			EventType = typeof(TEvent);
		}

		public void SetHandler<TEvent>(IHandler<TEvent> handler, Func<TEvent, bool> filter) where TEvent : class, IEvent
		{
			Action = e =>
			{
				var @event = e as TEvent;
				if (filter != null && !filter(@event))
					return;
				handler.Handle(@event);
			};
			EventType = handler.GetType();
		}

		public Type EventType { get; private set; }
		public Action<IEvent> Action { get; private set; }
	}
}
