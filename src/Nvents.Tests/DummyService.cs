using System;
using Nvents.Services;

namespace Nvents.Tests
{
	public class DummyService : ServiceBase
	{
		public override void Publish<TEvent>(TEvent e)
		{
			PublishWasCalled = true;
		}
		
		public bool PublishWasCalled { get; private set; }
	}
}
