using System;

namespace Nvents.Services.Network
{
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
	public partial class EventServiceClient : System.ServiceModel.ClientBase<IEventService>, IEventService
	{
		public EventServiceClient()
		{
		}

		public EventServiceClient(string endpointConfigurationName) :
			base(endpointConfigurationName)
		{
		}

		public EventServiceClient(string endpointConfigurationName, string remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}

		public EventServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}

		public EventServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			base(binding, remoteAddress)
		{
		}

		public void Publish(IEvent @event)
		{
			base.Channel.Publish(@event);
		}
	}
}
