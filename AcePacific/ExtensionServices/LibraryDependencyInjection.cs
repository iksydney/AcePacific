using AcePacific.Data.RepositoryPattern;
using AcePacific.Data.RepositoryPattern.Implementations;
using NetCore.AutoRegisterDi;
using System.Reflection;

namespace AcePacific.API.ExtensionServices
{
    public static class LibraryDependencyInjection
    {
        public static IServiceCollection RegisterLibraryServices<TapplicationContext, TRepository>(this IServiceCollection services)
            where TapplicationContext : EntityFrameworkDataContext<TapplicationContext> where TRepository : class
        {
            services.AddTransient<IDataContextAsync, TapplicationContext>();
            services.AddTransient<IUnitOfWork, EntityFrameworkUnitOfWork>();
            services.AddTransient(typeof(IRepositoryAsync<>), typeof(Repository<>));
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            var assemblyRepositoryToScan = Assembly.GetAssembly(typeof(IRepository<>));
            services.RegisterAssemblyPublicNonGenericClasses(assemblyRepositoryToScan)
                .Where(x => x.Name.EndsWith("Repository"))
                .AsPublicImplementedInterfaces();
            return services;
        }
        public static IServiceCollection RegisterApplicationService<T>(this IServiceCollection services)
        {
            var assemblyToScan = Assembly.GetAssembly(typeof(T));
            services.RegisterAssemblyPublicNonGenericClasses(assemblyToScan)
                .Where(x => x.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces();
            return services;
        }
    }
}
