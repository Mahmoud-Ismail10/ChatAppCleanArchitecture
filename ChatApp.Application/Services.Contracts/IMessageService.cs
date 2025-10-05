using ChatApp.Domain.Entities;
using System.Linq.Expressions;

namespace ChatApp.Application.Services.Contracts
{
    public interface IMessageService
    {
        Task<string> AddMessageAsync(Message message);
        Task<int> CountAsync(Expression<Func<Message, bool>> predicate);
        Task<string> DeleteMessageAsync(Message message);
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task<string> UpdateMessageAsync(Message message);
    }
}
