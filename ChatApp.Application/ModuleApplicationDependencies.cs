using ChatApp.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ChatApp.Application
{
    public static class ModuleApplicationDependencies
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            /// Configuaration for MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            /// Configuaration for FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
