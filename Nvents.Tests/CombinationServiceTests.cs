using System;
using System.Data.Odbc;
using System.Threading.Tasks;
using Nvents.Services;
using Xunit;

namespace Nvents.Tests
{
	public class CombinationServiceTests
	{
	    private readonly CombinationService service;

        [Fact]
        public void AllServicesStarted()
        {
            foreach (IService srv in service.Services)
                Assert.True(srv.IsStarted, "All Services weren't started");
        }

		[Fact]
		public void CanPublishCombinationEvent()
		{
		    int count = 0;
		    int expectedCount = service.Services.Count;
            service.DuplicateSuppression = DuplicateSuppressionOption.None;
			Events.Subscribe<FooEvent>(e => count++);

            Test.WaitFor(() => count == expectedCount, TimeSpan.FromSeconds(1), () =>
				Events.Publish(new FooEvent()));

            Assert.True(count == expectedCount, String.Format("FooEvent was not raised {0} times", expectedCount));
		}

        [Fact]
        public void CanFilterDuplicateEventWhenUnqiueOnly()
        {
            int count = 0;
            service.DuplicateSuppression = DuplicateSuppressionOption.UniqueOnly;
            Events.Subscribe<UniqueEvent>(e => count++);

            Test.WaitFor(() => count == 2, TimeSpan.FromSeconds(1), () =>
                Events.Publish(new UniqueEvent { ID = Guid.NewGuid()}));

            Assert.True(count == 1, "UniqueEvent was raised more than once");
        }
        
        [Fact]
        public void DoesNotFilterDuplicateEventWhenUniqueOnly()
        {
            int count = 0;
            int expectedCount = service.Services.Count;
            service.DuplicateSuppression = DuplicateSuppressionOption.UniqueOnly;
            Events.Subscribe<FooEvent>(e => count++);

            Test.WaitFor(() => count == expectedCount, TimeSpan.FromSeconds(1), () =>
                Events.Publish(new FooEvent()));

            Assert.True(count == expectedCount, String.Format("FooEvent was not raised {0} times", expectedCount));
        }

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

            Test.WaitFor(() => count == 2, TimeSpan.FromSeconds(1), () =>
                Events.Publish(new FooEvent()));

            Assert.True(count == 1, "UniqueEvent was raised more than once");
        }

        [Fact]
        public void ExpiresOldUniqueEvents()
        {
            int count = 0;
            service.UniqueCheckTimeLimit = TimeSpan.FromMilliseconds(10);
            service.DuplicateSuppression = DuplicateSuppressionOption.UniqueOnly;
            Events.Subscribe<UniqueEvent>(e => count++);

            UniqueEvent nvent = new UniqueEvent { ID = Guid.NewGuid() };
            Test.WaitFor(() => false, TimeSpan.FromSeconds(.1), () => Events.Publish(nvent));
            Test.WaitFor(() => count==2, TimeSpan.FromSeconds(1), () => Events.Publish(nvent));

            service.UniqueCheckTimeLimit = TimeSpan.FromMinutes(1);
            Assert.True(count == 2, String.Format("UniqueEvent was not raised second time after cache expiration"));
        }

        public CombinationServiceTests()
        {
            service = new CombinationService(
                new DummyService(), new DummyService());
            service.Start();
            Events.Service = service;
        }
	}
}
