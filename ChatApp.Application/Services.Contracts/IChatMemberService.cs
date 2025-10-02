using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IChatMemberService
    {
        Task<string> AddChatMemberAsync(ChatMember chatMember);
        Task<IReadOnlyList<ChatMember?>> GetAllChatsMemberAsync(Guid userId);
        Task<ChatMember?> GetAnotherUserInSameChatAsync(Guid currentUserId, Guid chatId);
        Task<ChatMember?> GetChatMemberByIdAsync(Guid chatMemberId);
        Task<bool> IsDeletedFromAllMembersAsync(Guid chatId);
        Task MarkAsReadAsync(Guid chatMemberId);
        Task<string> SoftDeleteChatMemberAsync(ChatMember chatMember);
        Task<string> UpdateChatMemberAsync(ChatMember chatMember);
    }
}
