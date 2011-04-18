using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Chat.Moderator.ViewModels;
using Ninject;

namespace Chat.Moderator
{
	public class Bootstrapper : Bootstrapper<ShellViewModel>	
	{
		public static IKernel kernel;

		protected override void Configure()
		{
			kernel = new StandardKernel();
			kernel
				.Bind<ShellViewModel>()
				.ToSelf()
				.InSingletonScope();
			kernel.Load(new NventsModule());
		}

		protected override object GetInstance(Type service, string key)
		{
			return kernel.Get(service, key) ??
				base.GetInstance(service, key);
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return kernel.GetAll(service) ??
				base.GetAllInstances(service);
		}
	}
}
