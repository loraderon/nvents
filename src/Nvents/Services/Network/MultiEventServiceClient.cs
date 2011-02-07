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
		ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		DateTime lastDiscoveryLookup = DateTime.Now.AddDays(-1);

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
					try
					{
						s.Publish(@event);
					}
					catch (TimeoutException)
					{
						RemoveServer(s);
					}
				}, server);
			}
		}

		private IEnumerable<IEventService> GetServers()
		{
			locker.EnterUpgradeableReadLock();
			foreach (var server in servers.Values.ToArray())
			{
				yield return server;
			}

			locker.EnterWriteLock();
			foreach (var address in GetAddresses())
			{
				if (servers.ContainsKey(address))
					continue;

				var server = factory.CreateChannel(address);
				var connection = server as ICommunicationObject;
				connection.Faulted += connection_FaultedOrClosing;
				connection.Closing += connection_FaultedOrClosing;
				try
				{
					connection.Open();
				}
				catch (EndpointNotFoundException)
				{
					continue;
				}
				servers[address] = server;
				yield return server;
			}
			locker.ExitWriteLock();
			locker.ExitUpgradeableReadLock();
		}

		private void connection_FaultedOrClosing(object sender, EventArgs e)
		{
			var server = sender as IEventService;
			RemoveServer(server);
		}

		private void RemoveServer(IEventService server)
		{
			locker.EnterWriteLock();
			if (servers.ContainsValue(server))
			{
				var address = servers.Single(x => x.Value == server).Key;
				servers.Remove(address);
			}
			locker.ExitWriteLock();
		}

		private IEnumerable<EndpointAddress> GetAddresses()
		{
			var now = DateTime.Now;
			if (now - lastDiscoveryLookup < TimeSpan.FromSeconds(1))
				return new EndpointAddress[0];

			lastDiscoveryLookup = now;
			using (var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint()))
			{
				var discoveryResponse = discoveryClient.Find(new FindCriteria(typeof(IEventService)) { Duration = TimeSpan.FromMilliseconds(500) });
				return discoveryResponse.Endpoints.Select(x => x.Address);
			}
		}

		public void Dispose()
		{
			locker.EnterWriteLock();
			foreach (var server in servers.Values.ToArray())
			{
				var connection = server as ICommunicationObject;
				if (connection.State == CommunicationState.Opened)
					connection.Close();
			}
			locker.ExitWriteLock();
			var disposable = factory as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
	}
}