using System;
using System.Linq;

namespace Nvents.Services.Wcf
{
	public abstract class WcfServiceBase : ServiceBase
	{
		WcfEventService server;
		IEventServiceHost host;
		WcfEventServiceClientBase client;
		string encryptionKey;

		public WcfServiceBase(bool autoStart = true, string encryptionKey = null)
			: base(autoStart)
		{
			this.encryptionKey = encryptionKey;
			server = new WcfEventService();

			server.EventPublished += server_EventPublished;
		}

		abstract protected WcfEventServiceClientBase CreateClient(string encryptionKey);
		abstract protected IEventServiceHost CreateEventServiceHost(string encryptionKey);

		void server_EventPublished(object sender, EventPublishedEventArgs e)
		{
			foreach (var registration in registrations
				.Where(x => ShouldEventBeHandled(x, e.Event)))
			{
				try
				{
					registration.Action(e.Event);
				}
				catch { }
			}
		}

		public override void Publish<TEvent>(TEvent e)
		{
			if (!IsStarted)
				throw new NotSupportedException("Service is not started.");
			client.Publish(e);
		}

		protected override void OnStart()
		{
			if (host == null)
				host = CreateEventServiceHost(encryptionKey);
			host.Start(server);
			client = CreateClient(encryptionKey);
		}

		protected override void OnStop()
		{
			if (client != null)
				client.Dispose();
			if (host != null)
				host.Stop();
		}
	}
}
