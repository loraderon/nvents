using System;
using System.Net;
using Nvents.Services.Wcf;

namespace Nvents.Services
{
	public class NetworkService : WcfServiceBase
	{
		IPAddress ipAddress;
		int port;

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