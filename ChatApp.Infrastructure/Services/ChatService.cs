using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        #region Fields
        private readonly IChatRepository _chatRepository;
        #endregion

        #region Constructors
        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }
        #endregion

        #region Functions
        public async Task<Chat?> GetChatWithMessagesAsync(Guid chatId)
        {
            return await _chatRepository.GetTableNoTracking()
                                        .Include(c => c.Messages.OrderBy(m => m.SentAt))
                                        .Where(c => c.Id == chatId)
                                        .FirstOrDefaultAsync();
        }
        #endregion
    }
}
