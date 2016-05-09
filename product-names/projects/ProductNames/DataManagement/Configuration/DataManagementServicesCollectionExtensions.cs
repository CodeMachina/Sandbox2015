using ProductNames.DataManagement.Implementation.ADO;
using ProductNames.DataManagement.Contract;
using ProductNames.DataManagement.Implementation.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Framework.Configuration;

namespace ProductNames.DataManagement.Configuration {
    public static class DataManagementServiceCollectionExtensions {
        public static IServiceCollection AddDataManagementForEntityFrameworkSqlite(this IServiceCollection services, IConfiguration configuration) {
            services.AddEntityFramework()
                .AddSqlite()
                .AddDbContext<ProductNameContext>(options => options.UseSqlite(configuration["Data:SqliteConnection:ConnectionString"]));
            services.AddTransient<IHandleProductNameData, EFProductNameDataAccessor>();

            return services;
        }

        public static IServiceCollection AddDataManagementForADOSqlite(this IServiceCollection services, IConfiguration configuration) {
            services.AddSingleton<IHandleProductNameData, ADOSqliteProductNameDataAccessor>()
                .Configure<ADOProductNameDataAccessorOptions>(options => options.ConnectionString = configuration["Data:SqliteConnection:ConnectionString"]);

            return services;
        }
    } 
}
