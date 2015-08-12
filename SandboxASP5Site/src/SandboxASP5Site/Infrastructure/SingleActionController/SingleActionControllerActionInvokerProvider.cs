using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System.Collections.Generic;
using System.Linq;

namespace SandboxASP5Site.Infrastructure.SingleActionController
{
    public class SingleActionControllerActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IControllerActionArgumentBinder _argumentBinder;
        private readonly IControllerFactory _controllerFactory;
        private readonly IFilterProvider[] _filterProviders;
        private readonly IReadOnlyList<IInputFormatter> _inputFormatters;
        private readonly IReadOnlyList<IModelBinder> _modelBinders;
        private readonly IReadOnlyList<IOutputFormatter> _outputFormatters;
        private readonly IReadOnlyList<IModelValidatorProvider> _modelValidatorProviders;
        private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        private readonly IScopedInstance<ActionBindingContext> _actionBindingContextAccessor;
        private readonly ITempDataDictionary _tempData;
        private readonly int _maxModelValidationErrors;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<SingleActionControllerOptions> _singleActionOptionsAccessor;

        public SingleActionControllerActionInvokerProvider(
            IControllerFactory controllerFactory,
            IEnumerable<IFilterProvider> filterProviders,
            IControllerActionArgumentBinder argumentBinder,
            IOptions<MvcOptions> optionsAccessor,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
            ITempDataDictionary tempData,
            ILoggerFactory loggerFactory,
            IOptions<SingleActionControllerOptions> singleActionOptionsAccessor)
        {
            _controllerFactory = controllerFactory;
            _filterProviders = filterProviders.OrderBy(item => item.Order).ToArray();
            _argumentBinder = argumentBinder;
            _inputFormatters = optionsAccessor.Options.InputFormatters.ToArray();
            _outputFormatters = optionsAccessor.Options.OutputFormatters.ToArray();
            _modelBinders = optionsAccessor.Options.ModelBinders.ToArray();
            _modelValidatorProviders = optionsAccessor.Options.ModelValidatorProviders.ToArray();
            _valueProviderFactories = optionsAccessor.Options.ValueProviderFactories.ToArray();
            _actionBindingContextAccessor = actionBindingContextAccessor;
            _maxModelValidationErrors = optionsAccessor.Options.MaxModelValidationErrors;
            _tempData = tempData;
            _loggerFactory = loggerFactory;
            _singleActionOptionsAccessor = singleActionOptionsAccessor;
        }

        public int Order
        {
            get
            {
                return DefaultOrder.DefaultFrameworkSortOrder;
            }
        }

        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {

        }

        public void OnProvidersExecuting(ActionInvokerProviderContext context)
        {
            var actionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;

            if (actionDescriptor != null)
            {
                context.Result = new SingleActionControllerActionInvoker(
                    context.ActionContext,
                    _filterProviders,
                    _controllerFactory,
                    actionDescriptor,
                    _inputFormatters,
                    _outputFormatters,
                    _argumentBinder,
                    _modelBinders,
                    _modelValidatorProviders,
                    _valueProviderFactories,
                    _actionBindingContextAccessor,
                    _tempData,
                    _loggerFactory,
                    _maxModelValidationErrors,
                    _singleActionOptionsAccessor);
            }
        }
    }
}
