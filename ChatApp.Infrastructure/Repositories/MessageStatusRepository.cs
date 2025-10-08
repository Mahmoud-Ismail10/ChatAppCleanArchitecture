using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Data;

namespace ChatApp.Infrastructure.Repositories
{
    public class MessageStatusRepository : GenericRepositoryAsync<MessageStatus>, IMessageStatusRepository
    {
        public MessageStatusRepository(ChatAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
