using System;
using Xunit;

namespace Nvents.Tests
{
    public class FooEventHandler : IHandler<FooEvent>
    {
        public int NumberOfEvents { get; private set; }

        public void Handle(FooEvent @event)
        {
            NumberOfEvents++;
        }
    }
}
