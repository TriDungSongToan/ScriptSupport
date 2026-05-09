using Scrapiyard.Core.Models;

namespace ScriptSupport.Interfaces
{
    public interface IScrapiyardInterface
    {
        Task<(bool Success, string Message)> LoadSymbols();
        IReadOnlyList<CompletionSymbol> SearchName(string input, int max = 50);
        IReadOnlyList<CompletionSymbol> SearchDesc(string input, int max = 50);
        IReadOnlyList<CompletionSymbol> SearchPrefix(string prefix, int max = 50);
        IEnumerable<CompletionSymbol> Suggest(string prefix, int max = 20);
        IReadOnlyList<CompletionSymbol>? FindByName(string key);
        IReadOnlyList<CompletionSymbol>? FindByNameSpace(string key);
        IReadOnlyList<CompletionSymbol>? FindByFullName(string fullName);
        IEnumerable<CompletionSymbol> FilterByKind(SymbolKind kind);
        IEnumerable<CompletionSymbol> FilterByTag(string tag);
    }
}
