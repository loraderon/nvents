using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;

namespace Nvents.Services.Network
{
	public class EncryptableNetDataContractSerializer : XmlObjectSerializer
	{
		static NetDataContractSerializer serializer = new NetDataContractSerializer();
		string encryptionKey;

		public EncryptableNetDataContractSerializer(string encryptionKey)
		{
			this.encryptionKey = encryptionKey;
		}

		public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
		{
			writer.WriteStartElement("data");
		}

		public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
		{
			var data = Serialize(graph);
			if (UseEncryption())
				data = Encrypt(data);
			var content = Convert.ToBase64String(data);
			writer.WriteValue(content);
		}

		public override void WriteEndObject(XmlDictionaryWriter writer)
		{
			writer.WriteEndElement();
		}

		public override bool IsStartObject(XmlDictionaryReader reader)
		{
			return true;
		}

		public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
		{
			string content = reader.ReadElementContentAsString();
			var data = Convert.FromBase64String(content);
			if (UseEncryption())
				data = Decrypt(data);
			var graph = Deserialize(data);
			return graph;
		}

		private byte[] Serialize(object graph)
		{
			using (var stream = new MemoryStream())
			{
				serializer.Serialize(stream, graph);
				return stream.ToArray();
			}
		}

		private object Deserialize(byte[] data)
		{
			using (var stream = new MemoryStream(data))
			{
				return serializer.Deserialize(stream);
			}
		}

		private byte[] Encrypt(Byte[] data)
		{
			var alg = CreateSymmetricAlgorithm(encryptionKey);
			return TransformCryptoBytes(alg.CreateEncryptor(), data);
		}

		private byte[] Decrypt(Byte[] data)
		{
			var alg = CreateSymmetricAlgorithm(encryptionKey);
			return TransformCryptoBytes(alg.CreateDecryptor(), data);
		}

		private SymmetricAlgorithm CreateSymmetricAlgorithm(string password)
		{
			var pdb = new PasswordDeriveBytes(password, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			try
			{
				var AESE = new AesManaged();
				AESE.Key = pdb.GetBytes(AESE.KeySize / 8);
				AESE.IV = pdb.GetBytes(AESE.BlockSize / 8);
				return AESE;
			}
			finally
			{
#if !NET35
				if (pdb != null)
					pdb.Dispose();
#endif
			}
		}

		private byte[] TransformCryptoBytes(ICryptoTransform transform, byte[] data)
		{
			using (var ms = new MemoryStream())
			{
				var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
				cs.Write(data, 0, data.Length);
				cs.Close();
				return ms.ToArray();
			}
		}

		private bool UseEncryption()
		{
			return !string.IsNullOrEmpty(encryptionKey);
		}
	}
}
