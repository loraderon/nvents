using System;
using System.Collections.Generic;
using System.Linq;
using Nvents.Services.Network;

namespace Nvents.Services
{
	public class AutoNetworkService : IService
	{
		List<Handler> handlers = new List<Handler>();
		EventService server;
		IEventServiceHost host;
		IEventService client;

		public AutoNetworkService()
		{
			server = new EventService();
			client = new MultiEventServiceClient();

			host = new EventServiceHost();

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

		public void Subscribe<TEvent>(Action<TEvent> action) where TEvent : class, IEvent
		{
			var handler = new Handler();
			handler.SetHandler(action);
			handlers.Add(handler);
		}

		public void Unsubscribe<TEvent>() where TEvent : class, IEvent
		{
			handlers.RemoveAll(x => x.EventType == typeof(TEvent));
		}

		public bool IsStarted { get { return host != null && host.IsStarted; } }

		public void Publish(IEvent e)
		{
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

		private class Handler
		{
			public void SetHandler<TEvent>(Action<TEvent> action) where TEvent : class, IEvent
			{
				Action = e =>
				{
					action(e as TEvent);
				};
				EventType = typeof(TEvent);
			}
			public Type EventType { get; private set; }
			public Action<IEvent> Action { get; private set; }
		}
	}
}