using System;

namespace Nvents.Tests
{
	public class DummyHandler : IHandler<FooEvent>
	{
		public FooEvent HandledEvent { get; private set; }

		public void Handle(FooEvent @event)
		{
			HandledEvent = @event;
		}
	}
}
