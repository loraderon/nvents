using System;
using System.Linq;
using Nvents;
using Ninject;
using Ninject.Modules;
using Nvents.Services;

namespace Chat.Moderator
{
    public class NventsModule : NinjectModule
    {
        public override void Load()
        {
            // Bind IPublisher to the default service
            Bind<IPublisher>()
                .ToConstant(Events.Service);

            foreach (var type in System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetInterface(typeof(IHandler<>).Name) != null))
            {
                var handler = Kernel.Get(type);
                Events.RegisterHandler(handler);
            }
        }
    }
}
