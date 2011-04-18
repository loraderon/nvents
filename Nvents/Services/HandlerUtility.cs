using System;
using System.Linq;

namespace Nvents.Services
{
	internal static class HandlerUtility
	{
		public static Type[] GetHandlerEventTypes(object handler)
		{
			var eventTypes =
				from m in handler
					.GetType()
					.GetMethods()
				where m.Name == "Handle"
				let parameters = m.GetParameters()
				where parameters.Length == 1
				from p in parameters
				where p.ParameterType
					.GetInterface("IEvent") == typeof(IEvent)
				select p.ParameterType;

			if (eventTypes.Count() == 0)
				throw new ArgumentException(string.Format("Handler {0} does not contain any methods named Handle which accepts only one parameter with type that derives from IEvent.", handler.GetType().Name));
			return eventTypes.ToArray();
		}
	}
}
