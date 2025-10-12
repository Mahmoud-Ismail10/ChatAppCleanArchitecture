using ChatApp.Application.Features.MessageStatuses.Queries.Responses;
using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services
{
    public class MessageStatusService : IMessageStatusService
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly IChatMemberRepository _chatMemberRepository;
        private readonly IMessageStatusRepository _messageStatusRepository;
        #endregion

        #region Constructors
        public MessageStatusService(
            IUserService userService,
            IChatMemberRepository chatMemberRepository,
            IMessageStatusRepository messageStatusRepository)
        {
            _userService = userService;
            _chatMemberRepository = chatMemberRepository;
            _messageStatusRepository = messageStatusRepository;
        }
        #endregion

        #region Functions
        public async Task<List<MessageStatus>> CreateMessageStatusesAsync(Guid chatId, Guid currentUserId, Guid messageId)
        {
            return await _chatMemberRepository.GetTableNoTracking()
                .Where(cm => cm.ChatId == chatId &&
                        cm.UserId != currentUserId &&
                        cm.Status == MemberStatus.Active)
                .Select(m => new MessageStatus
                {
                    Id = Guid.NewGuid(),
                    MessageId = messageId,
                    UserId = m.UserId,
                    Status = MessageState.Sent
                }).ToListAsync();
        }

        public async Task<MessageStatusDto> MarkAsDeliveredAsync(Guid messageId, Guid userId)
        {
            var status = await _messageStatusRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(ms => ms.MessageId == messageId && ms.UserId == userId);

            if (status != null && status.Status < MessageState.Delivered)
            {
                status.Status = MessageState.Delivered;
                status.DeliveredAt = DateTimeOffset.UtcNow.ToLocalTime();
                await _messageStatusRepository.UpdateAsync(status);
            }

            var user = await _userService.GetUserByIdAsync(userId);

            return new MessageStatusDto
            (
                userId,
                MessageState.Delivered,
                status?.DeliveredAt,
                status?.ReadAt,
                status?.PlayedAt,
                user!.Name,
                user.ProfileImageUrl);
        }

        public async Task<MessageStatusDto> MarkAsReadAsync(Guid messageId, Guid userId)
        {
            var status = await _messageStatusRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(ms => ms.MessageId == messageId && ms.UserId == userId);

            if (status != null && status.Status < MessageState.Read)
            {
                status.Status = MessageState.Read;
                status.ReadAt = DateTimeOffset.UtcNow.ToLocalTime();
                await _messageStatusRepository.UpdateAsync(status);
            }

            var user = await _userService.GetUserByIdAsync(userId);

            return new MessageStatusDto
            (
                userId,
                MessageState.Read,
                status?.DeliveredAt,
                status?.ReadAt,
                status?.PlayedAt,
                user!.Name,
                user.ProfileImageUrl);
        }

        public async Task<MessageStatusDto> MarkAsPlayedAsync(Guid messageId, Guid userId)
        {
            var status = await _messageStatusRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(ms => ms.MessageId == messageId && ms.UserId == userId);

            if (status != null && status.Status < MessageState.Played)
            {
                status.Status = MessageState.Played;
                status.PlayedAt = DateTimeOffset.UtcNow.ToLocalTime();
                await _messageStatusRepository.UpdateAsync(status);
            }

            var user = await _userService.GetUserByIdAsync(userId);

            return new MessageStatusDto
            (
                userId,
                MessageState.Played,
                status?.DeliveredAt,
                status?.ReadAt,
                status?.PlayedAt,
                user!.Name,
                user.ProfileImageUrl);
        }

        public async Task<List<MessageStatusDto>> GetMessageStatusesAsync(Guid messageId)
        {
            var statuses = await _messageStatusRepository.GetTableNoTracking()
                .Where(ms => ms.MessageId == messageId)
                .ToListAsync();

            var users = await _userService.GetUsersByIdsAsync(statuses.Select(s => s.UserId).ToList());

            return statuses.Select(s =>
            {
                var user = users.FirstOrDefault(u => u.Id == s.UserId);
                return new MessageStatusDto(
                    s.UserId,
                    s.Status,
                    s.DeliveredAt,
                    s.ReadAt,
                    s.PlayedAt,
                    user?.Name,
                    user?.ProfileImageUrl
                );
            }).ToList();
        }

        public async Task<List<Guid>?> GetUndeliveredMessagesAsync(Guid userId)
        {
            return await _messageStatusRepository.GetTableNoTracking()
                .Where(ms => ms.UserId == userId && ms.Status == MessageState.Sent)
                .Select(ms => ms.MessageId)
                .ToListAsync();
        }

        #endregion
    }
}
