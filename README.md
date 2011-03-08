[nvents](http://nvents.org/) - Open source library for strongly typed publishing/subscribing of events over the network.
----------------
Release history:
----------------
 0.6 2011-03-08
  * Added support for .NET 3.5 (custom udp based service locator) [nvents 0.6 now runs on .NET 3.5 and 4.0] [3]
 0.5 2011-02-07
  * Fixed Issue #1 "WCF Timeout Exception"
  * Added EventSubscription<TEvent> to simplify standard .NET events
 0.4 2010-12-13
  * Increased performance and added inheritance support [Performance improvements and inheritance support in nvents 0.4] [2]
 0.3 2010-11-09
  * Added encryption, handlers and filters [Encryption, handlers and filters in version 0.3] [1]
 0.2 2010-10-18
  * Fixed security exception
  * NetworkService to explicity set IPAddress and port
  * Only requiring .NET Framework 4 Client Profile
 0.1 2010-10-12
  * initial release

  [1]: http://nvents.org/post/2010/11/09/Encryption-handlers-and-filters-in-version-03.aspx
  [2]: http://nvents.org/post/2010/12/13/Performance-improvements-and-inheritance-support-in-nvents-04.aspx
  [3]: http://nvents.org/post/2011/03/09/nvents-06-now-runs-on-NET-35-and-40.aspx

Sample code

	using System;
	using Nvents;
	
	namespace NventsSample
	{
	  class Program
	  {
		static void Main(string[] args)
		{
		  // subscribe to events
		  Events.Subscribe<FooEvent>(
			e => Console.WriteLine(e.Bar));

		  // publish events
		  Events.Publish(new FooEvent { Bar = "FooBar" });

		  Console.ReadLine();
		}
	  }

	  public class FooEvent : IEvent
	  {
		public string Bar { get; set; }
	  }
	}