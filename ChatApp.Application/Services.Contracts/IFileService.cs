using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Services.Contracts
{
    public interface IFileService
    {
        void DeleteFile(string? fileUrl);
        Task<(string? FileUrl, MessageType MessageType, string? ErrorMessage)> GetFileAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<string> UploadImageAsync(string location, IFormFile file);
    }
}
