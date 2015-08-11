using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.Facebook;
using Microsoft.AspNet.Authentication.Google;
using Microsoft.AspNet.Authentication.MicrosoftAccount;
using Microsoft.AspNet.Authentication.Twitter;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Routing;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using Microsoft.Framework.Runtime;
using SandboxASP5Site.Models;
using SandboxASP5Site.Services;
using SandboxASP5Site.Infrastructure;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.OptionsModel;
using System.Reflection;

namespace SandboxASP5Site
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv, IApplicationShutdown shutdown)
        {         
            // Setup configuration sources.

            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Entity Framework services to the services container.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure the options for the authentication middleware.
            // You can add options for Google, Twitter and other middleware as shown below.
            // For more information see http://go.microsoft.com/fwlink/?LinkID=532715
            services.Configure<FacebookAuthenticationOptions>(options =>
            {
                options.AppId = Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            services.Configure<MicrosoftAccountAuthenticationOptions>(options =>
            {
                options.ClientId = Configuration["Authentication:MicrosoftAccount:ClientId"];
                options.ClientSecret = Configuration["Authentication:MicrosoftAccount:ClientSecret"];
            });

            // Add MVC services to the services container.         
            services.AddMvc();

            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            // Register application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSingleton<IRegisterRoutes, RegisterRoutes>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IActionInvokerProvider, SingleActionControllerActionInvokerProvider>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IActionDescriptorProvider, SingleActionControllerActionDescriptorProvider>());
            services.AddSingleton<IControllerFactory, SingleActionControllerFactory>();
            services.AddSingleton<IActionInvoker, SingleActionControllerActionInvoker>();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IRegisterRoutes registerRoutes)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();

            // Configure the HTTP request pipeline.

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseErrorPage();
                app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseErrorHandler("/Home/Error");
            }

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline.
            app.UseIdentity();

            // Add authentication middleware to the request pipeline. You can configure options such as Id and Secret in the ConfigureServices method.
            // For more information see http://go.microsoft.com/fwlink/?LinkID=532715
            // app.UseFacebookAuthentication();
            // app.UseGoogleAuthentication();
            // app.UseMicrosoftAccountAuthentication();
            // app.UseTwitterAuthentication();

            // Add MVC to the request pipeline.
            app.UseMvc(registerRoutes.BuildRoutes);
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");

            //    // Uncomment the following line to add a route for porting Web API 2 controllers.
            //    // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            //});
        }
    }

    public class SingleActionControllerFactory : IControllerFactory
    {
        private readonly IControllerActivator controllerActivator;
        private readonly IEnumerable<IControllerPropertyActivator> propertyActivators;

        public SingleActionControllerFactory(IControllerActivator controllerActivator, IEnumerable<IControllerPropertyActivator> propertyActivators)
        {
            this.controllerActivator = controllerActivator;
            this.propertyActivators = propertyActivators;

        }

        public object CreateController(ActionContext actionContext)
        {
            actionContext.RouteData.Values["action"] = "Execute";
            var actionDescriptor = actionContext.ActionDescriptor as ControllerActionDescriptor;

            if(actionDescriptor == null)
            {
                throw new ArgumentException("Explosion");
            }
            
            var controller = controllerActivator.Create(actionContext, actionDescriptor.ControllerTypeInfo.AsType());

            foreach (var prop in propertyActivators)
            {
                prop.Activate(actionContext, controller);
            }

            return controller;
        }

        public void ReleaseController(object controller)
        {
            var disposableController = controller as IDisposable;

            if(disposableController != null)
            {
                disposableController.Dispose();
            }
        }
    }

    public class SingleActionControllerActionDescriptorProvider : IActionDescriptorProvider
    {
        public int Order
        {
            get
            {
                return DefaultOrder.DefaultFrameworkSortOrder;
            }
        }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
            throw new NotImplementedException();
        }

        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class SingleActionControllerActionDescriptor : ActionDescriptor
    {

    }

    public class SingleActionControllerActionInvokerProvider : IActionInvokerProvider
    {
        //private readonly IControllerActionArgumentBinder _argumentBinder;
        //private readonly IControllerFactory _controllerFactory;
        //private readonly IFilterProvider[] _filterProviders;
        //private readonly IReadOnlyList<IInputFormatter> _inputFormatters;
        //private readonly IReadOnlyList<IModelBinder> _modelBinders;
        //private readonly IReadOnlyList<IOutputFormatter> _outputFormatters;
        //private readonly IReadOnlyList<IModelValidatorProvider> _modelValidatorProviders;
        //private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        //private readonly IScopedInstance<ActionBindingContext> _actionBindingContextAccessor;
        //private readonly ITempDataDictionary _tempData;
        //private readonly int _maxModelValidationErrors;
        //private readonly ILoggerFactory _loggerFactory;

        //public SingleActionControllerActionInvokerProvider(
        //    IControllerFactory controllerFactory,
        //    IEnumerable<IFilterProvider> filterProviders,
        //    IControllerActionArgumentBinder argumentBinder,
        //    IOptions<MvcOptions> optionsAccessor,
        //    IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
        //    ITempDataDictionary tempData,
        //    ILoggerFactory loggerFactory)
        //    :base (
        //         controllerFactory,
        //         filterProviders,
        //         argumentBinder,
        //         optionsAccessor,
        //         actionBindingContextAccessor,
        //         tempData,
        //         loggerFactory)
        //{
        //    _controllerFactory = controllerFactory;
        //    _filterProviders = filterProviders.OrderBy(item => item.Order).ToArray();
        //    _argumentBinder = argumentBinder;
        //    _inputFormatters = optionsAccessor.Options.InputFormatters.ToArray();
        //    _outputFormatters = optionsAccessor.Options.OutputFormatters.ToArray();
        //    _modelBinders = optionsAccessor.Options.ModelBinders.ToArray();
        //    _modelValidatorProviders = optionsAccessor.Options.ModelValidatorProviders.ToArray();
        //    _valueProviderFactories = optionsAccessor.Options.ValueProviderFactories.ToArray();
        //    _actionBindingContextAccessor = actionBindingContextAccessor;
        //    _maxModelValidationErrors = optionsAccessor.Options.MaxModelValidationErrors;
        //    _tempData = tempData;
        //    _loggerFactory = loggerFactory;
        //}

        //public new void OnProvidersExecuting(ActionInvokerProviderContext context)
        //{
        //    var actionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;

        //    if (actionDescriptor != null)
        //    {
        //        context.Result = new SingleActionControllerActionInvoker(
        //                            context.ActionContext,
        //                            _filterProviders,
        //                            _controllerFactory,
        //                            actionDescriptor,
        //                            _inputFormatters,
        //                            _outputFormatters,
        //                            _argumentBinder,
        //                            _modelBinders,
        //                            _modelValidatorProviders,
        //                            _valueProviderFactories,
        //                            _actionBindingContextAccessor,
        //                            _tempData,
        //                            _loggerFactory,
        //                            _maxModelValidationErrors);
        //    }
        //}
        public int Order
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {
            throw new NotImplementedException();
        }

        public void OnProvidersExecuting(ActionInvokerProviderContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class SingleActionControllerActionInvoker : IActionInvoker
    {
        //private readonly ControllerActionDescriptor _descriptor;
        //public SingleActionControllerActionInvoker(
        //    ActionContext actionContext,
        //    IReadOnlyList<IFilterProvider> filterProviders,
        //    IControllerFactory controllerFactory,
        //    ControllerActionDescriptor descriptor,
        //    IReadOnlyList<IInputFormatter> inputFormatters,
        //    IReadOnlyList<IOutputFormatter> outputFormatters,
        //    IControllerActionArgumentBinder controllerActionArgumentBinder,
        //    IReadOnlyList<IModelBinder> modelBinders,
        //    IReadOnlyList<IModelValidatorProvider> modelValidatorProviders,
        //    IReadOnlyList<IValueProviderFactory> valueProviderFactories,
        //    IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
        //    ITempDataDictionary tempData,
        //    ILoggerFactory loggerFactory,
        //    int maxModelValidationErrors)
        //    : base(
        //          actionContext,
        //          filterProviders,
        //          controllerFactory,
        //          descriptor,
        //          inputFormatters,
        //          outputFormatters,
        //          controllerActionArgumentBinder,
        //          modelBinders,
        //          modelValidatorProviders,
        //          valueProviderFactories,
        //          actionBindingContextAccessor,
        //          tempData,
        //          loggerFactory,             
        //          maxModelValidationErrors
        //          )
        //{
        //    _descriptor = descriptor;
        //}

        //protected override async Task<IActionResult> InvokeActionAsync(ActionExecutingContext actionExecutingContext)
        //{
        //    var actionMethodInfo = _descriptor.MethodInfo;
        //    var actionReturnValue = await ControllerActionExecutor.ExecuteAsync(
        //        actionMethodInfo,
        //        actionExecutingContext.Controller,
        //        actionExecutingContext.ActionArguments);

        //    var actionResult = CreateActionResult(
        //        actionMethodInfo.ReturnType,
        //        actionReturnValue);
        //    return actionResult;
        //}

        //internal static IActionResult CreateActionResult(Type declaredReturnType, object actionReturnValue)
        //{
        //    // optimize common path
        //    var actionResult = actionReturnValue as IActionResult;
        //    if (actionResult != null)
        //    {
        //        return actionResult;
        //    }

        //    if (declaredReturnType == typeof(void) ||
        //        declaredReturnType == typeof(Task))
        //    {
        //        return new EmptyResult();
        //    }

        //    // Unwrap potential Task<T> types.
        //    var actualReturnType = GetTaskInnerTypeOrNull(declaredReturnType) ?? declaredReturnType;
        //    if (actionReturnValue == null &&
        //        typeof(IActionResult).GetTypeInfo().IsAssignableFrom(actualReturnType.GetTypeInfo()))
        //    {
        //        throw new InvalidOperationException("Kaboom");
        //    }

        //    return new ObjectResult(actionReturnValue)
        //    {
        //        DeclaredType = actualReturnType
        //    };
        //}

        //private static Type GetTaskInnerTypeOrNull(Type type)
        //{
        //    var genericType = ExtractGenericInterface(type, typeof(Task<>));

        //    return genericType?.GenericTypeArguments[0];
        //}

        //public static Type ExtractGenericInterface(Type queryType,Type interfaceType)
        //{
        //    Func<Type, bool> matchesInterface =
        //        type => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == interfaceType;
        //    if (matchesInterface(queryType))
        //    {
        //        // Checked type matches (i.e. is a closed generic type created from) the open generic type.
        //        return queryType;
        //    }

        //    // Otherwise check all interfaces the type implements for a match.
        //    return queryType.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(matchesInterface);
        //}
        public Task InvokeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
