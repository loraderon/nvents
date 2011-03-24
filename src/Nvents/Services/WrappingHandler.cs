using System;
using System.Linq;
using System.Reflection;

namespace Nvents.Services
{
	internal class WrappingHandler : IHandler<IEvent>
	{
		object handler;
		MethodInfo handle;

		public WrappingHandler(object handler)
		{
			this.handler = handler;
			handle = handler
				.GetType().GetMethods()
				.Where(x => x.Name == "Handle")
				.Where(x =>
				{
					return x.GetParameters()
						.Where(p => p.ParameterType.GetInterface("IEvent") == typeof(IEvent))
						.Count() == 1;
				})
				.FirstOrDefault();
			if (handle == null)
				throw new ArgumentException(string.Format("Handler {0} does not contain a method named Handle which accepts only one parameter with type IEvent.", handler.GetType().Name));
		}

		public void Handle(IEvent @event)
		{
			handle.Invoke(handler, new object[] { @event });
		}
	}
}
