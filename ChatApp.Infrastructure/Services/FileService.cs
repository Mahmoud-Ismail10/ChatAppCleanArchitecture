﻿using ChatApp.Application.Services.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Infrastructure.Services
{
    public class FileService : IFileService
    {
        #region Fields
        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region Constructors
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
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
        #endregion
    }
}
