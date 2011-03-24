using System;

namespace Nvents.Tests
{
	public class FooBarHandler : IHandler<FooEvent>, IHandler<BarEvent>
	{
		public bool FooHandled { get; private set; }
		public bool BarHandled { get; private set; }
		
		public void Handle(FooEvent @event)
		{
			FooHandled = true;
		}

		public void Handle(BarEvent @event)
		{
			BarHandled = true;
		}
	}
}
