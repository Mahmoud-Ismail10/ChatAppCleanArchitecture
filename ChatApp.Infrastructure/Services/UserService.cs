using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ChatApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly IFileService _fileService;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructors
        public UserService(IFileService fileService,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _fileService = fileService;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Functions
        public async Task<string> AddUserAsync(User user, IFormFile file)
        {
            if (file != null)
            {
                var context = _httpContextAccessor.HttpContext!.Request;
                var baseUrl = context.Scheme + "://" + context.Host;
                var imageUrl = await _fileService.UploadImageAsync("Users", file);
                if (imageUrl == "FailedToUploadImage") return "FailedToUploadImage";
                user.ProfileImageUrl = baseUrl + imageUrl;
            }
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

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetTableNoTracking()
                                        .Where(u => u.Id == userId)
                                        .FirstOrDefaultAsync();
        }

        public async Task<string> UpdateUserAsync(User user, IFormFile file)
        {
            if (file != null)
            {
                var context = _httpContextAccessor.HttpContext!.Request;
                var baseUrl = context.Scheme + "://" + context.Host;
                var imageUrl = await _fileService.UploadImageAsync("Users", file);
                if (imageUrl == "FailedToUploadImage") return "FailedToUploadImage";
                user.ProfileImageUrl = baseUrl + imageUrl;
            }
            try
            {
                user.ProfileImageUrl = null;
                await _userRepository.UpdateAsync(user);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error updating user : {ErrorMessage}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }
        #endregion
    }
}
