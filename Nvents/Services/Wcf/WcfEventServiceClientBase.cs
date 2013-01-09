using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
#if !NET35
using System.ServiceModel.Discovery;
#endif
using System.Threading;
using System.ServiceModel.Channels;

namespace Nvents.Services.Wcf
{
	public abstract class WcfEventServiceClientBase : IEventService, IDisposable
	{
		ChannelFactory<IEventService> factory;
		Dictionary<EndpointAddress, IEventService> servers = new Dictionary<EndpointAddress, IEventService>();
		ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		DateTime lastDiscoveryLookup = DateTime.Now.AddDays(-1);
		public event EventHandler<PublishErrorEventArgs> PublishError;

		protected virtual void OnPublishError(PublishErrorEventArgs e)
		{
			EventHandler<PublishErrorEventArgs> handler = PublishError;
			if (handler != null) handler(this, e);
		}

		public WcfEventServiceClientBase(string encryptionKey)
		{
			factory = new ChannelFactory<IEventService>(GetBinding());
			factory.Endpoint.Contract.Operations[0].Behaviors.Add(new EncryptionBehavior { EncryptionKey = encryptionKey });
		}

		abstract protected Binding GetBinding();

		public void Publish(object @event)
		{
            ThreadPool.QueueUserWorkItem(_ =>
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
                        catch (TimeoutException toe)
                        {
	                        HandleException(@event, s, toe);
                        }
                        catch (Exception ex)
						{
							HandleException(@event, s, ex);
						}
                    }, server);
                }
            });
		}

		private void HandleException(object @event, IEventService service, Exception exception)
		{
			var server = servers.SingleOrDefault(x => x.Value == service);
			string destination = "UNKNOWN";
			if (!server.Equals(default(KeyValuePair<EndpointAddress,IEventService>)))
				destination = server.Key.Uri.ToString();

			OnPublishError(new PublishErrorEventArgs(@event, destination, exception));
			RemoveServer(service);
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
#if !NET35
			using (var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint()))
			{
				var discoveryResponse = discoveryClient.Find(new FindCriteria(typeof(IEventService)) { Duration = TimeSpan.FromMilliseconds(500) });
				return discoveryResponse.Endpoints.Select(x => x.Address);
			}
#else
			var services = Discovery.ServiceDiscoverer.FindServices(TimeSpan.FromMilliseconds(500));
			if (services.Length == 0)
				services = Discovery.ServiceDiscoverer.FindServices(TimeSpan.FromMilliseconds(500));
			return services
				.Select(x => GetEndpoint(x));
#endif
		}
#if NET35
		abstract protected EndpointAddress GetEndpoint(Discovery.Service service);
#endif

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
			{
				try
				{
					disposable.Dispose();
				}
				catch (CommunicationObjectFaultedException)
				{ }
			}
		}
	}
}