using System.Text.Json;
using System.Text.Json.Serialization;
using Character.Core.Models;
using Character.Core.Interfaces;

namespace Character.Core.Services
{
    public class CharacterService : ICharacterInterface
    {
        private readonly JsonSerializerOptions _options;

        public CharacterService()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            _options.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<(bool success, string message)> SaveAsync(List<CharacterItem> items, string fullPath)
        {
            try
            {
                var directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

                var data = new CharacterDataFile
                {
                    Version = 1,
                    Items = items
                };

                using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
                await JsonSerializer.SerializeAsync(stream, data, _options);
                return (true, "Saved successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(List<CharacterItem>? data, string message)> LoadAsync(string fullPath)
        {
            try
            {
                if (!File.Exists(fullPath)) return (null, "File does not exist");

                using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);

                var fileData = await JsonSerializer.DeserializeAsync<CharacterDataFile>(stream, _options);

                if (fileData == null) return (null, "File is empty or invalid");

                foreach (var item in fileData.Items)
                {
                    if (item == null) continue;
                    item.Metadata.SubCategory = item.Metadata.SubCategory.Trim().ToLowerInvariant();
                }

                // xử lý version sau này
                if (fileData.Version != 1)
                {
                    // migrate nếu cần
                }

                return (fileData.Items, "Loaded successfully");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
    }
}