using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Contracts
{
    public interface IUserService
    {
        Task<string> AddUserAsync(User user);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
    }
}
