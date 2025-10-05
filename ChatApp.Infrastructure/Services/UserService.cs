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
        private readonly ITransactionService _transactionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructors
        public UserService(IFileService fileService,
            IUserRepository userRepository,
            ITransactionService transactionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _fileService = fileService;
            _userRepository = userRepository;
            _transactionService = transactionService;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetTableNoTracking()
                                        .Where(u => u.Id == userId)
                                        .FirstOrDefaultAsync();
        }

        public async Task<string> UpdateUserAsync(User user)
        {
            try
            {
                await _userRepository.UpdateAsync(user);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error updating user : {ErrorMessage}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<string> UpdateProfileImageAsync(User user, IFormFile profileImage)
        {
            using var transaction = await _transactionService.BeginTransactionAsync();
            try
            {
                var oldImageUrl = user.ProfileImageUrl;
                if (profileImage != null)
                {
                    var context = _httpContextAccessor.HttpContext!.Request;
                    var baseUrl = context.Scheme + "://" + context.Host;
                    var imagePath = await _fileService.UploadImageAsync("Users", profileImage);
                    if (imagePath == "FailedToUploadImage") return "FailedToUploadImage";
                    user.ProfileImageUrl = imagePath;
                }
                else
                    return "NoImageProvided";

                _fileService.DeleteFile(oldImageUrl);
                await _userRepository.UpdateAsync(user);
                await transaction.CommitAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error("Error updating profile image of user {UserName}: {ErrorMessage}", user.Name, ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<string> DeleteProfileImageAsync(User user)
        {
            try
            {
                _fileService.DeleteFile(user.ProfileImageUrl);
                user.ProfileImageUrl = null;

                await _userRepository.UpdateAsync(user);
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Error deleting profile image of user {UserName}: {ErrorMessage}", user.Name, ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }

        public async Task<bool> IsPhoneUniqueAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return !await _userRepository.GetTableNoTracking()
                .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
        }
        #endregion
    }
}
