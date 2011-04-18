using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Nvents.Services.Wcf
{
	public class TcpEventEventServiceHost : WcfEventServiceHostBase
	{
		IPAddress ipAddress;
		int port;

		public TcpEventEventServiceHost(string encryptionKey, IPAddress ipAddress, int port)
			: base(encryptionKey)
		{
			this.ipAddress = ipAddress;
			this.port = port;
		}

		public override void Start(IEventService instance)
		{
			if (port < 1 || port > 65535)
				throw new ArgumentOutOfRangeException("port");
			if (ipAddress == null)
				throw new ArgumentNullException("ipaddress");
			base.Start(instance);
		}

		protected override Uri GetUri(Guid guid)
		{
			return new Uri(string.Format(
				"net.tcp://{0}:{1}/Nvents.Services.Network/{2}",
				ipAddress,
				port,
				guid));
		}

		protected override Binding GetBinding()
		{
			var binding = new NetTcpBinding(SecurityMode.None);
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
			binding.ReliableSession.Enabled = true;
			return binding;
		}

#if NET35
		protected override void StartDiscoverer(Discovery.ServiceDiscoverer discoverer, Guid guid)
		{
			discoverer.Start(ipAddress.ToString(), port, guid);
		}
#endif
	}
}
