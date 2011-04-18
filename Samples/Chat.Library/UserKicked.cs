using System;
using Nvents;

namespace Chat.Library
{
	/// <summary>
	/// Event for when a user is kicked
	/// </summary>
	public class UserKicked : IEvent
	{
		/// <summary>
		/// The id of the user being kicked
		/// </summary>
		public Guid UserId { get; set; }
	}
}
