using System;

namespace Nvents
{
	public class PublishErrorEventArgs : EventArgs
	{
		public object Event { get; private set; }
		public string Destination { get; private set; }
		public Exception PublishException { get; private set; }

		public PublishErrorEventArgs(object nvent, string destination, Exception exception)
		{
			Event = nvent;
			Destination = destination;
			PublishException = exception;
		}
	}
}
