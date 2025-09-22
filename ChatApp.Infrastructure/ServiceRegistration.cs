using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            // SQL Server connection
            var sqlConnectionString = configuration.GetConnectionString("ConnectionString");
            services.AddDbContext<ChatAppBbContext>(options => options.UseSqlServer(sqlConnectionString));

            return services;
        }
    }
}
