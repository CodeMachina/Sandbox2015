using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;

namespace SandboxASP5Site.Infrastructure
{
    public interface IRegisterRoutes
    {
        void BuildRoutes(IRouteBuilder builder);
    }

    public class RegisterRoutes : IRegisterRoutes
    {
        public void BuildRoutes(IRouteBuilder builder)
        {
            //TODO: remove route
            builder.MapRoute(name: "blah",
                template: "blah/blah/blah",
                defaults: new {controller = "Home", action = "About" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint(new[] { "GET" }) }
            );

            builder.MapRoute(name: "default",
                template: "{controller=Home}/{action=Index}/{id?}"
            );           
        }
    }
}
