using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using System.Collections.Generic;
using System.Linq;

namespace SandboxASP5Site.Infrastructure
{
    public class HttpMethodRouteConstraint : IRouteConstraint
    {
        private readonly IEnumerable<string> methods;

        public HttpMethodRouteConstraint(IEnumerable<string> methods)
        {
            this.methods = methods;
        }
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, IDictionary<string, object> values, RouteDirection routeDirection)
        {
            return methods.Contains(httpContext.Request.Method);
        }
    }
}
