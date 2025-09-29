using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IChatService
    {
        Task<string> CreateChatAsync(Chat chat);
        Task<IReadOnlyList<Chat?>> GetAllChatsOfUserAsync(Guid userId);
        Task<Chat?> GetChatBetweenUsersAsync(Guid senderId, Guid recevierId);
        Task<Chat?> GetChatWithMessagesAsync(Guid chatId);
        Task<string> UpdateChatAsync(Chat chat);
    }
}
