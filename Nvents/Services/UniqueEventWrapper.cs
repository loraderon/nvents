using System;

namespace Nvents.Services
{
    internal class UniqueEventWrapper : IUniqueNvent
    {
        public object Event { get; private set; }
        public Guid ID { get; private set; }

        public UniqueEventWrapper(object nvent)
        {
            Event = nvent;
            ID = Guid.NewGuid();
        }
    }
}
