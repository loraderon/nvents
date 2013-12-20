using System;

namespace Nvents.Tests
{
	public class UniqueEvent
	{
	    public string Baz { get; set; }
        public Guid ID { get; set; }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((UniqueEvent) obj);
	    }

        protected bool Equals(UniqueEvent other)
        {
            return ID.Equals(other.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
	}
}
