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
		System.Reflection.MethodInfo registerHandler;
		System.Reflection.MethodInfo unregisterHandler;

		public ServiceBase(bool autoStart = true)
		{
			this.autoStart = autoStart;

			registerHandler =
				GetType().GetMethods()
				.Where(x => x.Name == "RegisterHandler" && x.IsGenericMethod)
				.Single();
			unregisterHandler =
				GetType().GetMethods()
				.Where(x => x.Name == "UnregisterHandler" && x.IsGenericMethod)
				.Single();
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
			DetermineAutoStart();
			var internalHandler = new EventHandler();
			internalHandler.SetHandler(handler, filter);
			handlers.Add(internalHandler);
		}

		public void RegisterHandler(object handler)
		{
			foreach (var eventType in HandlerUtility.GetHandlerEventTypes(handler))
			{
				registerHandler
					.MakeGenericMethod(new Type[] { eventType })
					.Invoke(this, new object[] { handler, null });
			}
		}

		public void UnregisterHandler<TEvent>(IHandler<TEvent> handler) where TEvent : class, IEvent
		{
			handlers.RemoveAll(x => x.EventType == handler.GetType());
		}

		public void UnregisterHandler(object handler)
		{
			foreach (var eventType in HandlerUtility.GetHandlerEventTypes(handler))
			{
				unregisterHandler
					.MakeGenericMethod(new Type[] { eventType })
					.Invoke(this, new object[] { handler });
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
			var handlerEventType = handler.EventType;

			var handlerInterfaces = handlerEventType
				.GetInterfaces()
				.Where(x => x.Name == typeof(IHandler<>).Name);

			if (handlerInterfaces.Count() > 0)
			{
				if(handlerInterfaces
					.Any(x => x.GetGenericArguments().FirstOrDefault() == eventType))
				{
					handlerEventType = eventType;
				}
			}

			return handlerEventType == eventType
				|| eventType.IsSubclassOf(handlerEventType)
				|| eventType.GetInterface(handlerEventType.Name) == handlerEventType;
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