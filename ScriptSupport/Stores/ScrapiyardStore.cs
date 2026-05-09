using System.Collections.Frozen;
using Scrapiyard.Core.Models;

namespace ScriptSupport.Stores
{
    public class ScrapiyardStore : IDisposable
    {
        public int Version { get; private set; }
        public IReadOnlyList<CompletionSymbol> AllSymbols { get; private set; } = Array.Empty<CompletionSymbol>();
        public FrozenDictionary<string, List<CompletionSymbol>> _byName = FrozenDictionary<string, List<CompletionSymbol>>.Empty;
        public FrozenDictionary<string, List<CompletionSymbol>> _byNamespace = FrozenDictionary<string, List<CompletionSymbol>>.Empty;
        public FrozenDictionary<string, List<CompletionSymbol>> _byFullName = FrozenDictionary<string, List<CompletionSymbol>>.Empty;

        public void Set(IReadOnlyList<CompletionSymbol> symbols)
        {
            var list = symbols.ToArray();

            var byName = new Dictionary<string, List<CompletionSymbol>>(StringComparer.OrdinalIgnoreCase);
            var byNs = new Dictionary<string, List<CompletionSymbol>>(StringComparer.OrdinalIgnoreCase);
            var byFull = new Dictionary<string, List<CompletionSymbol>>(StringComparer.OrdinalIgnoreCase);

            foreach (var s in list)
            {
                if (!string.IsNullOrEmpty(s.Name)) Add(byName, s.Name, s);
                if (!string.IsNullOrEmpty(s.Namespace)) Add(byNs, s.Namespace, s);
                Add(byFull, GetFullName(s), s);
            }

            AllSymbols = list;

            _byName = ToFrozen(byName);
            _byNamespace = ToFrozen(byNs);
            _byFullName = ToFrozen(byFull);

            Version++;
        }
        private void Add(Dictionary<string, List<CompletionSymbol>> dict, string key, CompletionSymbol value)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            if (!dict.TryGetValue(key, out var list))
            {
                list = new List<CompletionSymbol>();
                dict[key] = list;
            }

            list.Add(value);
        }
        private static FrozenDictionary<string, List<CompletionSymbol>> ToFrozen(Dictionary<string, List<CompletionSymbol>> dict)
        {
            return dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        }
        private string GetFullName(CompletionSymbol s)
        {
            return string.IsNullOrWhiteSpace(s.Namespace) ? s.Name : $"{s.Namespace}.{s.Name}";
        }

        public void Dispose()
        {
            AllSymbols = Array.Empty<CompletionSymbol>();

            _byName = FrozenDictionary<string, List<CompletionSymbol>>.Empty;
            _byNamespace = FrozenDictionary<string, List<CompletionSymbol>>.Empty;
            _byFullName = FrozenDictionary<string, List<CompletionSymbol>>.Empty;

            Version = 0;
        }
    }
}
