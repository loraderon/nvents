using System;
using System.Linq;
using System.Net;
using Nvents.Services.Network;

namespace Nvents.Services
{
	public class NetworkService : ServiceBase
	{
		EventService server;
		IEventServiceHost host;
		MultiEventServiceClient client;
		string encryptionKey;

		public NetworkService(IPAddress ipAddress, int port, string encryptionKey = null)
		{
			this.encryptionKey = encryptionKey;
			server = new EventService();
			CreateClient();
			host = new EventServiceHost(ipAddress, port, encryptionKey);

			server.EventPublished += server_EventPublished;
		}

		private void CreateClient()
		{
			client = new MultiEventServiceClient(encryptionKey);
		}

		void server_EventPublished(object sender, EventPublishedEventArgs e)
		{
			foreach (var handler in handlers
				.Where(x => ShouldEventBeHandled(x, e.Event)))
			{
				try
				{
					handler.Action(e.Event);
				}
				catch { }
			}
		}

		public override void Publish(IEvent e)
		{
			if (!IsStarted)
				throw new NotSupportedException("Service is not started.");
			client.Publish(e);
		}

		protected override void OnStart()
		{
			host.Start(server);
			CreateClient();
		}

		protected override void OnStop()
		{
			client.Dispose();
			host.Stop();
		}
	}
}