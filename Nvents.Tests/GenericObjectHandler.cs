using System;
using Xunit;

namespace Nvents.Tests
{
    public class GenericObjectHandler : IHandler<object>
    {
        public int NumberOfEvents { get; private set; }

        public void Handle(object @event)
        {
            NumberOfEvents++;
        }
    }
}
