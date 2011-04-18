using System;
using Nvents;
using Nvents.Services;

namespace Chat.Moderator.Tests.Fakes
{
	public class FakePublisher : IPublisher
	{
		public IEvent LastPublishedEvent { get; private set; }

		public void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
		{
			LastPublishedEvent = @event;
		}
	}
}
