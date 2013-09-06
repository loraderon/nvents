using System;

namespace Nvents.Services
{
	public interface IService : ISubscriber, IPublisher
	{
		/// <summary>
		/// Unsubscribes all events of the specified type
		/// </summary>
		/// <typeparam name="TEvent">The type of event to unsubscribe</typeparam>
		void Unsubscribe<TEvent>()
				where TEvent : class;

        /// <summary>
        /// Unsubscribes the specifed Action from all events of the specified type
        /// </summary>
        /// <typeparam name="TEvent">The type of event to unsubscribe</typeparam>
        void Unsubscribe<TEvent>(Action<TEvent> action)
                where TEvent : class;
		
		/// <summary>
		/// Unregisters an event handler
		/// </summary>
		/// <typeparam name="TEvent">The event type for the event handler</typeparam>
		/// <param name="handler">The event handler to unregister</param>
		void UnregisterHandler<TEvent>(IHandler<TEvent> handler)
				where TEvent : class;

		/// <summary>
		/// Unregisters an event handler
		/// </summary>
		/// <param name="handler">The event handler to unregister</param>
		void UnregisterHandler(object handler);
		
		/// <summary>
		/// Get the service running state
		/// </summary>
		bool IsStarted { get; }
		
		/// <summary>
		/// Starts the service
		/// </summary>
		void Start();
		
		/// <summary>
		/// Stops the service
		/// </summary>
		void Stop();

		event EventHandler<PublishErrorEventArgs> PublishError;
	}
}