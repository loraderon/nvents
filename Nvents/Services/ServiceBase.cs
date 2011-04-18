using System;
using System.Linq;
using System.Collections.Generic;

namespace Nvents.Services
{
	/// <summary>
	/// Base implementation of a service
	/// </summary>
	public abstract class ServiceBase : IService
	{
		protected List<EventRegistration> registrations = new List<EventRegistration>();
		bool startStateIsPending;
		bool autoStart;
		System.Reflection.MethodInfo registerHandler;
		System.Reflection.MethodInfo unregisterHandler;

		public ServiceBase(bool autoStart = true)
		{
			this.autoStart = autoStart;

			// gets the generic methods for registering and unregistering event handlers
			registerHandler =
				GetType().GetMethods()
				.Where(x => x.Name == "RegisterHandler" && x.IsGenericMethod)
				.Single();
			unregisterHandler =
				GetType().GetMethods()
				.Where(x => x.Name == "UnregisterHandler" && x.IsGenericMethod)
				.Single();
		}

		/// <summary>
		/// Subscribes to all events of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to subscribe for.</typeparam>
		/// <param name="action">Action to perfom when event is published.</param>
		/// <param name="filter">Optional filter action.</param>
		public void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{
			DetermineAutoStart();
			var registration = new EventRegistration();
			registration.SetHandler(action, filter);
			registrations.Add(registration);
		}

		/// <summary>
		/// Unsubscribes all events of the specified type
		/// </summary>
		/// <typeparam name="TEvent">The type of event to unsubscribe</typeparam>
		public void Unsubscribe<TEvent>() where TEvent : class, IEvent
		{
			registrations.RemoveAll(x => x.EventType == typeof(TEvent));
		}

		/// <summary>
		/// Register an event handler.
		/// </summary>
		/// <param name="handler">The event handler.</param>		
		/// <param name="filter">Optional filter action.</param>
		public void RegisterHandler<TEvent>(IHandler<TEvent> handler, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{
			DetermineAutoStart();
			var registration = new EventRegistration();
			registration.SetHandler(handler, filter);
			registrations.Add(registration);
		}

		/// <summary>
		/// Register an event handler.
		/// </summary>
		/// <param name="handler">The event handler.</param>
		public void RegisterHandler(object handler)
		{
			// get all the event types that this handler can handle
			foreach (var eventType in HandlerUtility.GetHandlerEventTypes(handler))
			{
				// register the event handler for the specified event type using the strongly typed RegisterHandler
				registerHandler
					.MakeGenericMethod(new Type[] { eventType })
					.Invoke(this, new object[] { handler, null });
			}
		}

		/// <summary>
		/// Unregisters an event handler
		/// </summary>
		/// <typeparam name="TEvent">The event type for the event handler</typeparam>
		/// <param name="handler">The event handler to unregister</param>
		public void UnregisterHandler<TEvent>(IHandler<TEvent> handler) where TEvent : class, IEvent
		{
			registrations.RemoveAll(x => x.EventType == handler.GetType());
		}

		/// <summary>
		/// Unregisters an event handler
		/// </summary>
		/// <param name="handler">The event handler to unregister</param>
		public void UnregisterHandler(object handler)
		{
			// get all the event types that this handler can handle
			foreach (var eventType in HandlerUtility.GetHandlerEventTypes(handler))
			{
				// unregister the event handler for the specified event type using the strongly typed UnregisterHandler
				unregisterHandler
					.MakeGenericMethod(new Type[] { eventType })
					.Invoke(this, new object[] { handler });
			}
		}

		/// <summary>
		/// Publishes an event to all subscribers of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to publish.</typeparam>
		/// <param name="e">The event to publish.</param>
		public virtual void Publish<TEvent>(TEvent e) where TEvent : class, IEvent
		{
			DetermineAutoStart();
			Publish(e);
		}

		protected virtual void OnStart() { }

		protected virtual void OnStop() { }

		private bool isStarted;
		/// <summary>
		/// Get the service running state
		/// </summary>
		public bool IsStarted
		{
			get
			{
				while (startStateIsPending) ;
				return isStarted;
			}
		}

		/// <summary>
		/// Starts the service
		/// </summary>
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

		/// <summary>
		/// Stops the service
		/// </summary>
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

		/// <summary>
		/// Determines whether a published event should be handled by the specified event handler
		/// </summary>
		/// <param name="registration">The internal event registration</param>
		/// <param name="e">The event that was published</param>
		/// <returns>True if the event should be handled</returns>
		protected bool ShouldEventBeHandled(EventRegistration registration, IEvent e)
		{
			var eventType = e.GetType();
			var registrationEventType = registration.EventType;

			var registrationInterfaces = registrationEventType
				.GetInterfaces()
				.Where(x => x.Name == typeof(IHandler<>).Name);

			if (registrationInterfaces.Count() > 0)
			{
				if(registrationInterfaces
					.Any(x => x.GetGenericArguments().FirstOrDefault() == eventType))
				{
					registrationEventType = eventType;
				}
			}

			return registrationEventType == eventType
				|| eventType.IsSubclassOf(registrationEventType)
				|| eventType.GetInterface(registrationEventType.Name) == registrationEventType;
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
