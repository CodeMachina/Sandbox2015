using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SandboxASP5Site.Infrastructure.SingleActionController
{
    /// <summary>
    /// Overrides the Default MVC Controller invoker
    /// </summary>
    public class SingleActionControllerActionInvoker : ControllerActionInvoker
    {
        private readonly ControllerActionDescriptor _descriptor;
        private readonly Assembly assembly;
        private readonly string @namespace;

        public SingleActionControllerActionInvoker(
            ActionContext actionContext,
            IReadOnlyList<IFilterProvider> filterProviders,
            IControllerFactory controllerFactory,
            ControllerActionDescriptor descriptor,
            IReadOnlyList<IInputFormatter> inputFormatters,
            IReadOnlyList<IOutputFormatter> outputFormatters,
            IControllerActionArgumentBinder controllerActionArgumentBinder,
            IReadOnlyList<IModelBinder> modelBinders,
            IReadOnlyList<IModelValidatorProvider> modelValidatorProviders,
            IReadOnlyList<IValueProviderFactory> valueProviderFactories,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
            ITempDataDictionary tempData,
            ILoggerFactory loggerFactory,
            int maxModelValidationErrors,
            IOptions<SingleActionControllerOptions> optionsAccessor)
            : base(
                  actionContext,
                  filterProviders,
                  controllerFactory,
                  descriptor,
                  inputFormatters,
                  outputFormatters,
                  controllerActionArgumentBinder,
                  modelBinders,
                  modelValidatorProviders,
                  valueProviderFactories,
                  actionBindingContextAccessor,
                  tempData,
                  loggerFactory,
                  maxModelValidationErrors
                  )
        {
            _descriptor = descriptor;
            assembly = optionsAccessor.Options.Assembly;
            @namespace = optionsAccessor.Options.Namespace;
        }

        /// <summary>
        /// Ultimately invokes the method Execute on a controller
        /// </summary>
        /// <param name="actionExecutingContext"></param>
        /// <returns></returns>
        protected override async Task<IActionResult> InvokeActionAsync(ActionExecutingContext actionExecutingContext)
        {
            //TODO: Refactor this out as it is shared with SingleActionControllerFactory
            var controllerPath = string.Format("{0}.{1}.{2}", @namespace, actionExecutingContext.RouteData.Values["controller"], actionExecutingContext.RouteData.Values["action"]);
            var actionMethodInfo = assembly.GetType(controllerPath, true, true).GetMethod("Execute");

            if (actionMethodInfo == null)
            {
                var message = "Method Execute not implemented for Controller " + _descriptor.MethodInfo.DeclaringType.ToString();
                throw new NotImplementedException(message);
            }

            var actionReturnValue = await ControllerActionExecutor.ExecuteAsync(
                actionMethodInfo,
                actionExecutingContext.Controller,
                actionExecutingContext.ActionArguments);

            var actionResult = CreateActionResult(
                actionMethodInfo.ReturnType,
                actionReturnValue);
            return actionResult;
        }

        internal static IActionResult CreateActionResult(Type declaredReturnType, object actionReturnValue)
        {
            // optimize common path
            var actionResult = actionReturnValue as IActionResult;
            if (actionResult != null)
            {
                return actionResult;
            }

            if (declaredReturnType == typeof(void) ||
                declaredReturnType == typeof(Task))
            {
                return new EmptyResult();
            }

            // Unwrap potential Task<T> types.
            var actualReturnType = GetTaskInnerTypeOrNull(declaredReturnType) ?? declaredReturnType;
            if (actionReturnValue == null &&
                typeof(IActionResult).GetTypeInfo().IsAssignableFrom(actualReturnType.GetTypeInfo()))
            {
                //TODO: Write a better exception
                throw new InvalidOperationException("Kaboom");
            }

            return new ObjectResult(actionReturnValue)
            {
                DeclaredType = actualReturnType
            };
        }

        private static Type GetTaskInnerTypeOrNull(Type type)
        {
            var genericType = ExtractGenericInterface(type, typeof(Task<>));

            return genericType?.GenericTypeArguments[0];
        }

        public static Type ExtractGenericInterface(Type queryType, Type interfaceType)
        {
            Func<Type, bool> matchesInterface =
                type => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == interfaceType;
            if (matchesInterface(queryType))
            {
                // Checked type matches (i.e. is a closed generic type created from) the open generic type.
                return queryType;
            }

            // Otherwise check all interfaces the type implements for a match.
            return queryType.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(matchesInterface);
        }
    }
}
