using System;
using System.Net;
using System.Net.Sockets;

namespace Nvents.Services
{
	/// <summary>
	/// Network service for publishing/subscribing to events on the local network that automatically listens to the first local network on a available port
	/// </summary>
	public class AutoNetworkService : NetworkService
	{
		/// <summary>
		/// Network service for publishing/subscribing to events on the local network that automatically listens to the first local network on a available port
		/// </summary>
		/// <param name="autoStart">Determines if the service should automatically start when publishing events</param>
		/// <param name="encryptionKey">Key for encrypting the events before publishing</param>
		public AutoNetworkService(bool autoStart = true, string encryptionKey = null)
			: base(GetIpAddress(), GetFreeTcpPort(), autoStart, encryptionKey)
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