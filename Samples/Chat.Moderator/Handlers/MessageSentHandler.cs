using System;
using Chat.Library;
using Chat.Moderator.ViewModels;
using Nvents;

namespace Chat.Moderator.Handlers
{
	/// <summary>
	/// Handles MessageSent events
	/// </summary>
	public class MessageSentHandler : IHandler<MessageSent>	
	{
		ShellViewModel vm;
		
		public MessageSentHandler(ShellViewModel vm)
		{
			// vm is injected by ninject
			this.vm = vm;
		}

		public void Handle(MessageSent @event)
		{
			// Add message to the viewmodel when MessageSent events are handled
			vm.Messages.Add(@event);
		}
	}
}
