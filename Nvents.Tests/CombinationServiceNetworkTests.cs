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
            service.Subscribe<FooEvent>(e =>
            {
                if (e != null)
                    count++;
            });

            Test.WaitFor(() => count == 2, TimeSpan.FromSeconds(2), () =>
                service.Publish(new FooEvent()));

            Assert.True(count == 1, String.Format("UniqueEvent was raised {0} times, instead of once", count));
        }

        [Fact]
        public void DoesNotFilterDuplicateEventWhenUseEqualsOnly()
        {
            int count = 0;
            int expectedCount = service.Services.Count;
            service.DuplicateSuppression = DuplicateSuppressionOption.UseEquals;
            service.Subscribe<FooEvent>(e => count++);

            Test.WaitFor(() => count == expectedCount, TimeSpan.FromSeconds(5), () =>
                service.Publish(new FooEvent()));

            Assert.True(count >= expectedCount, String.Format("FooEvent was raised {0} times, expected {1}",count, expectedCount));
        }

        public CombinationServiceNetworkTests()
        {
            service = new CombinationService(
                new NetworkService(), new NetworkService());
            service.Start();
        }
	}
}
