using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Services.Contracts
{
    public interface IUserService
    {
        Task<string> AddUserAsync(User user);
        Task<string> DeleteProfileImageAsync(User user);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<bool> IsPhoneUniqueAsync(string phoneNumber, CancellationToken cancellationToken);
        Task<string> UpdateProfileImageAsync(User user, IFormFile profileImage);
        Task<string> UpdateUserAsync(User user);
    }
}
