using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace Nvents.Services.Network
{
	public class EncryptableNetDataContractFormatAttribute : Attribute, IOperationBehavior
	{
		public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
		{ }

		public void ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
		{
			ReplaceDataContractSerializerOperationBehavior(description);
		}

		public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
		{
			ReplaceDataContractSerializerOperationBehavior(description);
		}
		public void Validate(OperationDescription description)
		{ }

		private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription description)
		{
			var dcs = description.Behaviors.Find<DataContractSerializerOperationBehavior>();

			if (dcs != null)
				description.Behaviors.Remove(dcs);

			var encryptionBehavior = description.Behaviors.Find<EncryptionBehavior>();

			description.Behaviors.Add(new EncryptableNetDataContractSerializerOperationBehavior(description, encryptionBehavior.EncryptionKey));
		}

		public class EncryptableNetDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
		{
			string encryptionKey;

			public EncryptableNetDataContractSerializerOperationBehavior(OperationDescription operationDescription, string encryptionKey)
				: base(operationDescription)
			{
				this.encryptionKey = encryptionKey;
			}

			public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
			{
				return new EncryptableNetDataContractSerializer(encryptionKey);
			}

			public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
			{
				return new EncryptableNetDataContractSerializer(encryptionKey);
			}
		}
	}
}