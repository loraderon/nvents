using System;
using Nvents;

namespace Chat.Library
{
	/// <summary>
	/// Event for when a message is sent
	/// </summary>
	public class MessageSent : IEvent
	{
		/// <summary>
		/// The text of the message
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The user that sent the message
		/// </summary>
		public User Sender { get; set; }
	}
}