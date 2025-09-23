using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            return services;
        }
    }
}
