using System;
using Nvents;

namespace Chat.Library
{
	public class MessageSent : IEvent
	{
		public string Message { get; set; }
		public User Sender { get; set; }
	}
}