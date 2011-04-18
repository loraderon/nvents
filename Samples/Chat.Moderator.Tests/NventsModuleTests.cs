using System;
using Chat.Library;
using Chat.Moderator.Handlers;
using Chat.Moderator.Tests.Fakes;
using Ninject;
using Nvents;
using Nvents.Services;
using Xunit;

namespace Chat.Moderator.Tests
{
	public class NventsModuleTests
	{
		[Fact]
		public void ShouldRegisterMessageSentHandler()
		{
			var type = service.LastEventTypeSubscription;

			Assert.NotNull(type);
			Assert.Equal(typeof(MessageSent), type);			
		}

		[Fact]
		public void CanResolveMessageSentHandler()
		{
			var handler = kernel.Get<MessageSentHandler>();

			Assert.NotNull(handler);
		}

		[Fact]
		public void CanResolvePublisher()
		{
			var publisher = kernel.Get<IPublisher>();

			Assert.NotNull(publisher);
		}

		IKernel kernel;
		FakeService service;

		public NventsModuleTests()
		{
			service = new FakeService();
			Events.Service = service;
			kernel = new StandardKernel(new NventsModule());
		}
	}
}
