using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SandboxSite.Startup))]
namespace SandboxSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
