using System;
using System.ServiceModel;
using System.ServiceModel.Description;
#if !NET35
using System.ServiceModel.Discovery;
#endif

namespace Nvents.Services.Network
{
	public class NamedPipesEventServiceHost : IEventServiceHost
	{
		ServiceHost host;
		string pipe;
		string encryptionKey;
#if NET35
		ServiceDiscoverer.ServiceDiscoverer discoverer;
#endif

		public NamedPipesEventServiceHost(string pipe, string encryptionKey)
		{
			this.pipe = pipe;
			this.encryptionKey = encryptionKey;
		}

		public void Start(IEventService instance)
		{
			if (host != null)
				throw new NotSupportedException("Service has already started.");
			if (pipe == null)
				throw new ArgumentNullException("pipe");
			CreateServiceHost(instance);
			host.Open();
		}

		public void Stop()
		{
			if (host == null)
				return;
			if (host.State == CommunicationState.Opened)
				host.Close();
			host = null;

#if NET35
			if (discoverer == null)
				return;
			discoverer.Stop();
			discoverer = null;
#endif
		}

		public bool IsStarted { get; private set; }

		private void CreateServiceHost(IEventService instance)
		{
			var guid = Guid.NewGuid();
			host = new ServiceHost(instance,
				new Uri(string.Format(
					"net.pipe://localhost/Nvents.Services.Network/{0}/{1}",
					pipe,
					guid)));
			var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

			host.AddServiceEndpoint(
				typeof(IEventService),
				binding,
				"EventService");
#if !NET35
			var discoveryBehavior = new ServiceDiscoveryBehavior();
			host.Description.Behaviors.Add(discoveryBehavior);
			host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
			discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
#else
			discoverer = new ServiceDiscoverer.ServiceDiscoverer();
			discoverer.Start(null, 0, guid);
#endif

			host.Description.Behaviors.Add(new ServiceThrottlingBehavior
			{
				MaxConcurrentSessions = 1000
			});

			host.Description.Endpoints[0].Contract.Operations[0].Behaviors.Add(new EncryptionBehavior { EncryptionKey = encryptionKey });

			host.Opened += (s, e) => IsStarted = true;
			host.Closed += (s, e) => IsStarted = false;
		}
	}
}
