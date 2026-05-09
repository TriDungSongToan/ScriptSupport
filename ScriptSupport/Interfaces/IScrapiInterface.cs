using ScriptSupport.Models;

namespace ScriptSupport.Interfaces
{
    public interface IScrapiInterface
    {
        Task<(bool Success, string Message)> LoadScrapisAsync();
        Task SearchFileNames(string query, IProgress<List<FileItem>> progress, CancellationToken ct);
        Task SearchFileContent(string query, IProgress<List<FileItem>> progress, CancellationToken ct);
        IReadOnlyList<string>? GetListScrapi(string name);
    }
}
