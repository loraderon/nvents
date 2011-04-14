using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Nvents.Services.Network;
using Xunit;

namespace Nvents.Tests
{
	public class EncryptionTests
	{
		[Fact]
		public void UnencryptedSerializerCannotDeserializeEncryptedEvent()
		{
			var e = new FooEvent { Baz = "Bar" };
			string serializedEncrypted = Serialize(encrypted, e);

			Assert.Throws<SerializationException>(() => 
				Deserialize(plain, serializedEncrypted));
		}

		[Fact]
		public void EncryptedSerializerCanDeserializeEncryptedEvent()
		{
			var e = new FooEvent { Baz = "Bar" };
			string serializedEncrypted = Serialize(encrypted, e);

			var deserialized = Deserialize(encrypted, serializedEncrypted) as FooEvent;
			
			Assert.NotNull(deserialized);
			Assert.Equal("Bar", deserialized.Baz);
		}

		private string Serialize(EncryptableNetDataContractSerializer serializer, object graph)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
				{
					serializer.WriteStartObject(writer, graph);
					serializer.WriteObjectContent(writer, graph);
					serializer.WriteEndObject(writer);
					writer.Flush();
					return Encoding.UTF8.GetString(stream.ToArray());
				}
			}
		}

		private object Deserialize(EncryptableNetDataContractSerializer serializer, string data)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				using (var reader = new XmlTextReader(stream))
				{
					return serializer.ReadObject(reader);
				}
			}
		}

		EncryptableNetDataContractSerializer encrypted = new EncryptableNetDataContractSerializer("encryption-key");
		EncryptableNetDataContractSerializer plain = new EncryptableNetDataContractSerializer(null);
	}
}
