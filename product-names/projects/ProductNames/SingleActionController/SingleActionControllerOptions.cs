using System.Reflection;

namespace ProductNames.SingleActionController {
    public class SingleActionControllerOptions {
        public Assembly Assembly { get; set; }

        public string Namespace { get; set; }

        public string ActionMethodName { get; set; }

        public bool IsOptionsConfigured {
            get {
                return (Assembly != null && !string.IsNullOrWhiteSpace(Namespace) && !string.IsNullOrWhiteSpace(ActionMethodName)) ? true : false;
            }
        }
    }
}
