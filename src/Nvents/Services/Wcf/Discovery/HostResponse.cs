#if NET35
using System;
using System.Collections;
using System.Net;

namespace Nvents.Services.Wcf.Discovery
{

	public struct HostResponse
	{
		readonly IPAddress address;
		readonly IDictionary endpointProps;

		internal HostResponse(IPAddress address, IDictionary endpointProps)
		{
			this.address = address;
			this.endpointProps = endpointProps;
		}

		public IPAddress IPAddress
		{
			get { return address; }
		}

		public IDictionary EndpointProperties
		{
			get { return endpointProps; }
		}

		public override bool Equals(object obj)
		{
			return ((obj is HostResponse) &&
				(address.Equals(((HostResponse)obj).address)));
		}

		public override int GetHashCode()
		{
			return address.GetHashCode();
		}

		public static bool operator ==(HostResponse a, HostResponse b)
		{
			return a.address.Equals(b.address);
		}

		public static bool operator !=(HostResponse a, HostResponse b)
		{
			return !(a == b);
		}
	}
}
#endif