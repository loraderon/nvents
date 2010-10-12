using System;
using System.ServiceModel;

namespace Nvents.Services.Network
{
	[ServiceContract(Namespace = "http://Nvents.Services.Network")]
	[NetDataContractFormat]
	public interface IEventService
	{
		[OperationContract(IsOneWay = true)]
		[NetDataContractFormat]
		void Publish(IEvent @event);
	}
}