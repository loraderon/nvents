using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Net.Sockets;
using System.Net;

namespace Nvents.Services.Network
{

	public class EventServiceHost : IEventServiceHost
	{
		ServiceHost host;

		public void Start(IEventService instance)
		{
			if (host != null)
				throw new NotSupportedException("Service has already started.");
			CreateServiceHost(instance, GetIpAddress(), GetFreeTcpPort());
			host.Open();
		}

		public void Stop()
		{
			if (host == null)
				return;
			host.Close();
			host = null;
		}

		public bool IsStarted { get; private set; }

		private void CreateServiceHost(IEventService instance, string ipaddress, int port)
		{
			host = new ServiceHost(instance,
				new Uri(string.Format(
					"net.tcp://{0}:{1}/Nvents.Services.Network/{2}",
					ipaddress,
					port,
					Guid.NewGuid())));
			host.AddServiceEndpoint(
				typeof(IEventService),
				new NetTcpBinding(),
				"EventService");

			var discoveryBehavior = new ServiceDiscoveryBehavior();
			host.Description.Behaviors.Add(discoveryBehavior);
			host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
			discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());

			host.Description.Behaviors.Add(new ServiceThrottlingBehavior
			{
				MaxConcurrentSessions = 1000
			});

			host.Opened += (s, e) => IsStarted = true;
			host.Closed += (s, e) => IsStarted = false;
		}

		private int GetFreeTcpPort()
		{
			var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 0);
			listener.Start();
			int port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();
			return port;
		}

		private string GetIpAddress()
		{
			foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
			{
				if (ip.AddressFamily != AddressFamily.InterNetwork)
					continue;
				return ip.ToString();
			}
			throw new NotSupportedException("Can not start without a valid network.");
		}
	}
}
