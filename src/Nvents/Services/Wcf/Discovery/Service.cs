#if NET35
using System;

namespace Nvents.Services.Wcf.Discovery
{
	[Serializable]
	public class Service
	{
		public Service(string iPAddress, int port, Guid guid)
		{
			IPAddress = iPAddress;
			Port = port;
			Guid = guid;
		}

		public readonly string IPAddress;
		public readonly int Port;
		public readonly Guid Guid;
	}
}
#endif