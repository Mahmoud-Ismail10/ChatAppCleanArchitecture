
namespace ChatApp.Application.Services.Contracts
{
    public interface ISmsService
    {
        Task<string> SendSmsAsync(string phoneNumber, string body);
    }
}
