using System;
using System.Linq;

namespace Nvents.Services
{
	internal static class HandlerUtility
	{
		/// <summary>
		/// Gets the event types that the specified event handler handles
		/// </summary>
		/// <param name="handler">The event handler</param>
		/// <returns>The type of events that are handled</returns>
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
					.IsClass
				select p.ParameterType;

			if (!eventTypes.Any())
				throw new ArgumentException(string.Format("Handler {0} does not contain any methods named Handle which accepts only one parameter that is a class (not a value type).", handler.GetType().Name));
			return eventTypes.ToArray();
		}
	}
}
