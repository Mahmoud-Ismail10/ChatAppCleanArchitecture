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
        Task<List<Guid>> GetChatMembersIdsAsync(Guid chatId);
        Task<bool> IsDeletedFromAllMembersAsync(Guid chatId);
        Task<bool> IsMemberOfChatAsync(Guid userId, Guid chatId);
        Task<bool> IsOwnerOrAdminAsync(Guid userId, Guid chatId);
        Task<string> MakeAsAdminOrUnadminAsync(ChatMember chatMember);
        Task<List<MessageStatus?>> MarkAsReadAsync(Guid chatMemberId);
        Task<string> PinOrUnpinChatAsync(Guid chatMemberId);
        Task<string> RestoreDeletedChatMembersAsync(ChatMember? chatMember);
        Task<string> SoftDeleteChatMemberAsync(ChatMember chatMember);
        Task<string> UpdateChatMemberAsync(ChatMember chatMember);
    }
}
