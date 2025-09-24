using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Data;

namespace ChatApp.Infrastructure.Repositories
{
    public class UserRepository : GenericRepositoryAsync<User>, IUserRepository
    {
        public UserRepository(ChatAppBbContext dbContext) : base(dbContext)
        {
        }
    }
}
