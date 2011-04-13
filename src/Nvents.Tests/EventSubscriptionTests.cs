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

			Assert.True(raised);
		}

		public EventSubscriptionTests()
		{
			Events.Service = new DummyService();
		}

		public class FooEventSubscription : EventSubscription<FooEvent>
		{ }
	}
}
