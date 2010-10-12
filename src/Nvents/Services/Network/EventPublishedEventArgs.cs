using System;

namespace Nvents.Services.Network
{
	public class EventPublishedEventArgs : EventArgs
	{
		public readonly IEvent Event;
		public EventPublishedEventArgs(IEvent @event)
		{
			Event = @event;
		}
	}
}
