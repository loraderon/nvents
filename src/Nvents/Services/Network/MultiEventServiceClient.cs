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
		string encryptionKey;

		public MultiEventServiceClient(string encryptionKey)
		{
			this.encryptionKey = encryptionKey;
		}

		public void Publish(IEvent @event)
		{
			foreach (var server in GetServers())
			{
				ThreadPool.QueueUserWorkItem(state =>
				{
					var s = state as IEventService;
					PublishToServer(s, @event);
				}, server);
			}
		}

		private void PublishToServer(IEventService server, IEvent @event)
		{
			var connection = server as ICommunicationObject;
			connection.Open();
			server.Publish(@event);
			connection.Close();
		}

		private IEnumerable<IEventService> GetServers()
		{
			var binding = new NetTcpBinding(SecurityMode.None);
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
			binding.ReliableSession.Enabled = true;
			var factory = new ChannelFactory<IEventService>(binding);
			factory.Endpoint.Contract.Operations[0].Behaviors.Add(new EncryptionBehavior { EncryptionKey = encryptionKey });
			foreach (var address in GetAddresses())
				yield return factory.CreateChannel(address);
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
