using System;
using System.ServiceModel;

namespace Nvents.Services.Wcf
{
	[ServiceContract(Namespace = "http://Nvents.Services.Network")]
	public interface IEventService
	{
		[OperationContract(IsOneWay = true)]
		[EncryptableNetDataContractFormatAttribute]
		void Publish(IEvent @event);
	}
}