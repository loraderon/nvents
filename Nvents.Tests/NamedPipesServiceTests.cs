using System;
using Nvents.Services;
using Xunit;

namespace Nvents.Tests
{
	public class NamedPipesServiceTests
	{
		[Fact]
		public void CanPublishEvent()
		{
			var raised = false;
			Events.Subscribe<FooEvent>(
				e => raised = true);

			Test.WaitFor(() => raised, TimeSpan.FromSeconds(3), () =>
				Events.Publish(new FooEvent()));

			Assert.True(raised, "FooEvent was not raised");
		}

		[Fact]
		public void CanHaveMultipleInstances()
		{
			var otherRaised = false;
			var originalRaised = false;
			var other = new NamedPipesService();

			other.Subscribe<FooEvent>(
				e => otherRaised = true);
			Events.Subscribe<FooEvent>(
				e => originalRaised = true);

			Test.WaitFor(() => otherRaised && originalRaised, TimeSpan.FromSeconds(3), () =>
				Events.Publish(new FooEvent()));

			Assert.True(originalRaised, "FooEvent was not raised on original service");
			Assert.True(otherRaised, "FooEvent was not raised on other service");
		}

		public NamedPipesServiceTests()
		{
			Events.Service = new NamedPipesService();
		}
	}
}
