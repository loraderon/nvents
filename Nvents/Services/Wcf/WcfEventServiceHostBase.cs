using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
#if !NET35
using System.ServiceModel.Discovery;
#endif

namespace Nvents.Services.Wcf
{

	public abstract class WcfEventServiceHostBase : IEventServiceHost
	{
		ServiceHost host;
		string encryptionKey;
#if NET35
		Discovery.ServiceDiscoverer discoverer;
#endif

		public WcfEventServiceHostBase(string encryptionKey)
		{
			this.encryptionKey = encryptionKey;
		}

		public virtual void Start(IEventService instance)
		{
			if (host != null)
				throw new NotSupportedException("Service has already started.");
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
				GetUri(guid));
			host.AddServiceEndpoint(
				typeof(IEventService),
				GetBinding(),
				"EventService");
#if !NET35
			var discoveryBehavior = new ServiceDiscoveryBehavior();
			host.Description.Behaviors.Add(discoveryBehavior);
			host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
			discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
#else
			discoverer = new Discovery.ServiceDiscoverer();
			StartDiscoverer(discoverer, guid);
#endif

			host.Description.Behaviors.Add(new ServiceThrottlingBehavior
			{
				MaxConcurrentSessions = 1000
			});

			host.Description.Endpoints[0].Contract.Operations[0].Behaviors.Add(new EncryptionBehavior { EncryptionKey = encryptionKey });

			host.Opened += (s, e) => IsStarted = true;
			host.Closed += (s, e) => IsStarted = false;
		}
		
		abstract protected Uri GetUri(Guid guid);
		abstract protected Binding GetBinding();
#if NET35
		abstract protected void StartDiscoverer(Discovery.ServiceDiscoverer discoverer, Guid guid);
#endif
	}
}