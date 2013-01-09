using System;
using System.Net;
using System.Net.Sockets;
using Nvents.Services.Wcf;

namespace Nvents.Services
{
	/// <summary>
	/// Network service for publishing/subscribing to events on the local network
	/// </summary>
	public class NetworkService : WcfServiceBase
	{
		IPAddress ipAddress;
		int port;

		/// <summary>
		/// Network service for publishing/subscribing to events on the local network
		/// </summary>
		/// <param name="ipAddress">The IP address to listen to</param>
		/// <param name="port">The port to listen to</param>
		/// <param name="autoStart">Determines if the service should automatically start when publishing events</param>
		/// <param name="encryptionKey">Key for encrypting the events before publishing</param>
		public NetworkService(IPAddress ipAddress, int port, bool autoStart = true, string encryptionKey = null)
			: base(autoStart, encryptionKey)
		{
			this.ipAddress = ipAddress;
			this.port = port;
		}

		/// Network service for publishing/subscribing to events on the local network that automatically listens to the first local network on a available port
		/// <param name="autoStart">Determines if the service should automatically start when publishing events</param>
		/// <param name="encryptionKey">Key for encrypting the events before publishing</param>
		public NetworkService(bool autoStart = true, string encryptionKey = null)
			: this(GetIpAddress(), GetFreeTcpPort(), autoStart, encryptionKey)
		{ }

		protected override WcfEventServiceClientBase CreateClient(string encryptionKey)
		{
			var client = new TcpEventServiceClient(encryptionKey);
			client.PublishError += ClientOnPublishError;
			return client;
		}

		private void ClientOnPublishError(object sender, PublishErrorEventArgs publishErrorEventArgs)
		{
			OnPublishError(publishErrorEventArgs);
		}

		protected override IEventServiceHost CreateEventServiceHost(string encryptionKey)
		{
			return new TcpEventEventServiceHost(encryptionKey, ipAddress, port);
		}

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