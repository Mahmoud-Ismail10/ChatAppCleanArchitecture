using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IChatService
    {
        Task<Chat?> GetChatWithMessagesAsync(Guid chatId);
    }
}
