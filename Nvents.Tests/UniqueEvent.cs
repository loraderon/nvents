using System;
using Nvents.Services;

namespace Nvents.Tests
{
	public class UniqueEvent : IUniqueNvent
	{
		public string Baz { get; set; }
        public Guid ID { get; set; }
	}
}
