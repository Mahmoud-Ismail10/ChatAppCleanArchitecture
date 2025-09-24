namespace ChatApp.Application.Services.Contracts
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string phoneNumber);
        Task<bool> VerifyOtpAsync(string phoneNumber, string code);
    }
}
