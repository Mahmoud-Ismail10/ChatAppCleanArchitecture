using ChatApp.Application.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace ChatApp.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        #region Fields
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _otpExpiry = TimeSpan.FromMinutes(5);
        #endregion

        #region Constructors
        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }
        #endregion

        #region Functions
        public Task<string> GenerateOtpAsync(string phoneNumber)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set(phoneNumber, otp, _otpExpiry);
            return Task.FromResult(otp);
        }

        public Task<bool> VerifyOtpAsync(string phoneNumber, string code)
        {
            if (_cache.TryGetValue(phoneNumber, out string? storedCode) && storedCode == code)
            {
                _cache.Remove(phoneNumber);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        #endregion
    }
}
