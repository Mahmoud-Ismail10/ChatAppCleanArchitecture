using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Services.Contracts
{
    public interface IUserService
    {
        Task<string> AddUserAsync(User user, IFormFile file);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<string> UpdateUserAsync(User user, IFormFile file);
    }
}
