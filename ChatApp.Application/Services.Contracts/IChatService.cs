using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Services.Contracts
{
    public interface IChatService
    {
        Task<string> CreateChatAsync(Chat chat);
        Task<string> DeleteChatAsync(Chat chat);
        Task<IReadOnlyList<Chat?>> GetAllChatsOfUserAsync(Guid userId);
        Task<Chat?> GetChatBetweenUsersAsync(Guid senderId, Guid recevierId);
        Task<Chat?> GetChatByIdAsync(Guid chatId);
        Task<Chat?> GetChatWithMessagesAsync(Guid chatId);
        Task<string> UpdateChatAsync(Chat chat);
        Task<string> UpdateGroupImageAsync(Chat chat, IFormFile groupImageUrl);
    }
}
