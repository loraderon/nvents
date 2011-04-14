using System;
using System.Threading;
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

			var started = DateTime.Now;
			Events.Publish(new FooEvent());

			var timeout = TimeSpan.FromSeconds(1);
			while (!raised && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.True(raised, "FooEvent was not raised witin the given timeout " + timeout);
		}

		public InProcessServiceTests()
		{
			Events.Service = new InProcessService();
		}
	}
}
