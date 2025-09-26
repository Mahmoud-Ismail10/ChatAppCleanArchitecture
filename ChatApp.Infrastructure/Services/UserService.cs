using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ChatApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly IUserRepository _userRepository;
        #endregion

        #region Constructors
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region Functions
        public async Task<string> AddUserAsync(User user)
        {
            try
            {
                await _userRepository.AddAsync(user);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error adding user : {ErrorMessage}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _userRepository.GetTableNoTracking()
                                        .Where(u => u.PhoneNumber == phoneNumber)
                                        .FirstOrDefaultAsync();
        }
        #endregion
    }
}
