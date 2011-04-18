using System;
using System.Linq;
using System.Threading;

namespace Nvents.Services
{
	/// <summary>
	/// In memory service for publishing/subscribing to events in the same process
	/// </summary>
	public class InMemoryService : ServiceBase
	{
		/// <summary>
		/// Publishes an event to all subscribers of type TEvent.
		/// </summary>
		/// <typeparam name="TEvent">The event type to publish.</typeparam>
		/// <param name="e">The event to publish.</param>
		public override void Publish<TEvent>(TEvent e)
		{
			foreach (var registration in registrations
				.Where(x => ShouldEventBeHandled(x, e)))
			{
				ThreadPool.QueueUserWorkItem(s =>
					((EventRegistration)s).Action(e), registration);
			}
		}
	}
}