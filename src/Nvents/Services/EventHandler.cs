using System;

namespace Nvents.Services
{
	internal class EventHandler
	{
		public void SetHandler<TEvent>(Action<TEvent> action) where TEvent : class, IEvent
		{
			Action = e =>
			{
				action(e as TEvent);
			};
			EventType = typeof(TEvent);
		}
		public Type EventType { get; private set; }
		public Action<IEvent> Action { get; private set; }
	}
}
