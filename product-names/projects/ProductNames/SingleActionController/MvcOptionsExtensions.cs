using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using System;

namespace ProductNames.SingleActionController {
    public static class MvcOptionsExtensions {
        public static void EnableSingleActionControllerRouting(this MvcOptions opts, IServiceProvider serviceProvider) {
            opts.Conventions.Add(new SingleActionApplicationModelConvention(serviceProvider.GetService(typeof(IOptions<SingleActionControllerOptions>)) as IOptions<SingleActionControllerOptions>));
        }       
    }

    public static class MvcServicesCollectionExtensions {
        public static IServiceCollection AddCustomRouteRegistration(this IServiceCollection services) {
            services.AddSingleton<IRegisterRoutes, SingleActionControllerRouteBuilder>();
            return services;
        }
    }

    public static class RouteBuilderExtensions {
        public static void BuildRouteConventions(this IRouteBuilder builder, IServiceProvider serviceProvider) {
            var routeRegister = serviceProvider.GetRequiredService(typeof(IRegisterRoutes)) as IRegisterRoutes;

            routeRegister.BuildRoutes(builder);
        }
    }
}
