using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IChatMemberService
    {
        Task<IReadOnlyList<ChatMember?>> GetAllChatsMemberAsync(Guid userId);
    }
}
