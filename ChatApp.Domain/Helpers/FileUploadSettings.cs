using System.Text.Json.Serialization;

namespace ChatApp.Domain.Helpers
{
    public class FileUploadSettings
    {
        public string AllowedExtensions { get; set; } = string.Empty;
        public int MaxFileSizeMB { get; set; }

        [JsonIgnore]
        public List<string> AllowedExtensionsList =>
        AllowedExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
