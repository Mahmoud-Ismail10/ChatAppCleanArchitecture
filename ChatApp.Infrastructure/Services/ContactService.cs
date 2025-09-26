using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ChatApp.Infrastructure.Services
{
    public class ContactService : IContactService
    {
        #region Fields
        private readonly IContactRepository _contactRepository;
        #endregion

        #region Constructors
        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }
        #endregion

        #region Handle Functions
        public async Task<string> AddToContactAsync(Contact contact)
        {
            try
            {
                await _contactRepository.AddAsync(contact);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error in adding contact : {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<Contact?> GetContactAsync(Guid userId, Guid contactId)
        {
            return await _contactRepository.GetTableNoTracking()
                                           .Where(c => c.UserId == userId && c.ContactUserId == contactId)
                                           .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Contact?>> GetAllContactsAsync(Guid userId)
        {
            return await _contactRepository.GetTableNoTracking()
                                           .Include(c => c.ContactUser)
                                           .Where(c => c.UserId == userId)
                                           .ToListAsync();
        }
        #endregion
    }
}
