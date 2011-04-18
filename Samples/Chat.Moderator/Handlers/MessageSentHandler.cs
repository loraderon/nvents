using System;
using Chat.Library;
using Chat.Moderator.ViewModels;
using Nvents;

namespace Chat.Moderator.Handlers
{
	public class MessageSentHandler : IHandler<MessageSent>	
	{
		ShellViewModel vm;
		
		public MessageSentHandler(ShellViewModel vm)
		{
			this.vm = vm;
		}

		public void Handle(MessageSent @event)
		{
			vm.Messages.Add(@event);
		}
	}
}
