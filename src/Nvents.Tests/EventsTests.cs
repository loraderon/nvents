﻿using System;
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

			var timeout = TimeSpan.FromSeconds(3);
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

		[Fact]
		public void CanRegisterHandler()
		{
			var handler = new DummyHandler();
			Events.RegisterHandler(handler);

			var started = DateTime.Now;
			Events.Publish(new FooEvent());

			var timeout = TimeSpan.FromSeconds(2);
			while (handler.HandledEvent == null && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.NotNull(handler.HandledEvent);
		}

		[Fact]
		public void CanFilterEvents()
		{
			int raised1 = 0;
			Events.Subscribe<FooEvent>(
				e => raised1++);

			int raised2 = 0;
			Events.Subscribe<FooEvent>(
				e => raised2++,
				e => e.Baz == "Filtered");

			var started = DateTime.Now;
			Events.Publish(new FooEvent());
			Events.Publish(new FooEvent { Baz = "Filtered" });

			var timeout = TimeSpan.FromSeconds(3);
			while ((DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.Equal(2, raised1);
			Assert.Equal(1, raised2);
		}

		[Fact]
		public void CanPublishEncryptedEvents()
		{
			Events.Service = new Nvents.Services.AutoNetworkService("encryption-key");
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
		public void CanPublishMultipleEvents()
		{
			var events = 3;

			var raised = 0;
			Events.Subscribe<FooEvent>(
				e => raised++);

			var started = DateTime.Now;
			for (int i = 0; i < events; i++)
			{
				Events.Publish(new FooEvent());
			}

			var timeout = TimeSpan.FromSeconds(3);
			while (raised != events && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.Equal(events, raised);
		}

		[Fact]
		public void CanPublishMultipleEventsInParalell()
		{
			var events = 10;

			var raised = 0;
			Events.Subscribe<FooEvent>(
				e => raised++);

			var started = DateTime.Now;
			System.Threading.Tasks.Parallel.For(0, events, i =>
			{
				Events.Publish(new FooEvent());
			});

			var timeout = TimeSpan.FromSeconds(5);
			while (raised != events && (DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.Equal(events, raised);
		}

		[Fact]
		public void CanSubscribeToInheritedEvents()
		{
			Events.Service = new Services.InProcessService();
			var foos = 0;
			var fooChilds = 0;

			Events.Subscribe<FooEvent>(e => foos++);
			Events.Subscribe<FooChildEvent>(e => fooChilds++);

			var started = DateTime.Now;
			Events.Publish(new FooEvent());
			Events.Publish(new FooChildEvent());

			var timeout = TimeSpan.FromMilliseconds(200);
			while ((DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.Equal(2, foos);
			Assert.Equal(1, fooChilds);
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