using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IContactService
    {
        Task<string> AddToContactAsync(Contact contact);
        Task<IReadOnlyList<Contact?>> GetAllContactsAsync(Guid userId);
        Task<Contact?> GetContactAsync(Guid userId, Guid contactId);
    }
}
