using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Data;

namespace ChatApp.Infrastructure.Repositories
{
    public class ChatRepository : GenericRepositoryAsync<Chat>, IChatRepository
    {
        public ChatRepository(ChatAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
