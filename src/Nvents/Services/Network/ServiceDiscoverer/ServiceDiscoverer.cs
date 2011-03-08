#if NET35
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Nvents.Services.Network.ServiceDiscoverer
{
	public class ServiceDiscoverer
	{
		static readonly Guid UDP_SERVICE_ID = new Guid("{DFCB6ED4-E42A-425B-BF9B-6ABF880A730A}");
		static readonly IPAddress UDP_IP_GROUP = IPAddress.Parse("239.255.255.250");
		const int UDP_IP_PORT = 3702;

		public static Service[] FindServices(TimeSpan timeout)
		{
			var responses = ServiceLocator.LocateService(UDP_IP_GROUP, UDP_IP_PORT, UDP_SERVICE_ID, timeout);

			var services = new List<Service>();			
			foreach (var response in responses)
			{
				var service = ConvertResponseToService(response);
				services.Add(service);
			}
			return services.ToArray();
		}

		private static Service ConvertResponseToService(HostResponse response)
		{
			string ipaddress = response.EndpointProperties["ipaddress"] as string;
			int port = Int32.Parse(response.EndpointProperties["port"] as string);
			var guid = new Guid(response.EndpointProperties["guid"] as string);

			return new Service(ipaddress, port, guid);
		}

		ServicePublisher sp;

		public void Start(string ipaddress, int port, Guid guid)
		{
			if (sp != null)
				throw new NotSupportedException("Service publisher already started");

			sp = new ServicePublisher(UDP_SERVICE_ID, UDP_IP_GROUP);
			var props = new Hashtable();
			props.Add("ipaddress", ipaddress);
			props.Add("port", port.ToString());
			props.Add("guid", guid.ToString());

			sp.PublishServiceEndpoint(props, UDP_IP_PORT, 3);
		}

		public void Stop()
		{
			if (sp == null)
				return;
			sp.Stop();
			sp = null;
		}
	}
}
#endif