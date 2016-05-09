using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Framework.Configuration;
using ProductNames.DataManagement.Configuration;
using ProductNames.SingleActionController;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.PlatformAbstractions;

namespace ProductNames {
    public sealed class Startup {
        public Startup(IApplicationEnvironment appEnv) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services) {
            services.Configure<SingleActionControllerOptions>(options => {
                options.Assembly = typeof(Startup).GetTypeInfo().Assembly;
                options.Namespace = "ProductNames.ControllersSingleAction";
                options.ActionMethodName = "Execute";
            });

            services.AddMvc();
            services.AddCustomRouteRegistration()
                .Configure<MvcOptions>(options => {
                    options.EnableSingleActionControllerRouting(services.BuildServiceProvider());                  
                });

            services.AddDataManagementForADOSqlite(Configuration);
            //services.AddDataManagementForEntityFrameworkSqlite(Configuration);        
        }

        public void Configure(IApplicationBuilder app) {
            app.UseMvc(routes => {
                routes.BuildRouteConventions(app.ApplicationServices);
            });
        }
    }
}
