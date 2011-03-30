using System;
using System.Linq;
using System.ComponentModel;
using Nvents.Services;

namespace Nvents
{
	public static class Events
	{
		static IService service;
		static System.Reflection.MethodInfo registerHandler = 
			typeof(Events).GetMethods()
			.Where(x => x.Name == "RegisterHandler" && x.IsGenericMethod)
			.Single();

		/// <summary>
		/// Publishes an event to all subscribers of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to publish.</typeparam>
		/// <param name="event">The event to publish.</param>
		public static void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
		{
			Service.Publish(@event);
		}

		/// <summary>
		/// Subscribes to all events of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to subscribe for.</typeparam>
		/// <param name="action">Action to perfom when event is published.</param>
		/// <param name="filter">Optional filter action.</param>
		public static void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{
			Service.Subscribe(action, filter);
		}

		/// <summary>
		/// Unsubscribes from all events of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to unsubscribe for.</typeparam>
		public static void Unsubscribe<TEvent>() where TEvent : class, IEvent
		{
			Service.Unsubscribe<TEvent>();
		}

		/// <summary>
		/// Register an event handler.
		/// </summary>
		/// <typeparam name="TEvent">The event type for the handler.</typeparam>
		/// <param name="handler">The event handler.</param>
		/// <param name="filter">Optional filter action.</param>
		public static void RegisterHandler<TEvent>(IHandler<TEvent> handler, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{
			Subscribe<TEvent>(
				e => handler.Handle(e), 
				filter);
		}

		/// <summary>
		/// Register an event handler.
		/// </summary>
		/// <param name="handler">The event handler.</param>
		public static void RegisterHandler(object handler)
		{
			foreach (var eventType in HandlerUtility.GetHandlerEventTypes(handler))
			{
				registerHandler
					.MakeGenericMethod(new Type[] { eventType })
					.Invoke(null, new object[] { handler, null });
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IService Service
		{
			get
			{
				if (service == null)
					service = new AutoNetworkService();
				return service;
			}
			set
			{
				if (service != null && value != service)
					service.Stop();
				service = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object objA, object objB)
		{
			return object.Equals(objA, objB);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool ReferenceEquals(object objA, object objB)
		{
			return object.ReferenceEquals(objA, objB);
		}
	}
}
