using System.Text;
using Character.Core.Models;
using Character.Core.Interfaces;
using System.Diagnostics;

namespace Character.Core.Services
{
    public class TagsService : ITagInterface
    {
        public async Task<(List<TagItem>?, string)> ReadLinesAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(filePath)) return (null, "File không tồn tại");

                var result = new List<TagItem>();

                await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read,
                    FileShare.Read, bufferSize: 4096, useAsync: true);

                using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

                string? line;
                while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {

                        result.Add(new TagItem { Name = line });
                    }
                }
                return (result, string.Empty);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
        public async Task<(bool, string)> WriteLinesAsync(List<TagItem> items, string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                if(items == null) return (false, "Item không tồn tại");

                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

                await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
                    FileShare.None, bufferSize: 4096, useAsync: true);

                await using var writer = new StreamWriter(stream, Encoding.UTF8);

                foreach (var item in items)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!string.IsNullOrWhiteSpace(item.Name))
                    {
                        await writer.WriteLineAsync(item.Name);
                    }
                }

                await writer.FlushAsync();
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message); 
            }
        }
        public HashSet<TagItem> ExtractTags(IEnumerable<CharacterItem> items)
        {
            var tagStrings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                if (item.Metadata?.Tags == null) continue;

                foreach (var tagItem in item.Metadata.Tags)
                {
                    var tag = tagItem.Name?.Trim();

                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        tagStrings.Add(tag);
                    }
                }
            }
            return tagStrings.Select(t => new TagItem { Name = t }).ToHashSet();
        }
    }
}
