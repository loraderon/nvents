using System;
using Nvents.Services.Wcf;

namespace Nvents.Services
{
	public class NamedPipesService : WcfServiceBase
	{
		string pipe;

		public NamedPipesService(string pipe, bool autoStart = true, string encryptionKey = null)
			: base(autoStart, encryptionKey)
		{
			this.pipe = pipe;
		}

		public NamedPipesService(bool autoStart = true, string encryptionKey = null)
			: this("nvents", autoStart, encryptionKey)
		{
			System.Diagnostics.Debugger.Break();
		}
		 
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
