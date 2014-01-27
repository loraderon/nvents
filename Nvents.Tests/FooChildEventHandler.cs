using System;
using Xunit;

namespace Nvents.Tests
{
    public class FooChildEventHandler : IHandler<FooChildEvent>
    {
        public int NumberOfEvents { get; private set; }

        public void Handle(FooChildEvent @event)
        {
            NumberOfEvents++;
        }
    }
}
