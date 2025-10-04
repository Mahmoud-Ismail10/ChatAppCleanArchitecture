using ChatApp.Application.Features.ChatsMember.Queries.Responses;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IChatMemberService
    {
        Task<string> AddChatMemberAsync(ChatMember chatMember);
        Task<IReadOnlyList<GetAllChatsMemberResponse>> GetAllChatsMemberAsync(Guid userId);
        Task<ChatMember?> GetAnotherUserInSameChatAsync(Guid currentUserId, Guid chatId);
        Task<ChatMember?> GetChatMemberByIdAsync(Guid chatMemberId);
        Task<bool> IsDeletedFromAllMembersAsync(Guid chatId);
        Task<bool> IsMemberOfChatAsync(Guid userId, Guid chatId);
        Task MarkAsReadAsync(Guid chatMemberId, Guid userId);
        Task<string> PinOrUnpinChatAsync(Guid chatMemberId);
        Task<string> RestoreDeletedChatMembersAsync(ChatMember? chatMember);
        Task<string> SoftDeleteChatMemberAsync(ChatMember chatMember);
        Task<string> UpdateChatMemberAsync(ChatMember chatMember);
    }
}
