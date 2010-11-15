using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Threading;

namespace Nvents.Services.Network
{
	public class MultiEventServiceClient : IEventService, IDisposable
	{
		ChannelFactory<IEventService> factory;
		Dictionary<EndpointAddress, IEventService> servers = new Dictionary<EndpointAddress, IEventService>();

		public MultiEventServiceClient(string encryptionKey)
		{
			var binding = new NetTcpBinding(SecurityMode.None);
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
			binding.ReliableSession.Enabled = true;
			factory = new ChannelFactory<IEventService>(binding);
			factory.Endpoint.Contract.Operations[0].Behaviors.Add(new EncryptionBehavior { EncryptionKey = encryptionKey });
		}

		public void Publish(IEvent @event)
		{
			foreach (var server in GetServers())
			{
				ThreadPool.QueueUserWorkItem(state =>
				{
					var s = state as IEventService;
					s.Publish(@event);
				}, server);
			}
		}

		private IEnumerable<IEventService> GetServers()
		{
			foreach (var server in servers.Values)
			{
				yield return server;
			}

			foreach (var address in GetAddresses())
			{
				if (servers.ContainsKey(address))
					continue;

				var server = factory.CreateChannel(address);
				var connection = server as ICommunicationObject;
				connection.Faulted += (s, e) => servers.Remove(address);
				connection.Closing += (s, e) => servers.Remove(address);
				connection.Open();
				servers[address] = server;
				yield return server;
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

		public void Dispose()
		{
			foreach (var server in servers.Values.ToArray())
			{
				var connection = server as ICommunicationObject;
				if (connection.State == CommunicationState.Opened)
					connection.Close();
			}
			var disposable = factory as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
	}
}
