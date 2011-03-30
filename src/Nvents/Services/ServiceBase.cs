using System;
using System.Linq;
using System.Collections.Generic;

namespace Nvents.Services
{
	public abstract class ServiceBase : IService
	{
		protected List<EventHandler> handlers = new List<EventHandler>();
		bool startStateIsPending;
		bool autoStart;
		System.Reflection.MethodInfo registerHandler =
			typeof(Events).GetMethods()
			.Where(x => x.Name == "RegisterHandler" && x.IsGenericMethod)
			.Single();

		public ServiceBase(bool autoStart = true)
		{
			this.autoStart = autoStart;
		}

		public void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{
			DetermineAutoStart();
			var handler = new EventHandler();
			handler.SetHandler(action, filter);
			handlers.Add(handler);
		}

		public void Unsubscribe<TEvent>() where TEvent : class, IEvent
		{
			handlers.RemoveAll(x => x.EventType == typeof(TEvent));
		}

		public void RegisterHandler<TEvent>(IHandler<TEvent> handler, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{
			Subscribe<TEvent>(
				e => handler.Handle(e),
				filter);
		}

		/// <summary>
		/// Register an event handler.
		/// </summary>
		/// <param name="handler">The event handler.</param>
		public void RegisterHandler(object handler)
		{
			foreach (var eventType in HandlerUtility.GetHandlerEventTypes(handler))
			{
				registerHandler
					.MakeGenericMethod(new Type[] { eventType })
					.Invoke(null, new object[] { handler, null });
			}
		}

		public virtual void Publish<TEvent>(TEvent e) where TEvent : class, IEvent
		{
			DetermineAutoStart();
			Publish(e);
		}

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

		private void DetermineAutoStart()
		{
			if (!autoStart)
				return;
			if (IsStarted)
				return;
			Start();
		}
	}
}
