using System;
using System.Linq;
using Nvents.Services.Network;

namespace Nvents.Services
{
	public class NamedPipesService : ServiceBase
	{
		EventService server;
		IEventServiceHost host;
		NamedPipesClient client;
		string encryptionKey;
		string pipe;

		public NamedPipesService(string pipe = "nvents", bool autoStart = true, string encryptionKey = null)
			: base(autoStart)
		{
			this.pipe = pipe;
			this.encryptionKey = encryptionKey;
			server = new EventService();
			CreateClient();
			host = new NamedPipesEventServiceHost(pipe, encryptionKey);

			server.EventPublished += server_EventPublished;
		}

		private void CreateClient()
		{
			client = new NamedPipesClient(pipe, encryptionKey);
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

		public override void Publish<TEvent>(TEvent e)
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
