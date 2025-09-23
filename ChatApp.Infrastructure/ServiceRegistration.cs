using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace ChatApp.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            // SQL Server connection
            var sqlConnectionString = configuration.GetConnectionString("ConnectionString");
            services.AddDbContext<ChatAppBbContext>(options => options.UseSqlServer(sqlConnectionString));

            //Swagger Gn
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Commerce Project",
                    Version = "v1",
                    Description = "Clean Architecture Project"
                });

                options.EnableAnnotations();

                //options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.Http,
                //    Scheme = JwtBearerDefaults.AuthenticationScheme,
                //    BearerFormat = "JWT"
                //});

                //options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = JwtBearerDefaults.AuthenticationScheme
                //            }
                //        },
                //        Array.Empty<string>()
                //    }
                //});

                options.MapType<TimeOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "time",
                    Example = new OpenApiString("14:30:00")
                });

                options.MapType<TimeOnly?>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "time",
                    Nullable = true,
                    Example = new OpenApiString("14:30:00")
                });
            });

            return services;
        }
    }
}
