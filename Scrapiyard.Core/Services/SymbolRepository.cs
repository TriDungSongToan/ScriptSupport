using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrapiyard.Core.Models;

namespace Scrapiyard.Core.Services
{
    public sealed class SymbolRepository
    {
        // =========================
        // RAW DATA (IMMUTABLE)
        // =========================
        public IReadOnlyList<CompletionSymbol> All { get; }

        // =========================
        // INDEXES
        // =========================
        private readonly FrozenDictionary<string, List<CompletionSymbol>> _byName;
        private readonly FrozenDictionary<string, List<CompletionSymbol>> _byFullName;
        private readonly FrozenDictionary<string, List<CompletionSymbol>> _byNamespace;

        // =========================
        // CTOR
        // =========================
        private SymbolRepository(
            List<CompletionSymbol> symbols,
            FrozenDictionary<string, List<CompletionSymbol>> byName,
            FrozenDictionary<string, List<CompletionSymbol>> byFullName,
            FrozenDictionary<string, List<CompletionSymbol>> byNamespace)
        {
            All = symbols;
            _byName = byName;
            _byFullName = byFullName;
            _byNamespace = byNamespace;
        }

        // =========================
        // FACTORY
        // =========================
        public static SymbolRepository Create(IEnumerable<CompletionSymbol> symbols)
        {
            var list = symbols.ToList();

            var byName = new Dictionary<string, List<CompletionSymbol>>();
            var byFullName = new Dictionary<string, List<CompletionSymbol>>();
            var byNamespace = new Dictionary<string, List<CompletionSymbol>>();

            foreach (var s in list)
            {
                var nameKey = Normalize(s.Name);
                var nsKey = Normalize(s.Namespace);
                var fullKey = Normalize(GetFullName(s));

                Add(byName, nameKey, s);
                Add(byFullName, fullKey, s);
                Add(byNamespace, nsKey, s);
            }

            return new SymbolRepository(
                list,
                byName.ToFrozenDictionary(),
                byFullName.ToFrozenDictionary(),
                byNamespace.ToFrozenDictionary()
            );
        }

        // =========================
        // LOOKUP APIs
        // =========================

        public IReadOnlyList<CompletionSymbol> FindByName(string name)
            => Get(_byName, name);

        public IReadOnlyList<CompletionSymbol> FindByFullName(string fullName)
            => Get(_byFullName, fullName);

        public IReadOnlyList<CompletionSymbol> FindByNamespace(string ns)
            => Get(_byNamespace, ns);

        public CompletionSymbol? FirstByName(string name)
            => FindByName(name).FirstOrDefault();

        // =========================
        // SEARCH (optional)
        // =========================
        public IEnumerable<CompletionSymbol> Search(string query)
        {
            query = Normalize(query);

            foreach (var s in All)
            {
                if (s.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    s.Namespace.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    yield return s;
                }
            }
        }

        // =========================
        // HELPERS
        // =========================

        private static string GetFullName(CompletionSymbol s)
            => string.IsNullOrWhiteSpace(s.Namespace)
                ? s.Name
                : $"{s.Namespace}.{s.Name}";

        private static void Add(
            Dictionary<string, List<CompletionSymbol>> dict,
            string key,
            CompletionSymbol value)
        {
            if (!dict.TryGetValue(key, out var list))
            {
                list = new List<CompletionSymbol>();
                dict[key] = list;
            }

            list.Add(value);
        }

        private static IReadOnlyList<CompletionSymbol> Get(
            FrozenDictionary<string, List<CompletionSymbol>> dict,
            string key)
        {
            key = Normalize(key);

            return dict.TryGetValue(key, out var list)
                ? list
                : Array.Empty<CompletionSymbol>();
        }

        private static string Normalize(string? input)
            => input?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
