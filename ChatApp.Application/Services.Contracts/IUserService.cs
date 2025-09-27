using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Services.Contracts
{
    public interface IUserService
    {
        Task<string> AddUserAsync(User user, IFormFile file);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
    }
}
