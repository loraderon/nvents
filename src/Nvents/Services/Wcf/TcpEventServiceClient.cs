using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Nvents.Services.Wcf
{
	public class TcpEventServiceClient : WcfEventServiceClientBase
	{
		public TcpEventServiceClient(string encryptionKey)
			: base(encryptionKey)
		{ }

		protected override Binding GetBinding()
		{
			var binding = new NetTcpBinding(SecurityMode.None);
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
			binding.ReliableSession.Enabled = true;
			return binding;
		}

#if NET35
		protected override EndpointAddress GetEndpoint(Discovery.Service service)
		{
			return new EndpointAddress(
				string.Format("net.tcp://{0}:{1}/Nvents.Services.Network/{2}/EventService",
				service.IPAddress,
				service.Port,
				service.Guid));
		}
#endif
	}
}
