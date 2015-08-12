using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SandboxASP5Site.Infrastructure.SingleActionController
{
    public class SingleActionControllerFactory : IControllerFactory
    {
        private readonly IControllerActivator controllerActivator;
        private readonly IEnumerable<IControllerPropertyActivator> propertyActivators;

        private readonly Assembly assembly;
        private readonly string @namespace;

        public SingleActionControllerFactory(IControllerActivator controllerActivator, IEnumerable<IControllerPropertyActivator> propertyActivators, IOptions<SingleActionControllerOptions> optionsAccessor)
        {
            this.controllerActivator = controllerActivator;
            this.propertyActivators = propertyActivators;
            assembly = optionsAccessor.Options.Assembly;
            @namespace = optionsAccessor.Options.Namespace;

        }

        public object CreateController(ActionContext actionContext)
        {
            //TODO: Refactor this out as it is shared with SingleActionControllerActionInvoker
            var controllerPath = string.Format("{0}.{1}.{2}", @namespace, actionContext.RouteData.Values["controller"], actionContext.RouteData.Values["action"]);
            var controller = controllerActivator.Create(actionContext, assembly.GetType(controllerPath, true, true));

            foreach (var prop in propertyActivators)
            {
                prop.Activate(actionContext, controller);
            }

            return controller;
        }

        public void ReleaseController(object controller)
        {
            var disposableController = controller as IDisposable;

            if (disposableController != null)
            {
                disposableController.Dispose();
            }
        }
    }
}
