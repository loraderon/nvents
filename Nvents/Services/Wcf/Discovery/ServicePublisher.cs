#if NET35
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Nvents.Services.Wcf.Discovery
{
	public sealed class ServicePublisher
	{
		readonly Guid serviceId;
		readonly IPAddress group;
		UdpClient listener;
		byte[] response;

		public ServicePublisher(Guid serviceId, IPAddress group)
		{
			this.serviceId = serviceId;
			this.group = group;
		}

		public void PublishServiceEndpoint(IDictionary endpointProps, int port, int ttl)
		{
			var bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				ms.Write(serviceId.ToByteArray(), 0, 16);
				if (endpointProps != null)
					bf.Serialize(ms, endpointProps);
				response = ms.ToArray();
			}
			listener = new UdpClient();
			listener.ExclusiveAddressUse = false;

			listener.JoinMulticastGroup(group, ttl);
			var localEp = new IPEndPoint(IPAddress.Any, port);

			listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			listener.Client.Bind(localEp);


			var listenerThread = new Thread(ThreadProc);
			listenerThread.IsBackground = true;
			listenerThread.Start();
		}

		public void Stop()
		{
			if (listener == null)
				return;
			listener.DropMulticastGroup(group);
			listener.Close();
			listener = null;
		}

		private void ThreadProc()
		{
			try
			{
				while (true)
				{
					try
					{
						// Wait for broadcast... will block until data recv'd, or underlying socket is closed
						IPEndPoint callerEndpoint = null;
						byte[] request = listener.Receive(ref callerEndpoint);

						if (request.Length < 16)
							continue;

						byte[] temp = new byte[16];
						request.CopyTo(temp, 0);
						var requestGuid = new Guid(temp);

						if (requestGuid != serviceId)
							continue;

						// Send response (our guid, followed by serialized endpoint info)
						listener.Send(response, response.Length, callerEndpoint);
					}
					catch (SocketException)
					{ }
				}
			}
			catch (ObjectDisposedException)
			{ }
			catch (NullReferenceException)
			{ }
		}
	}
}
#endif