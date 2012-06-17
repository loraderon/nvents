using System;
using Nvents.Services;

namespace Chat.Moderator.Tests.Fakes
{
	public class FakePublisher : IPublisher
	{
		public object LastPublishedEvent { get; private set; }

		public void Publish<TEvent>(TEvent @event) where TEvent : class
		{
			LastPublishedEvent = @event;
		}
	}
}
