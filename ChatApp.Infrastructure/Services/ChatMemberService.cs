using ChatApp.Application.Features.ChatsMember.Queries.Responses;
using ChatApp.Application.Resources;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
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
        private readonly IMessageStatusRepository _messageStatusRepository;
        private readonly IMessageNotifier _messageNotifier;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        #endregion

        #region Constructors
        public ChatMemberService(IChatMemberRepository chatMemberRepository,
            IMessageStatusRepository messageStatusRepository,
            IMessageNotifier messageNotifier,
            IStringLocalizer<SharedResources> stringLocalizer)
        {
            _chatMemberRepository = chatMemberRepository;
            _messageStatusRepository = messageStatusRepository;
            _messageNotifier = messageNotifier;
            _stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Functions
        public async Task<IReadOnlyList<GetAllChatsMemberResponse>> GetAllChatsMemberAsync(Guid userId)
        {
            return await _chatMemberRepository.GetTableNoTracking()
             .Where(cm => cm.UserId == userId && !cm.IsDeleted)
             .Select(cm => new GetAllChatsMemberResponse
             (
                 cm.Chat!.IsGroup
                     ? null
                     : cm.Chat.ChatMembers
                         .Where(m => m.UserId != userId)
                         .Select(m => m.Id)
                         .FirstOrDefault(), // ChatMemberId of the other member in case of one-to-one chat

                 cm.Chat.IsGroup
                     ? null
                     : cm.Chat.ChatMembers
                         .Where(m => m.UserId != userId)
                         .Select(m => m.UserId)
                         .FirstOrDefault(), // UserId of the other member in case of one-to-one chat

                 cm.Id, // ChatMemberId
                 cm.Chat!.Id, // ChatId
                 cm.Chat.IsGroup, // IsGroup
                 cm.IsPinned, // IsPinned
                 false, // IsOnline (This will be set in the handler) 

                 cm.Chat.IsGroup
                     ? cm.Chat.Name
                     : cm.Chat.ChatMembers
                         .Where(m => m.UserId != userId)
                         .Select(m => m.User!.Name)
                         .FirstOrDefault(), // ChatName

                 cm.Chat.IsGroup
                     ? cm.Chat.GroupImageUrl
                     : cm.Chat.ChatMembers
                         .Where(m => m.UserId != userId)
                         .Select(m => m.User!.ProfileImageUrl)
                         .FirstOrDefault(), // ChatImageUrl

                 cm.Chat.LastMessage!.Type, // LastMessageType
                 cm.Chat.LastMessage.Content, // LastMessageContent
                 cm.Chat.LastMessage.SentAt, // LastMessageSentAt
                 cm.Chat.Messages.Count(m =>
                     cm.LastReadMessageAt == null || m.SentAt > cm.LastReadMessageAt.Value) // UnreadMessagesCount
             )).ToListAsync();
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

        public async Task<string> RestoreDeletedChatMembersAsync(ChatMember? chatMember)
        {
            try
            {
                if (chatMember != null)
                {
                    chatMember.IsDeleted = false;
                    await _chatMemberRepository.UpdateAsync(chatMember);
                    return "Success";
                }
                return "Failed";
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

        public async Task<List<MessageStatus?>> MarkAsReadAsync(Guid chatMemberId)
        {
            try
            {
                var chatMember = await _chatMemberRepository.GetTableAsTracking()
                    .Include(cm => cm.Chat!)
                        .ThenInclude(c => c.Messages)
                    .FirstOrDefaultAsync(cm => cm.Id == chatMemberId);

                if (chatMember == null)
                    throw new Exception(_stringLocalizer[SharedResourcesKeys.ChatNotFound]);

                var messageIds = chatMember.Chat!.Messages.Select(m => m.Id).ToList();

                var unreadStatuses = await _messageStatusRepository.GetTableAsTracking()
                    .Include(ms => ms.Message)
                    .Where(ms => messageIds.Contains(ms.MessageId) &&
                                 ms.UserId == chatMember.UserId &&
                                 ms.Status == MessageState.Delivered)
                    .ToListAsync();

                foreach (var status in unreadStatuses)
                {
                    status.Status = MessageState.Read;
                    status.ReadAt = DateTimeOffset.UtcNow.ToLocalTime();
                }

                if (unreadStatuses.Any())
                    await _messageStatusRepository.UpdateRangeAsync(unreadStatuses);

                await _chatMemberRepository.UpdateAsync(chatMember);

                return unreadStatuses!;
            }
            catch (Exception ex)
            {
                Log.Error("Error in mark messages as read : {Message}", ex.InnerException?.Message ?? ex.Message);
                return null!;
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

        public async Task<bool> IsMemberOfChatAsync(Guid userId, Guid chatId)
        {
            return await _chatMemberRepository.GetTableNoTracking()
                                              .AnyAsync(cm => cm.UserId == userId && cm.ChatId == chatId && !cm.IsDeleted);
        }
        #endregion
    }
}
