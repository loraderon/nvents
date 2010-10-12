using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Threading;

namespace Nvents.Services.Network
{
	public class MultiEventServiceClient : IEventService
	{
		public void Publish(IEvent @event)
		{
			foreach (var server in GetServers())
			{
				ThreadPool.QueueUserWorkItem(state =>
				{
					var s = state as EventServiceClient;
					PublishToServer(s, @event);
				}, server);
			}
		}

		private void PublishToServer(EventServiceClient server, IEvent @event)
		{
			server.Open();
			server.Publish(@event);
			server.Close();
		}

		private IEnumerable<EventServiceClient> GetServers()
		{
			foreach (var address in GetAddresses())
			{
				yield return new EventServiceClient(new NetTcpBinding(), address);
			}
		}

		private static IEnumerable<EndpointAddress> GetAddresses()
		{
			using (var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint()))
			{
				var discoveryResponse = discoveryClient.Find(new FindCriteria(typeof(IEventService)) { Duration = TimeSpan.FromMilliseconds(500) });
				return discoveryResponse.Endpoints.Select(x => x.Address);
			}
		}
	}
}
