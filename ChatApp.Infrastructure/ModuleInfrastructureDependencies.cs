using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddTransient<IOtpService, OtpService>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<ISmsService, SmsService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IChatMemberService, ChatMemberService>();
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IOnlineUserService, OnlineUserService>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IChatMemberRepository, ChatMemberRepository>();
            services.AddTransient<ISessionRepository, SessionRepository>();
            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            return services;
        }
    }
}
