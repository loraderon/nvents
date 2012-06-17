using System;

namespace Nvents.Services.Wcf
{
	public class EventPublishedEventArgs : EventArgs
	{
		public readonly object Event;
        public EventPublishedEventArgs(object @event)
		{
			Event = @event;
		}
	}
}
