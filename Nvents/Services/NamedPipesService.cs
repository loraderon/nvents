using System;
using Nvents.Services.Wcf;

namespace Nvents.Services
{
	/// <summary>
	/// Named pipes service for publishing/subscribing to events on the same machine
	/// </summary>
	public class NamedPipesService : WcfServiceBase
	{
		string pipe;

		/// <summary>
		/// Named pipes service for publishing/subscribing to events on the same machine
		/// </summary>
		/// <param name="autoStart">Name of the pipe to use</param>
		/// <param name="autoStart">Determines if the service should automatically start when publishing events</param>
		/// <param name="encryptionKey">Key for encrypting the events before publishing</param>
		public NamedPipesService(string pipe, bool autoStart = true, string encryptionKey = null)
			: base(autoStart, encryptionKey)
		{
			this.pipe = pipe;
		}

		/// <summary>
		/// Named pipes service for publishing/subscribing to events on the same machine using the default pipe name
		/// </summary>
		/// <param name="autoStart">Determines if the service should automatically start when publishing events</param>
		/// <param name="encryptionKey">Key for encrypting the events before publishing</param>
		public NamedPipesService(bool autoStart = true, string encryptionKey = null)
			: this("nvents", autoStart, encryptionKey)
		{ }
		 
		protected override WcfEventServiceClientBase CreateClient(string encryptionKey)
		{
			return new NamedPipesEventServiceClient(encryptionKey, pipe);
		}

		protected override IEventServiceHost CreateEventServiceHost(string encryptionKey)
		{
			return new NamedPipesEventServiceHost(encryptionKey, pipe);
		}
	}
}
