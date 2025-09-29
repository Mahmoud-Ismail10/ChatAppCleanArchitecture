using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
            return await _chatMemberRepository.GetTableAsTracking()
                                              .Include(cm => cm.Chat)
                                                .ThenInclude(c => c!.LastMessage)
                                              .Where(u => u.UserId == userId)
                                              .ToListAsync();
        }

        public async Task<string> AddChatMemberAsync(ChatMember chatMember)
        {
            try
            {
                await _chatMemberRepository.AddAsync(chatMember);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in creating new chat member: {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<ChatMember?> GetAnotherUserInSameChatAsync(Guid currentUserId, Guid chatId)
        {
            return await _chatMemberRepository.GetTableNoTracking()
                                .Include(cm => cm.User)
                                .Where(cm => cm.ChatId == chatId && cm.UserId != currentUserId)
                                .FirstOrDefaultAsync();
        }
        #endregion
    }
}
