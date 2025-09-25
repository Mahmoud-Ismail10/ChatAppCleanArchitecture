using ChatApp.Application;
using ChatApp.Infrastructure;
using ChatApp.Presentation.Hubs;
using ChatApp.Presentation.Middlewares;
using ChatApp.Presentation.Security;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Serilog;
using System.Globalization;


Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

#region Authorization
builder.Services.AddAuthentication("SessionKey")
.AddScheme<AuthenticationSchemeOptions, SessionKeyAuthenticationHandler>("SessionKey", null);
builder.Services.AddAuthorization();
#endregion

#region Dependency Injections
builder.Services
    .AddServiceRegistration(builder.Configuration)
    .AddApplicationDependencies()
    .AddInfrastructureDependencies();
#endregion

#region Localization
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(opt =>
{
    opt.ResourcesPath = "";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    List<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-EG")
                };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
#endregion

#region AllowCORS
var CORS = "_cors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS,
                      policy =>
                      {
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.AllowAnyOrigin();
                      });
});
#endregion

#region Serilog
Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(builder.Configuration)
              .CreateLogger();
builder.Services.AddSerilog();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Localization Middleware
var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options!.Value);
#endregion

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseCors(CORS);

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationsHub>("/hubs/notifications");

app.Run();
