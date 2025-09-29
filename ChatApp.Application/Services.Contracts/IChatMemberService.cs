using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IChatMemberService
    {
        Task<string> AddChatMemberAsync(ChatMember chatMember);
        Task<IReadOnlyList<ChatMember?>> GetAllChatsMemberAsync(Guid userId);
        Task<ChatMember?> GetAnotherUserInSameChatAsync(Guid currentUserId, Guid chatId);
        Task<bool> IsMemberOfChatAsync(Guid userId, Guid chatId);
    }
}
