using System;

namespace Nvents.Services.Wcf
{
	public interface IEventServiceHost
	{
		void Start(IEventService instance);
		void Stop();
		bool IsStarted { get; }
	}
}
