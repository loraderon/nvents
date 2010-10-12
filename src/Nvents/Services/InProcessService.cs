using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Nvents.Services
{
	public class InProcessService : IService
	{
		List<Handler> handlers = new List<Handler>();

		public void Subscribe<TEvent>(Action<TEvent> action) where TEvent : class, IEvent
		{
			var handler = new Handler();
			handler.SetHandler(action);
			handlers.Add(handler);
		}

		public void Unsubscribe<TEvent>() where TEvent : class, IEvent
		{
			handlers.RemoveAll(x => x.EventType == typeof(TEvent));
		}

		public bool IsStarted { get; private set; }

		public void Publish(IEvent e)
		{
			foreach (var handler in handlers
				.Where(x => x.EventType == e.GetType()))
			{
				ThreadPool.QueueUserWorkItem(s =>
					handler.Action(e));
			}
		}

		public void Start()
		{
			IsStarted = true;
		}

		public void Stop()
		{
			IsStarted = false;
		}

		private class Handler
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
}
