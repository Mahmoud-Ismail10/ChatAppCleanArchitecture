using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

        public async Task<string> CreateChatAsync(Chat chat)
        {
            try
            {
                await _chatRepository.AddAsync(chat);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in creating new chat : {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<Chat?> GetChatBetweenUsersAsync(Guid senderId, Guid recevierId)
        {
            return await _chatRepository.GetTableNoTracking()
                                        .Include(c => c.Messages)
                                        .Include(c => c.ChatMembers)
                                        .FirstOrDefaultAsync(c => c.ChatMembers.Any(m => m.UserId == senderId) &&
                                                                  c.ChatMembers.Any(m => m.UserId == recevierId) &&
                                                                  c.ChatMembers.Count == 2);
        }

        public async Task<IReadOnlyList<Chat?>> GetAllChatsOfUserAsync(Guid userId)
        {
            return await _chatRepository.GetTableNoTracking()
                                        .Where(c => c.ChatMembers.Any(cm => cm.UserId == userId))
                                        .ToListAsync();
        }

        public async Task<string> UpdateChatAsync(Chat chat)
        {
            try
            {
                await _chatRepository.UpdateAsync(chat);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in updating chat : {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }
        #endregion
    }
}
