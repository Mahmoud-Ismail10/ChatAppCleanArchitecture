using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Serilog;

namespace ChatApp.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        #region Fields
        private readonly IMessageRepository _messageRepository;
        #endregion

        #region Constructors
        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        #endregion

        #region Handle Functions
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
        #endregion
    }
}
