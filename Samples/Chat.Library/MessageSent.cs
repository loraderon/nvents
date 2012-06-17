using System;

namespace Chat.Library
{
	/// <summary>
	/// Event for when a message is sent
	/// </summary>
	public class MessageSent
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