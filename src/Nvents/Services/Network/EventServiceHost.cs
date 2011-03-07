using System;
using System.ServiceModel;
using System.ServiceModel.Description;
#if NET40
using System.ServiceModel.Discovery;
#endif
using System.Net;

namespace Nvents.Services.Network
{

	public class EventServiceHost : IEventServiceHost
	{
		ServiceHost host;
		IPAddress ipAddress;
		int port;
		string encryptionKey;

		public EventServiceHost(IPAddress ipAddress, int port, string encryptionKey)
		{
			this.ipAddress = ipAddress;
			this.port = port;
			this.encryptionKey = encryptionKey;
		}

		public void Start(IEventService instance)
		{
			if (host != null)
				throw new NotSupportedException("Service has already started.");
			if (port < 1 || port > 65535)
				throw new ArgumentOutOfRangeException("port");
			if (ipAddress == null)
				throw new ArgumentNullException("ipaddress");
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
		}

		public bool IsStarted { get; private set; }

		private void CreateServiceHost(IEventService instance)
		{
			host = new ServiceHost(instance,
				new Uri(string.Format(
					"net.tcp://{0}:{1}/Nvents.Services.Network/{2}",
					ipAddress,
					port,
					Guid.NewGuid())));
			var binding = new NetTcpBinding(SecurityMode.None);
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
			binding.ReliableSession.Enabled = true;
			host.AddServiceEndpoint(
				typeof(IEventService),
				binding,
				"EventService");
#if NET40
			var discoveryBehavior = new ServiceDiscoveryBehavior();
			host.Description.Behaviors.Add(discoveryBehavior);
			host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
			discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
#else
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
