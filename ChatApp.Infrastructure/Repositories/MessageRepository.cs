using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Data;

namespace ChatApp.Infrastructure.Repositories
{
    internal class MessageRepository : GenericRepositoryAsync<Message>, IMessageRepository
    {
        public MessageRepository(ChatAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
