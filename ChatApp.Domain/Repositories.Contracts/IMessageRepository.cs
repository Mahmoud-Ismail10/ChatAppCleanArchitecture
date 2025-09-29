using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Repositories.Contracts
{
    public interface IMessageRepository : IGenericRepositoryAsync<Message>
    {
    }
}
