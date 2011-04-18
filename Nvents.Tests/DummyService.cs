using System;
using System.Collections.Generic;
using System.Linq;
using Nvents.Services;

namespace Nvents.Tests
{
	public class DummyService : ServiceBase
	{
		public override void Publish<TEvent>(TEvent e)
		{
			publishedEvents.Add(e);
			foreach (var registration in registrations
				.Where(x => ShouldEventBeHandled(x, e)))
			{
				registration.Action(e);
			}
		}

		private List<IEvent> publishedEvents = new List<IEvent>();
		public IEvent[] PublishedEvents
		{
			get
			{
				return publishedEvents.ToArray();
			}
		}
	}
}
