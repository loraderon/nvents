using System;

namespace Nvents.Services.Network
{
	public interface IEventServiceHost
	{
		void Start(IEventService instance);
		void Stop();
		bool IsStarted { get; }
	}
}
