using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services
{
    public class ChatMemberService : IChatMemberService
    {
        #region Fields
        private readonly IChatMemberRepository _chatMemberRepository;
        #endregion

        #region Constructors
        public ChatMemberService(IChatMemberRepository chatMemberRepository)
        {
            _chatMemberRepository = chatMemberRepository;
        }
        #endregion

        #region Functions
        public async Task<IReadOnlyList<ChatMember?>> GetAllChatsMemberAsync(Guid userId)
        {
            return await _chatMemberRepository.GetTableNoTracking()
                                              .Include(cm => cm.Chat)
                                                .ThenInclude(c => c!.LastMessage)
                                              .Where(u => u.UserId == userId)
                                              .ToListAsync();
        }
        #endregion
    }
}
