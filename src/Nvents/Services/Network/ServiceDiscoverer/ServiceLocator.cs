#if NET35
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nvents.Services.Network.ServiceDiscoverer
{
	public static class ServiceLocator
	{
		public static HostResponse[] LocateService(IPAddress group, int port, Guid serviceId, TimeSpan timeout)
		{
			if (timeout <= TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("timeout");

			using (var sender = new UdpClient())
			{
				byte[] request = serviceId.ToByteArray();
				var groupEP = new IPEndPoint(group, port);
				try
				{
					sender.Send(request, request.Length, groupEP);
				}
				catch (SocketException ex)
				{
					return new HostResponse[0];
				}

				// Accumulate responses on a threadpool thread
				var rap = new ResponseAccumProc(ResponseAccumProcImpl);
				var ar = rap.BeginInvoke(sender, serviceId, null, null);

				// Wait the requisite amount of time, then shut the door
				System.Threading.Thread.Sleep(timeout);
				sender.Close();

				// waits for async delegate to complete
				return rap.EndInvoke(ar);
			}
		}

		private delegate HostResponse[] ResponseAccumProc(UdpClient udpClient, Guid serviceId);

		private static HostResponse[] ResponseAccumProcImpl(UdpClient udpClient, Guid serviceId)
		{
			var responses = new List<HostResponse>();

			try
			{
				while (true)
				{
					IPEndPoint remoteEndpoint = null;
					var response = udpClient.Receive(ref remoteEndpoint); //blocks until socket closed

					using (var responseStream = new MemoryStream(response, false))
					{
						if (response.Length < 16)
							continue;

						var data = new byte[16];
						responseStream.Read(data, 0, 16);
						var guid = new Guid(data);
						if (guid != serviceId)
							continue;

						try
						{
							var bf = new BinaryFormatter();
							var endpointProps = (IDictionary)bf.Deserialize(responseStream);
							responses.Add(new HostResponse(remoteEndpoint.Address, endpointProps));
						}
						catch (Exception)
						{
							responses.Add(new HostResponse(remoteEndpoint.Address, null));
						}
					}
				}
			}
			catch (ObjectDisposedException)
			{ }
			catch (SocketException)
			{ }

			return responses.ToArray();
		}
	}
}
#endif