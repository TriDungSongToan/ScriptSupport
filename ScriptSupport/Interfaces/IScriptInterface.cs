using ScriptSupport.Models;

namespace ScriptSupport.Interfaces
{
    public interface IScriptInterface
    {
        Task<(bool Success, string Message)> LoadScriptsAsync();
        IReadOnlyList<string>? GetListScript(ulong cardId);
        Task SearchFileContent(string query, IProgress<List<ScriptItem>> progress, CancellationToken ct);
    }
}
