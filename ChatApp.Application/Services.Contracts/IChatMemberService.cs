using ChatApp.Application.Features.ChatsMember.Queries.Responses;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IChatMemberService
    {
        Task<string> DeleteGroupAsync(ChatMember chatMember);
        Task<List<Guid>> GetActiveUsersAsync(Guid ChatId);
        Task<IReadOnlyList<GetAllChatsMemberResponse>> GetAllChatsMemberAsync(Guid userId);
        Task<ChatMember?> GetAnotherUserInSameChatAsync(Guid currentUserId, Guid chatId);
        Task<ChatMember?> GetChatMemberByIdAsync(Guid chatMemberId);
        Task<List<Guid>> GetChatMembersIdsAsync(Guid chatId);
        Task<bool> IsDeletedFromAllMembersAsync(Guid chatId);
        Task<bool> IsMemberOfChatAsync(Guid userId, Guid chatId);
        Task<bool> IsOwnerOrAdminAsync(Guid userId, Guid chatId);
        Task<string> LeftGroupAsync(ChatMember chatMember);
        Task<string> MakeAsAdminOrUnadminAsync(ChatMember chatMember);
        Task<List<MessageStatus?>> MarkAllAsReadAsync(Guid chatMemberId);
        Task<string> PinOrUnpinChatAsync(Guid chatMemberId);
        Task<string> RemoveMemberFromGroupAsync(ChatMember chatMember);
        Task<string> RestoreDeletedChatMembersAsync(ChatMember? chatMember);
        Task<string> SoftDeleteChatMemberAsync(ChatMember chatMember);
    }
}
