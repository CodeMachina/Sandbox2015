using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.Framework.DependencyInjection;

namespace SandboxASP5Site.Infrastructure.SingleActionController
{
    public static class SingleActionControllerServiceCollectionExtensions
    {
        public static IServiceCollection AddMvcSingleActionController(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IActionInvokerProvider, SingleActionControllerActionInvokerProvider>());
            services.AddSingleton<IControllerFactory, SingleActionControllerFactory>();

            return services;
        }
    }
}
