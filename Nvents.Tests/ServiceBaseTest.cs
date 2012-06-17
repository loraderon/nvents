using System;
using Xunit;

namespace Nvents.Tests
{
	public class ServiceBaseTest
	{
		[Fact]
		public void CanReplaceService()
		{
			Events.Publish(new FooEvent());

			Assert.Equal(1, service.PublishedEvents.Length);
		}

		[Fact]
		public void CanUnsubscribeEvents()
		{
			var raised = false;
			Events.Subscribe<FooEvent>(
				e => raised = true);

			Events.Unsubscribe<FooEvent>();

			Events.Publish(new FooEvent());

			Assert.False(raised);
		}

		[Fact]
		public void CanRegisterHandler()
		{
			var handler = new DummyHandler();
			Events.RegisterHandler(handler);

			Events.Publish(new FooEvent());

			Assert.NotNull(handler.HandledEvent);
		}

		[Fact]
		public void CanRegisterHandlerWithoutGenerics()
		{
			var handler = new DummyHandler();
			var handlerWithoutGenerics = (object)handler;
			Events.RegisterHandler(handlerWithoutGenerics);

			Events.Publish(new FooEvent());

			Assert.NotNull(handler.HandledEvent);
		}

		[Fact]
		public void CanRegisterHandlerThatHandlesMultipleEvents()
		{
			var handler = new FooBarHandler();
			Events.RegisterHandler(handler);

			Events.Publish(new FooEvent());
			Events.Publish(new BarEvent());

			Assert.True(handler.FooHandled, "FooEvent was not handled");
			Assert.True(handler.BarHandled, "BarEvent was not handled");
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

			Events.Publish(new FooEvent());
			Events.Publish(new FooEvent { Baz = "Filtered" });

			Assert.Equal(2, raised1);
			Assert.Equal(1, raised2);
		}

		[Fact]
		public void CanSubscribeToInheritedEvents()
		{
			var numberOfEvents = 0;
			var numberOfFooEvents = 0;
			var numberOfFooChildEvents = 0;

            Events.Subscribe<object>(e => numberOfEvents++);
			Events.Subscribe<FooEvent>(e => numberOfFooEvents++);
			Events.Subscribe<FooChildEvent>(e => numberOfFooChildEvents++);

			Events.Publish(new FooEvent());
			Events.Publish(new FooChildEvent());
			Events.Publish(new BarEvent());

			Assert.Equal(3, numberOfEvents);
			Assert.Equal(2, numberOfFooEvents);
			Assert.Equal(1, numberOfFooChildEvents);
		}

		[Fact]
		public void ShouldGetEventsToHandlerAfterUnsubscribeAction()
		{
			var handler = new DummyHandler();
			Events.RegisterHandler(handler);
			Events.Unsubscribe<FooEvent>();

			Events.Publish(new FooEvent());

			Assert.NotNull(handler.HandledEvent);
		}

		[Fact]
		public void CanUnregisterHandler()
		{
			var handler = new DummyHandler();
			Events.RegisterHandler(handler);

			Events.UnregisterHandler(handler);

			Events.Publish(new FooEvent());

			Assert.Null(handler.HandledEvent);
		}

		[Fact]
		public void CanDelayStart()
		{
			Events.Service = new Services.NetworkService(autoStart: false);

			Assert.Throws<NotSupportedException>(() => 
				Events.Publish(new FooEvent()));
			Assert.False(Events.Service.IsStarted);
		}

		DummyService service;

		public ServiceBaseTest()
		{
			service = new DummyService();
			Events.Service = service;
		}
	}
}
