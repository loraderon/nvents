using System;
using System.Collections.Generic;

namespace Nvents.Services
{
	public abstract class ServiceBase : IService
	{
		protected List<EventHandler> handlers = new List<EventHandler>();
		bool startStateIsPending;

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

		public abstract void Publish<TEvent>(TEvent e) where TEvent : class, IEvent;

		protected virtual void OnStart() { }

		protected virtual void OnStop() { }

		private bool isStarted;
		public bool IsStarted
		{
			get
			{
				while (startStateIsPending) ;
				return isStarted;
			}
		}

		public void Start()
		{
			startStateIsPending = true;
			try
			{
				OnStart();
				isStarted = true;
			}
			finally
			{
				startStateIsPending = false;
			}			
		}

		public void Stop()
		{
			startStateIsPending = true;
			try
			{
				OnStop();
				isStarted = false;
			}
			finally
			{
				startStateIsPending = false;
			}	
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
