using System;
using Xunit;
using Nvents.Services;
using System.Threading;

namespace Nvents.Tests
{
	public class EncryptionTests
	{
		[Fact]
		public void UnencryptedSerivceShouldNotGetEncryptedEvents()
		{
			var encrypted = new AutoNetworkService("encryption-key");
			encrypted.Start();
			var unecrypted = new AutoNetworkService();
			unecrypted.Start();

			bool raisedEncrypted = false;
			bool raisedUnencrypted = false;

			encrypted
				.Subscribe<FooEvent>(e => raisedEncrypted = true);
			unecrypted
				.Subscribe<FooEvent>(e => raisedUnencrypted = true);

			var started = DateTime.Now;
			encrypted.Publish(new FooEvent());

			var timeout = TimeSpan.FromSeconds(2);
			while ((DateTime.Now - started) < timeout)
			{
				Thread.Sleep(100);
			}

			Assert.True(raisedEncrypted, "Encrypted subscription was not raised.");
			Assert.False(raisedUnencrypted, "Unencrypted subscription was raised.");
		}
	}
}
