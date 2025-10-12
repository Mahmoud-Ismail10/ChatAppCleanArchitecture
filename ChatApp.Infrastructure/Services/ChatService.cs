using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ChatApp.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        #region Fields
        private readonly IFileService _fileService;
        private readonly IChatRepository _chatRepository;
        private readonly ITransactionService _transactionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructors
        public ChatService(
            IFileService fileService,
            IChatRepository chatRepository,
            ITransactionService transactionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _fileService = fileService;
            _chatRepository = chatRepository;
            _transactionService = transactionService;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Functions
        public async Task<Chat?> GetChatWithMessagesAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetTableNoTracking()
                                            .Include(c => c.Messages)
                                                .ThenInclude(m => m.MessageStatuses)
                                            .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat != null)
                chat.LastMessage = chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

            return chat;
        }

        public async Task<Chat?> GetChatByIdAsync(Guid chatId)
        {
            return await _chatRepository.GetTableNoTracking()
                                            .Include(c => c.ChatMembers)
                                            .FirstOrDefaultAsync(c => c.Id == chatId);
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

        public async Task<string> DeleteChatAsync(Chat chat)
        {
            try
            {
                await _chatRepository.DeleteAsync(chat);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in deleting chat : {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<Chat?> GetChatBetweenUsersAsync(Guid senderId, Guid recevierId)
        {
            return await _chatRepository.GetTableNoTracking()
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

        public async Task<string> UpdateGroupImageAsync(Chat chat, IFormFile groupImageUrl)
        {
            using var transaction = await _transactionService.BeginTransactionAsync();
            try
            {
                var oldImageUrl = chat.GroupImageUrl;
                if (groupImageUrl != null)
                {
                    var context = _httpContextAccessor.HttpContext!.Request;
                    var baseUrl = context.Scheme + "://" + context.Host;
                    var imagePath = await _fileService.UploadImageAsync("Groups", groupImageUrl);
                    if (imagePath == "FailedToUploadImage") return "FailedToUploadImage";
                    chat.GroupImageUrl = imagePath;
                }
                else
                    return "NoImageProvided";

                _fileService.DeleteFile(oldImageUrl);
                await _chatRepository.UpdateAsync(chat);
                await transaction.CommitAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error("Error updating image of group {GroupName}: {ErrorMessage}", chat.Name, ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }
        #endregion
    }
}
