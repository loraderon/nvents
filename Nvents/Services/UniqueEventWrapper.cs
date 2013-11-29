using System;

namespace Nvents.Services
{
    [Serializable]
    internal class UniqueEventWrapper : IEquatable<UniqueEventWrapper>
    {

        public object Event { get; private set; }
        public Guid ID { get; private set; }

        public UniqueEventWrapper(object nvent)
        {
            if (nvent==null)
                throw new ArgumentNullException("nvent", "Event object must be supplied for UniqueEventWrapper");
            
            Event = nvent;
            ID = Guid.NewGuid();
        }

        public bool Equals(UniqueEventWrapper other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.Event == null || other.Event == null) return false;
            if (this.Event.GetType() != other.Event.GetType()) return false;
            return ID.Equals(other.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public static bool operator ==(UniqueEventWrapper left, UniqueEventWrapper right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UniqueEventWrapper left, UniqueEventWrapper right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UniqueEventWrapper) obj);
        }
    }
}
