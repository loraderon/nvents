using System;
using Chat.Library;
using Chat.Moderator.Tests.Fakes;
using Chat.Moderator.ViewModels;
using Xunit;

namespace Chat.Moderator.Tests.ViewModels
{
	public class ShellViewModelTests
	{
		[Fact]
		public void ShouldPublishUserKickedMessageWhenKickingUser()
		{
			var userId = Guid.NewGuid();
			vm.KickUser(new MessageSent { Sender = new User { Id = userId } });

			var userKicked = publisher.LastPublishedEvent as UserKicked;

			Assert.NotNull(userKicked);
			Assert.Equal(userId, userKicked.UserId);
		}

		ShellViewModel vm;
		FakePublisher publisher;
		
		public ShellViewModelTests()
		{
			publisher = new FakePublisher();
			vm = new ShellViewModel(publisher);
		}
	}
}