using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Services.Contracts
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(string location, IFormFile file);
    }
}
