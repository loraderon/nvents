using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Nvents.Services;

namespace Nvents
{
	public static class Events
	{
		static IService service;

		/// <summary>
		/// Publishes an event to all subscribers of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to publish.</typeparam>
		/// <param name="event">The event to publish.</param>
		public static void Publish<TEvent>(TEvent @event) where TEvent : class
		{
			Service.Publish(@event);
		}

		/// <summary>
		/// Subscribes to all events of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to subscribe for.</typeparam>
		/// <param name="action">Action to perfom when event is published.</param>
		/// <param name="filter">Optional filter action.</param>
		public static void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class
		{
			Service.Subscribe(action, filter);
		}

		/// <summary>
		/// Unsubscribes from all events of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to unsubscribe for.</typeparam>
		public static void Unsubscribe<TEvent>() where TEvent : class
		{
			Service.Unsubscribe<TEvent>();
		}

		/// <summary>
		/// Register an event handler.
		/// </summary>
		/// <typeparam name="TEvent">The event type for the handler.</typeparam>
		/// <param name="handler">The event handler.</param>
		/// <param name="filter">Optional filter action.</param>
		public static void RegisterHandler<TEvent>(IHandler<TEvent> handler, Func<TEvent, bool> filter = null) where TEvent : class
		{
			Service.RegisterHandler<TEvent>(handler, filter);
		}

		/// <summary>
		/// Register an event handler.
		/// </summary>
		/// <param name="handler">The event handler.</param>
		public static void RegisterHandler(object handler)
		{
			Service.RegisterHandler(handler);
		}

		/// <summary>
		/// Unregisters an event handler
		/// </summary>
		/// <typeparam name="TEvent">The event type for tha handler</typeparam>
		/// <param name="handler">The event handler</param>
		public static void UnregisterHandler<TEvent>(IHandler<TEvent> handler) where TEvent : class
		{
			Service.UnregisterHandler<TEvent>(handler);
		}

		/// <summary>
		/// Unregisters an event handler
		/// </summary>
		/// <param name="handler">The event handler</param>
		public static void UnregisterHandler(object handler)
		{
			Service.UnregisterHandler(handler);
		}

		/// <summary>
		/// Registers handlers from assemblies.
		/// </summary>
		/// <param name="assemblies">The assemblies to scan for handlers.</param>
		public static void RegisterHandlers(params Assembly[] assemblies)
		{
			if (assemblies == null)
				return;
			var handlers = assemblies
				.Where(a => a != null)
				.SelectMany(assembly => 
					assembly.GetTypes()
						.Where(type => IsAssignableToGenericType(type, typeof(IHandler<>))))
						.Where(type => type.IsClass)
						.Where(type => !type.IsAbstract)
						.Where(type => type.GetConstructor(Type.EmptyTypes) != null)
						.Select(Activator.CreateInstance);
			foreach (var handler in handlers)
			{
				RegisterHandler(handler);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IService Service
		{
			get
			{
				if (service == null)
					service = new NetworkService();
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

		private static bool IsAssignableToGenericType(Type givenType, Type genericType)
		{
			var interfaceTypes = givenType.GetInterfaces();

			if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
			{
				return true;
			}

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
				return true;

			var baseType = givenType.BaseType;
			if (baseType == null)
				return false;

			return IsAssignableToGenericType(baseType, genericType);
		}
	}
}
