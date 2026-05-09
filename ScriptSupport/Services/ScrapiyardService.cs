using Scrapiyard.Core.Models;
using Scrapiyard.Core.Services;
using ScriptSupport.States;
using ScriptSupport.Stores;
using ScriptSupport.Helpers;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;

namespace ScriptSupport.Services
{
    public class ScrapiyardService : IScrapiyardInterface
    {
        private readonly ConfigStore _config;
        private readonly AppEnvironment _aev;
        private readonly ScrapiyardStore _store;
        private readonly FilterConfigState _filterState;

        public ScrapiyardService(ConfigStore config, AppEnvironment aev, ScrapiyardStore store, FilterConfigState filterState)
        {
            _config = config;
            _aev = aev;
            _store = store;
            _filterState = filterState;
        }

        #region Load
        public async Task<(bool Success, string Message)> LoadSymbols()
        {
            string lag = _config.UserSetting.Language;
            if (string.IsNullOrWhiteSpace(lag)) return (false, "Language not specified in settings.");
            string folderPath = System.IO.Path.Combine(_aev.DataFolderPath, $@"CardData\Language\{lag}\scriptinfo");
            if (!System.IO.Directory.Exists(folderPath)) return (false, $"Language folder not found: {folderPath}");

            try
            {
                var result = await SymbolLoader.LoadFolderAsync(folderPath);
                if (!result.Success) return (false, $"Failed to load symbols: {result.Error}");

                _store.Set(result.Symbols);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        #endregion

        #region Search
        public IReadOnlyList<CompletionSymbol> SearchName(string input, int max = 50)
        {
            if (string.IsNullOrWhiteSpace(input)) return Array.Empty<CompletionSymbol>();

            var match = SymbolMatcher.Build(_filterState);
            input = input.Trim();

            return _store.AllSymbols
                .Select(s => (Symbol: s, Score: ScoreName(s, input, match)))
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Take(max)
                .Select(x => x.Symbol)
                .ToList();
        }
        public IReadOnlyList<CompletionSymbol> SearchDesc(string input, int max = 50)
        {
            if (string.IsNullOrWhiteSpace(input)) return Array.Empty<CompletionSymbol>();

            var match = SymbolMatcher.Build(_filterState);
            input = input.Trim();

            return _store.AllSymbols
                .Select(s => (Symbol: s, Score: ScoreDesc(s, input, match)))
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Take(max)
                .Select(x => x.Symbol)
                .ToList();
        }
        public IReadOnlyList<CompletionSymbol> SearchPrefix(string prefix, int max = 50)
        {
            if (string.IsNullOrWhiteSpace(prefix)) return Array.Empty<CompletionSymbol>();

            prefix = prefix.Trim();

            // 1. Namespace match: tìm trong _byName các symbol có Kind = NameSpace
            var namespaceMatches = _store._byName
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .SelectMany(kv => kv.Value)
                .Where(s => s.Kind == SymbolKind.NameSpace)
                .ToList();

            // 2. Name match: các symbol còn lại (không phải NameSpace)
            var nameMatches = _store._byName
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .SelectMany(kv => kv.Value)
                .Where(s => s.Kind != SymbolKind.NameSpace)
                .ToList();

            // 3. Gộp: namespace trên, name dưới
            return namespaceMatches
                .Concat(nameMatches)
                .Take(max)
                .ToList();
        }
        private static int ScoreName(CompletionSymbol s, string input, Func<string, string, bool> matcher)
        {
            if (!string.IsNullOrEmpty(s.Name))
            {
                // Exact match — cao nhất
                if (s.Name.Equals(input, StringComparison.OrdinalIgnoreCase)) return 3;

                // Name match theo filter mode
                if (matcher(s.Name, input)) return 2;
            }

            // Namespace match — thấp hơn
            if (!string.IsNullOrEmpty(s.Namespace) && matcher(s.Namespace, input)) return 1;

            return 0;
        }
        private static int ScoreDesc(CompletionSymbol s, string input, Func<string, string, bool> matcher)
        {
            bool matchName = !string.IsNullOrEmpty(s.Name) && matcher(s.Name, input);

            bool matchNamespace = !string.IsNullOrEmpty(s.Namespace) && matcher(s.Namespace, input);

            bool matchText = (!string.IsNullOrEmpty(s.Summary) && matcher(s.Summary, input)) ||
                (!string.IsNullOrEmpty(s.Description) && matcher(s.Description, input));

            if (matchName || matchNamespace) return 2;
            if (matchText) return 1;
            return 0;
        }
        private static bool MatchesDescription(CompletionSymbol s, string input, Func<string, string, bool> matcher)
        {
            return
                (!string.IsNullOrEmpty(s.Name) && matcher(s.Name, input)) ||
                (!string.IsNullOrEmpty(s.Namespace) && matcher(s.Namespace, input)) ||
                (!string.IsNullOrEmpty(s.Summary) && matcher(s.Summary, input)) ||
                (!string.IsNullOrEmpty(s.Description) && matcher(s.Description, input));
        }
        #endregion

        #region Suggest
        public IEnumerable<CompletionSymbol> Suggest(string prefix, int max = 20)
        {
            if (string.IsNullOrWhiteSpace(prefix)) return Array.Empty<CompletionSymbol>();

            prefix = prefix.Trim();

            return _store.AllSymbols.Where(s => s.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Take(max);
        }
        #endregion

        #region Filter
        public IReadOnlyList<CompletionSymbol>? FindByName(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            return _store._byName.TryGetValue(key, out var list) ? list : null;
        }
        public IReadOnlyList<CompletionSymbol>? FindByNameSpace(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            return _store._byNamespace.TryGetValue(key, out var list) ? list : null;
        }
        public IReadOnlyList<CompletionSymbol>? FindByFullName(string fullName)
        {
            var parts = fullName.Split('.');
            if (parts.Length == 1) return FindByName(fullName);

            var ns = string.Join('.', parts.Take(parts.Length - 1));
            var name = parts[^1];

            return _store.AllSymbols.Where(s =>
                    s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    s.Namespace.Equals(ns, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        public IEnumerable<CompletionSymbol> FilterByKind(SymbolKind kind)
        {
            return _store.AllSymbols.Where(s => s.Kind == kind);
        }
        public IEnumerable<CompletionSymbol> FilterByTag(string tag)
        {
            return _store.AllSymbols.Where(s => s.Tags.Contains(tag));
        }
        #endregion

    }
}
