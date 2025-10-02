using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;

namespace ChatApp.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        #region Fields
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        #endregion

        #region Constructors
        public MessageService(IMessageRepository messageRepository, IChatRepository chatRepository)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
        }
        #endregion

        #region Functions
        public async Task<string> AddMessageAsync(Message message)
        {
            try
            {
                await _messageRepository.AddAsync(message);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in adding message : {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<int> CountAsync(Expression<Func<Message, bool>> predicate)
        {
            return await _messageRepository.GetTableNoTracking().CountAsync(predicate);
        }

        public async Task<string> DeleteMessageAsync(Message message)
        {
            try
            {
                var chat = await _chatRepository.GetByIdAsync(message.ChatId);
                if (chat == null) return "ChatNotFound";

                if (chat.LastMessageId == message.Id)
                {
                    var newLastMessage = await _messageRepository
                        .GetTableNoTracking()
                        .Where(m => m.ChatId == message.ChatId && m.Id != message.Id)
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefaultAsync();

                    chat.LastMessageId = newLastMessage?.Id;
                    await _chatRepository.UpdateAsync(chat);
                }
                await _messageRepository.DeleteAsync(message);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in deleting message : {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<Message?> GetMessageByIdAsync(Guid messageId)
        {
            return await _messageRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }
        #endregion
    }
}
