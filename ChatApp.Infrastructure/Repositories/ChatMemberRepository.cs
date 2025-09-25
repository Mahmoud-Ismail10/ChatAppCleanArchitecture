using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Data;

namespace ChatApp.Infrastructure.Repositories
{
    public class ChatMemberRepository : GenericRepositoryAsync<ChatMember>, IChatMemberRepository
    {
        public ChatMemberRepository(ChatAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
