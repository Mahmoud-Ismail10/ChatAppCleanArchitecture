using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Data;

namespace ChatApp.Infrastructure.Repositories
{
    public class ContactRepository : GenericRepositoryAsync<Contact>, IContactRepository
    {
        public ContactRepository(ChatAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
