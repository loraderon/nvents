using System;
using System.Linq;

namespace Nvents.Services
{
	internal static class HandlerUtility
	{
		public static Type GetHandlerEventType(object handler)
		{
			Type eventType = null;
			handler
			   .GetType().GetMethods()
			   .Where(x => x.Name == "Handle")
			   .Where(x =>
			   {
				   var parameters = x.GetParameters();
				   if (parameters.Length != 1)
					   return false;
				   var param = parameters[0];
				   if (param.ParameterType.GetInterface("IEvent") != typeof(IEvent))
					   return false;
				   eventType = param.ParameterType;
				   return true;
			   })
			   .FirstOrDefault();
			if (eventType == null)
				throw new ArgumentException(string.Format("Handler {0} does not contain a method named Handle which accepts only one parameter with type that derives from IEvent.", handler.GetType().Name));
			return eventType;
		}
	}
}
