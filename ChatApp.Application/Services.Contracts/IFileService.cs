using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Services.Contracts
{
    public interface IFileService
    {
        Task<(string? FileUrl, MessageType MessageType, string? ErrorMessage)> GetFileAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<string> UploadImageAsync(string location, IFormFile file);
    }
}
