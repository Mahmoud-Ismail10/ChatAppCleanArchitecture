using ChatApp.Domain.Helpers;
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
            services.AddDbContext<ChatAppDbContext>(options => options.UseSqlServer(sqlConnectionString));

            // Twilio Settings
            var twilioSettings = new TwilioSettings();
            configuration.GetSection("Twilio").Bind(twilioSettings);
            services.AddSingleton(twilioSettings);

            // File Upload Settings
            var fileUploadSettings = new FileUploadSettings();
            configuration.GetSection("FileUpload").Bind(fileUploadSettings);
            services.AddSingleton(fileUploadSettings);

            //Swagger Gn
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Chat Application",
                    Version = "v1",
                    Description = "Clean Architecture Project"
                });

                options.EnableAnnotations();

                options.AddSecurityDefinition("SessionKey", new OpenApiSecurityScheme
                {
                    Description = "Custom session key auth. Example: 'Key 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "SessionKey"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "SessionKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

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
