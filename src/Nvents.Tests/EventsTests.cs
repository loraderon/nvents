using System;
using System.Threading;
using Xunit;

namespace Nvents.Tests
{
	public class EventsTests
	{
		[Fact]
		public void CanPublishEventsWithoutSetup()
		{
			var raised = false;
			Events.Subscribe<FooEvent>(
				e => raised = true);

			Test.WaitFor(() => raised, TimeSpan.FromSeconds(3), () =>
				Events.Publish(new FooEvent()));

			Assert.True(raised, "FooEvent was not raised");
		}

		[Fact]
		public void ShouldRaiseOtherSubscriptionsWhenOneFails()
		{
			var raised1 = false;
			Events.Subscribe<FooEvent>(e =>
			{
				raised1 = true;
				throw new NotSupportedException("Custom error");
			});

			var raised2 = false;
			Events.Subscribe<FooEvent>(
				e => raised2 = true);

			Test.WaitFor(() => raised1 && raised2, TimeSpan.FromSeconds(2), () =>
				Events.Publish(new FooEvent()));

			Assert.True(raised1, "First subscription was not raised.");
			Assert.True(raised2, "Second subscription was not raised.");
		}

		[Fact]
		public void CanPublishMultipleEventsInParalell()
		{
			var events = 10;

			var raised = 0;
			Events.Subscribe<FooEvent>(
				e => raised++);

			Test.WaitFor(() => raised == events, TimeSpan.FromSeconds(15), () =>
			{
				for (int i = 0; i < events; i++)
				{
					ThreadPool.QueueUserWorkItem(_ =>
						Events.Publish(new FooEvent()));
				}
			});
			 
			Assert.Equal(events, raised);
		}
	}
}