using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Nvents.Services.Wcf
{
	public class NamedPipesEventServiceHost : WcfEventServiceHostBase
	{
		string pipe;

		public NamedPipesEventServiceHost(string encryptionKey, string pipe)
			: base(encryptionKey)
		{
			this.pipe = pipe;
		}

		public override void Start(IEventService instance)
		{
			if (pipe == null)
				throw new ArgumentNullException("pipe");
			base.Start(instance);
		}

		protected override Uri GetUri(Guid guid)
		{
			return new Uri(string.Format(
				"net.pipe://localhost/Nvents.Services.Network/{0}/{1}",
				pipe,
				guid));
		}

		protected override Binding GetBinding()
		{
			return new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
		}

#if NET35
		protected override void StartDiscoverer(Discovery.ServiceDiscoverer discoverer, Guid guid)
		{
			discoverer.Start(null, 0, guid);
		}
#endif
	}
}
