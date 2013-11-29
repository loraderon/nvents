using System;
using System.Data.Odbc;
using System.Threading.Tasks;
using Nvents.Services;
using Xunit;

namespace Nvents.Tests
{
	public class CombinationServiceNetworkTests
	{
	    private readonly CombinationService service;

        [Fact]
        public void CanFilterDuplicateEventWhenSuppressAll()
        {
            int count = 0;
            service.DuplicateSuppression = DuplicateSuppressionOption.All;
            Events.Subscribe<FooEvent>(e =>
            {
                if (e != null)
                    count++;
            });

            Test.WaitFor(() => count == 2, TimeSpan.FromSeconds(2), () =>
                Events.Publish(new FooEvent()));

            Assert.True(count == 1, String.Format("UniqueEvent was raised {0} times, instead of once", count));
        }

        [Fact]
        public void DoesNotFilterDuplicateEventWhenUseEqualsOnly()
        {
            int count = 0;
            int expectedCount = service.Services.Count;
            service.DuplicateSuppression = DuplicateSuppressionOption.UseEquals;
            Events.Subscribe<FooEvent>(e => count++);

            Test.WaitFor(() => count == expectedCount, TimeSpan.FromSeconds(1), () =>
                Events.Publish(new FooEvent()));

            Assert.True(count == expectedCount, String.Format("FooEvent was not raised {0} times", expectedCount));
        }

        public CombinationServiceNetworkTests()
        {
            service = new CombinationService(
                new NetworkService(), new NetworkService());
            service.Start();
            Events.Service = service;
        }
	}
}
