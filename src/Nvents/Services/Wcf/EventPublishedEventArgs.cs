using System;

namespace Nvents.Services.Wcf
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
