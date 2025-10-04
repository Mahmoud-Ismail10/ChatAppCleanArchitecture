using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ChatApp.Infrastructure.Services
{
    public class FileService : IFileService
    {
        #region Fields
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileUploadSettings _settings;

        #endregion

        #region Constructors
        public FileService(IWebHostEnvironment webHostEnvironment, FileUploadSettings settings)
        {
            _webHostEnvironment = webHostEnvironment;
            _settings = settings;
        }
        #endregion

        #region Functions
        public async Task<string> UploadImageAsync(string location, IFormFile file)
        {
            var path = _webHostEnvironment.WebRootPath + "/" + location + "/";
            var extention = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + extention;
            if (file.Length > 0)
            {
                try
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    using (FileStream filestreem = File.Create(path + fileName))
                    {
                        await file.CopyToAsync(filestreem);
                        await filestreem.FlushAsync();
                        return $"/{location}/{fileName}";
                    }
                }
                catch (Exception)
                {
                    return "FailedToUploadImage";
                }
            }
            else
                return "NoImage";
        }

        public async Task<(string? FileUrl, MessageType MessageType, string? ErrorMessage)> GetFileAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (null, MessageType.Text, "FileIsEmpty");

                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!_settings.AllowedExtensionsList.Contains(extension, StringComparer.OrdinalIgnoreCase))
                    return (null, MessageType.Text, "InvalidFileType");

                if (file.Length > _settings.MaxFileSizeMB * 1024 * 1024)
                    return (null, MessageType.Text, "FileSizeExceedsLimit");

                var fileName = Guid.NewGuid() + extension;
                var physicalPath = Path.Combine("wwwroot/Files", fileName);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                var fileUrl = "/Files/" + fileName;

                var messageType = file.ContentType.ToLower() switch
                {
                    var ct when ct.StartsWith("image/") => MessageType.Image,
                    var ct when ct.StartsWith("audio/") => MessageType.Audio,
                    var ct when ct.StartsWith("video/") => MessageType.Video,
                    "application/pdf" => MessageType.PDF,
                    _ => MessageType.Document
                };

                return (fileUrl, messageType, null);
            }
            catch (Exception ex)
            {
                Log.Error("Error while uploading file: {Message}", ex.InnerException?.Message ?? ex.Message);
                return (null, MessageType.Text, "FileUploadFailed");
            }
        }

        #endregion
    }
}
