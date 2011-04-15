using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Nvents.Services.Wcf
{
	public class NamedPipesEventServiceClient : WcfEventServiceClientBase
	{
		string pipe;

		public NamedPipesEventServiceClient(string encryptionKey, string pipe)
			: base(encryptionKey)
		{
			this.pipe = pipe;
		}         

		protected override Binding GetBinding()
		{
			return new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
		}

#if NET35
		protected override EndpointAddress GetEndpoint(Discovery.Service service)
		{
			return new EndpointAddress(
				string.Format("net.pipe://localhost/Nvents.Services.Network/{0}/{1}/EventService",
				pipe,
				service.Guid));
		}
#endif
	}
}
