using System;
using System.Threading;
using Xunit;

namespace Nvents.Tests
{
	public class EventsTests : IDisposable
	{
		[Fact]
		public void CanPublishEventsWithoutSetup()
		{
			var raised = false;
			Events.Subscribe<FooEvent>(
				e => raised = true);

			var started = DateTime.Now;
			Events.Publish(new FooEvent());

			var timeout = TimeSpan.FromSeconds(2);
			while (!raised && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.True(raised, "FooEvent was not raised witin the given timeout " + timeout);
		}

		[Fact]
		public void CanReplacePublish()
		{
			var service = new DummyService();
			Events.Service = service;

			Events.Publish(new FooEvent());

			Assert.True(service.StartWasCalled, "Dummy service did not receive a start command.");
			Assert.True(service.PublishWasCalled, "Dummy service did not receive a publish command.");
		}

		[Fact]
		public void CanUseInProcessService()
		{
			Events.Service = new Nvents.Services.InProcessService();

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
		public void CanUnsubscribeEvents()
		{
			var raised = false;
			Events.Subscribe<FooEvent>(
				e => raised = true);

			Events.Unsubscribe<FooEvent>();

			var started = DateTime.Now;
			Events.Publish(new FooEvent());

			var timeout = TimeSpan.FromSeconds(2);
			while (!raised && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.False(raised);
		}

		public EventsTests()
		{
			// reset configuration for each test
			Events.Service = null;
		}

		public void Dispose()
		{
			Events.Service.Stop();
		}
	}
}