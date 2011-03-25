using System;
using Chat.Library;
using Chat.Moderator.Handlers;
using Chat.Moderator.ViewModels;
using Xunit;

namespace Chat.Moderator.Tests.Handlers
{
	public class MessageSentHandlerTests
	{
		[Fact]
		public void ShouldAddMessageToViewModelWhenEventIsHandled()
		{
			handler.Handle(new MessageSent { Message = "message" });

			Assert.Equal(1, vm.Messages.Count);
			Assert.Equal("message", vm.Messages[0].Message);
		}

		MessageSentHandler handler;
		ShellViewModel vm;

		public MessageSentHandlerTests()
		{
			vm = new ShellViewModel(null);
			handler = new MessageSentHandler(vm);
		}
	}
}
