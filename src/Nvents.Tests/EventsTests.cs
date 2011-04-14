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

			var started = DateTime.Now;
			Events.Publish(new FooEvent());

			var timeout = TimeSpan.FromSeconds(3);
			while (!raised && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.True(raised, "FooEvent was not raised witin the given timeout " + timeout);
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

			var started = DateTime.Now;
			Events.Publish(new FooEvent());

			var timeout = TimeSpan.FromSeconds(2);
			while (!raised1 && !raised2 && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

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

			var started = DateTime.Now;
			for (int i = 0; i < events; i++)
			{
				ThreadPool.QueueUserWorkItem(_ =>
					Events.Publish(new FooEvent()));
			}

			var timeout = TimeSpan.FromSeconds(15);
			while (raised != events && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.Equal(events, raised);
		}
	}
}