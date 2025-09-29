using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IMessageService
    {
        Task<string> AddMessageAsync(Message message);
    }
}
