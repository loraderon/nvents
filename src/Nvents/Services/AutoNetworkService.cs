using System;
using System.Net;
using System.Net.Sockets;

namespace Nvents.Services
{
	public class AutoNetworkService : NetworkService
	{
		public AutoNetworkService(string encryptionKey = null)
			: base(GetIpAddress(), GetFreeTcpPort(), encryptionKey)
		{ }

		private static int GetFreeTcpPort()
		{
			var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 0);
			listener.Start();
			int port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();
			return port;
		}

		private static IPAddress GetIpAddress()
		{
			foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
			{
				if (ip.AddressFamily != AddressFamily.InterNetwork)
					continue;
				return ip;
			}
			throw new NotSupportedException("Can not start without a valid network.");
		}
	}
}