using System;
using Nvents;

namespace Chat.Library
{
	public class UserKicked : IEvent
	{
		public Guid UserId { get; set; }
	}
}
