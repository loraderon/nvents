using System;
using System.Net;
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

		protected override WcfEventServiceClientBase CreateClient(string encryptionKey)
		{
			return new TcpEventServiceClient(encryptionKey);
		}

		protected override IEventServiceHost CreateEventServiceHost(string encryptionKey)
		{
			return new TcpEventEventServiceHost(encryptionKey, ipAddress, port);
		}
	}
}