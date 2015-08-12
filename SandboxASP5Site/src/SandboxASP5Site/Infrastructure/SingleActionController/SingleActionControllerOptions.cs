using System.Reflection;

namespace SandboxASP5Site.Infrastructure.SingleActionController
{
    public class SingleActionControllerOptions
    {
        public Assembly Assembly { get; set; }

        public string Namespace { get; set; }
    }
}
