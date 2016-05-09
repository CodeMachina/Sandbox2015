using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;
using Microsoft.Extensions.OptionsModel;
using System.Collections.Generic;
using NuGet;
using Microsoft.AspNet.Routing;
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNet.Mvc.Controllers;

namespace ProductNames.SingleActionController {
    public class SingleActionApplicationModelConvention : IApplicationModelConvention {
        private readonly SingleActionControllerOptions options;

        public SingleActionApplicationModelConvention(IOptions<SingleActionControllerOptions> optionsAccessor) {
            options = optionsAccessor.Value;
            if(!options.IsOptionsConfigured) {
                throw new Exception("SingleActionControllerOptions class not configured in Startup.");
            }
        }

        public void Apply(ApplicationModel application) {
            //Remove controllers from the application model that are not in the accepted namespace
            application.Controllers.RemoveAll(controller => !controller.ControllerType.FullName.StartsWith(options.Namespace));

            //Remove any actions found that aren't the accepted single action method name from the application model
            foreach(var controllerModel in application.Controllers) {
                controllerModel.Actions.RemoveAll(x => x.ActionName != options.ActionMethodName);
            }
        }
    }

    public interface IRegisterRoutes {
        void BuildRoutes(IRouteBuilder routeBuilder);
    }

    public class SingleActionControllerRouteBuilder : IRegisterRoutes {
        private readonly IControllerTypeProvider controllerTypeProvider;
        private readonly IApplicationModelProvider[] applicationModelProviders;
        private readonly IEnumerable<IApplicationModelConvention> conventions;
        private readonly SingleActionControllerOptions options;

        private IRouteBuilder routeBuilder;

        public SingleActionControllerRouteBuilder(IOptions<SingleActionControllerOptions> optionsAccessor, IControllerTypeProvider controllerTypeProvider, IEnumerable<IApplicationModelProvider> applicationModelProviders, IOptions<MvcOptions> mvcOptionsAccessor) {
            options = optionsAccessor.Value;
            conventions = mvcOptionsAccessor.Value.Conventions;
            this.controllerTypeProvider = controllerTypeProvider;
            this.applicationModelProviders = applicationModelProviders.OrderBy(p => p.Order).ToArray();                   
        }

        public void BuildRoutes(IRouteBuilder routeBuilder) {
            this.routeBuilder = routeBuilder;
            //TODO: this needs more thought
            //var applicationModel = BuildModel();
            //ApplicationModelConventions.ApplyConventions(applicationModel, conventions);


            //foreach(var controllerModel in applicationModel.Controllers.Where(x => x.ControllerType.Namespace.StartsWith(options.Namespace))) {
            //    if(controllerModel.ControllerType.Namespace.Equals(options.Namespace)) {
            //        var action = controllerModel.Actions.Where(x => x.ActionMethod.Name == options.ActionMethodName).FirstOrDefault();
                    



            //        var routeParameters = action.Parameters.Where(x => x.ParameterInfo.ParameterType.GetProperties().Where(y => y.IsDefined(typeof(FromRouteAttribute))).Any());
            //        routeBuilder.AddGetRoute("/", c => c.Action(controllerModel.Actions.Where(x=> x.ActionMethod.Name == options.ActionMethodName).FirstOrDefault().ActionMethod));
            //    }
            //}
        }

        internal protected ApplicationModel BuildModel() {
            var controllerTypes = controllerTypeProvider.ControllerTypes;
            var context = new ApplicationModelProviderContext(controllerTypes);

            for(var i = 0; i < applicationModelProviders.Length; i++) {
                applicationModelProviders[i].OnProvidersExecuting(context);
            }

            for(var i = applicationModelProviders.Length - 1; i >= 0; i--) {
                applicationModelProviders[i].OnProvidersExecuted(context);
            }

            return context.Result;
        }
    }
}
