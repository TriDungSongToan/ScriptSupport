using System.Text.Json;
using System.Text.Json.Serialization;
using Scrapiyard.Core.Models;

namespace Scrapiyard.Core.Services;

/// <summary>
/// Đọc file JSON → IReadOnlyList&lt;CompletionSymbol&gt;
/// </summary>
public static class SymbolLoader
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    // ==========================================
    // PUBLIC API
    // ==========================================

    /// <summary>
    /// Load một file JSON duy nhất.
    /// </summary>
    /// <param name="jsonFilePath">Đường dẫn tới file JSON.</param>
    public static async Task<LoadResult> LoadFileAsync(string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath))
            return LoadResult.Fail("jsonFilePath is null or empty.");

        if (!File.Exists(jsonFilePath))
            return LoadResult.Fail($"File not found: {jsonFilePath}");

        try
        {
            await using var stream = File.OpenRead(jsonFilePath);

            var symbols =
                await JsonSerializer.DeserializeAsync<List<CompletionSymbol>>(stream, _jsonOptions)
                ?? new List<CompletionSymbol>();

            return LoadResult.Ok(symbols);
        }
        catch (Exception ex)
        {
            return LoadResult.Fail($"Failed to load '{jsonFilePath}': {ex.Message}");
        }
    }

    /// <summary>
    /// Load nhiều file JSON cùng lúc, gộp tất cả symbol lại.
    /// </summary>
    /// <param name="jsonFilePaths">Danh sách đường dẫn tới các file JSON.</param>
    public static async Task<LoadResult> LoadFilesAsync(IEnumerable<string> jsonFilePaths)
    {
        if (jsonFilePaths == null)
            return LoadResult.Fail("jsonFilePaths is null.");

        var tasks = jsonFilePaths.Select(LoadFileAsync);
        var results = await Task.WhenAll(tasks);

        var all = new List<CompletionSymbol>();
        var errors = new List<string>();

        foreach (var r in results)
        {
            if (r.Success)
                all.AddRange(r.Symbols);
            else
                errors.Add(r.Error ?? "unknown error");
        }

        if (all.Count == 0 && errors.Count > 0)
            return LoadResult.Fail(string.Join("; ", errors));

        return LoadResult.Ok(all);
    }

    /// <summary>
    /// Load tất cả file JSON trong một folder (không đệ quy).
    /// </summary>
    /// <param name="folderPath">Đường dẫn thư mục chứa các file JSON.</param>
    /// <param name="searchPattern">Pattern tìm file, mặc định là "*.json".</param>
    public static async Task<LoadResult> LoadFolderAsync(string folderPath, string searchPattern = "*.json")
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            return LoadResult.Fail("folderPath is null or empty.");

        if (!Directory.Exists(folderPath))
            return LoadResult.Fail($"Folder not found: {folderPath}");

        var files = Directory.GetFiles(folderPath, searchPattern, SearchOption.TopDirectoryOnly);

        return await LoadFilesAsync(files);
    }
}
