using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Serilog;

namespace ChatApp.Infrastructure.Services
{
    public class ChatMemberService : IChatMemberService
    {
        #region Fields
        private readonly IChatMemberRepository _chatMemberRepository;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ChatMemberService(IChatMemberRepository chatMemberRepository,
            IStringLocalizer<SharedResources> stringLocalizer)
        {
            _chatMemberRepository = chatMemberRepository;
            _stringLocalizer = stringLocalizer;
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

        public async Task<string> UpdateChatMemberAsync(ChatMember chatMember)
        {
            try
            {
                await _chatMemberRepository.UpdateAsync(chatMember);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in updating chat member: {Message}", ex.InnerException?.Message ?? ex.Message);
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

        public async Task MarkAsReadAsync(Guid chatMemberId)
        {
            try
            {
                var chatMember = await _chatMemberRepository.GetTableAsTracking()
                        .FirstOrDefaultAsync(cm => cm.Id == chatMemberId);

                if (chatMember == null) throw new Exception(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);

                chatMember.LastReadMessageAt = DateTimeOffset.UtcNow.ToLocalTime();
                await _chatMemberRepository.UpdateAsync(chatMember);
            }
            catch (Exception ex)
            {
                Log.Error("Error in mark messages as read : {Message}", ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<ChatMember?> GetChatMemberByIdAsync(Guid chatMemberId)
        {
            return await _chatMemberRepository.GetTableNoTracking()
                                              .Include(cm => cm.Chat)
                                              .FirstOrDefaultAsync(cm => cm.Id == chatMemberId);
        }

        public async Task<string> SoftDeleteChatMemberAsync(ChatMember chatMember)
        {
            try
            {
                chatMember.IsDeleted = true;
                chatMember.DeletedAt = DateTimeOffset.UtcNow.ToLocalTime();
                await _chatMemberRepository.UpdateAsync(chatMember);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in soft deleting chat member: {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<bool> IsDeletedFromAllMembersAsync(Guid chatId)
        {
            var chatMembers = await _chatMemberRepository.GetTableNoTracking()
                                                        .Where(cm => cm.ChatId == chatId)
                                                        .ToListAsync();
            if (chatMembers.All(cm => cm.IsDeleted))
                return true;
            return false;
        }

        public async Task<string> PinOrUnpinChatAsync(Guid chatMemberId)
        {
            try
            {
                var chatMember = await _chatMemberRepository.GetTableAsTracking()
                        .FirstOrDefaultAsync(cm => cm.Id == chatMemberId);
                if (chatMember == null) return "ChatMemberNotFound";
                chatMember.IsPinned = !chatMember.IsPinned;
                await _chatMemberRepository.UpdateAsync(chatMember);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in pinning/unpinning chat member: {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }
        #endregion
    }
}
