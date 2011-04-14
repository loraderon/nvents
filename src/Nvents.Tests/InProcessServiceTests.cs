using System;
using Nvents.Services;
using Xunit;

namespace Nvents.Tests
{
	public class InProcessServiceTests
	{
		[Fact]
		public void CanPublishEvent()
		{
			var raised = false;
			Events.Subscribe<FooEvent>(
				e => raised = true);

			Test.WaitFor(() => raised, TimeSpan.FromSeconds(1), () =>
				Events.Publish(new FooEvent()));

			Assert.True(raised, "FooEvent was not raised");
		}

		public InProcessServiceTests()
		{
			Events.Service = new InProcessService();
		}
	}
}
