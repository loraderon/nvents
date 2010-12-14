using System;
using Nvents;
using Xunit;

namespace Nvents.Tests
{
	public class EventSubscriptionTests
	{
		[Fact]
		public void ShouldRaiseEventPublishedOnPublish()
		{
			var raised = false;
			var fooEvents = new EventSubscription<FooEvent>();
			fooEvents.Published += (s, e) =>
				raised = true;

			Events.Publish(new FooEvent());
			System.Threading.Thread.Sleep(100);

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventPublishedForInheritedEvents()
		{
			var raised = false;
			var fooEvents = new EventSubscription<FooEvent>();
			fooEvents.Published += (s, e) =>
				raised = true;

			Events.Publish(new FooChildEvent());
			System.Threading.Thread.Sleep(100);

			Assert.True(raised);
		}

		[Fact]
		public void ShouldRaiseEventPublishedForInheritedEventSubscription()
		{
			var raised = false;
			var fooEvents = new FooEventSubscription();
			fooEvents.Published += (s, e) =>
				raised = true;

			Events.Publish(new FooEvent());
			System.Threading.Thread.Sleep(100);

			Assert.True(raised);
		}

		public EventSubscriptionTests()
		{
			Events.Service = new Services.InProcessService();
		}

		public class FooEventSubscription : EventSubscription<FooEvent>
		{ }
	}
}
