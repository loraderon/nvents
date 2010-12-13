using System;
using System.Collections.Generic;

namespace Nvents.Services
{
	public abstract class ServiceBase : IService
	{
		protected List<EventHandler> handlers = new List<EventHandler>();

		public void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{
			var handler = new EventHandler();
			handler.SetHandler(action, filter);
			handlers.Add(handler);
		}

		public void Unsubscribe<TEvent>() where TEvent : class, IEvent
		{
			handlers.RemoveAll(x => x.EventType == typeof(TEvent));
		}

		public abstract void Publish(IEvent e);

		protected virtual void OnStart() { }

		protected virtual void OnStop() { }

		public bool IsStarted { get; private set; }

		public void Start()
		{
			OnStart();
			IsStarted = true;
		}

		public void Stop()
		{
			OnStop();
			IsStarted = false;
		}

		protected bool ShouldEventBeHandled(EventHandler handler, IEvent e)
		{
			var eventType = e.GetType();
			return handler.EventType == eventType
				|| eventType.IsSubclassOf(handler.EventType)
				|| eventType.GetInterface(handler.EventType.Name) == handler.EventType;
		}
	}
}
