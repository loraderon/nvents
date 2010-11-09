using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nvents.Services.Network;

namespace Nvents.Services
{
	public class NetworkService : IService
	{
		List<EventHandler> handlers = new List<EventHandler>();
		EventService server;
		IEventServiceHost host;
		IEventService client;

		public NetworkService(IPAddress ipAddress, int port, string encryptionKey = null)
		{
			server = new EventService();
			client = new MultiEventServiceClient(encryptionKey);

			host = new EventServiceHost(ipAddress, port, encryptionKey);

			server.EventPublished += server_EventPublished;
		}

		void server_EventPublished(object sender, EventPublishedEventArgs e)
		{
			foreach (var handler in handlers
				.Where(x => x.EventType == e.Event.GetType()))
			{
				try
				{
					handler.Action(e.Event);
				}
				catch { }
			}
		}

		public void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class, IEvent
		{
			var handler = new EventHandler();
			handler.SetHandler(action, filter);
			handlers.Add(handler);
		}

		public void Unsubscribe<TEvent>() where TEvent : class, IEvent
		{
			handlers.RemoveAll(x => x.EventType == typeof(TEvent));
		}

		public bool IsStarted { get { return host != null && host.IsStarted; } }

		public void Publish(IEvent e)
		{
			if (!IsStarted)
				throw new NotSupportedException("Service is not started.");
			client.Publish(e);
		}

		public void Start()
		{
			host.Start(server);
		}

		public void Stop()
		{
			host.Stop();
		}
	}
}