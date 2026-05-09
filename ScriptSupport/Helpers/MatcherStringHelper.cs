using System.Text.RegularExpressions;
using ScriptSupport.States;

namespace ScriptSupport.Helpers
{
    public static class SymbolMatcher
    {
        /// <summary>
        /// Trả về một hàm so sánh (candidate, query) → bool
        /// dựa trên FilterConfigState hiện tại.
        /// </summary>
        public static Func<string, string, bool> Build(FilterConfigState config)
        {
            return (candidate, query) =>
            {
                if (string.IsNullOrWhiteSpace(query)) return true;
                if (string.IsNullOrWhiteSpace(candidate)) return false;

                var c = Normalize(candidate, config);
                var q = Normalize(query, config);

                var comparison = config.MatchCase
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase;

                bool result = true;

                // --- Wildcard ---
                if (config.Wildcards)
                {
                    if (!q.Contains('*') && !q.Contains('?'))
                        q = $"*{q}*";

                    result &= MatchWildcard(c, q, config.MatchCase);

                    // ⚠️ Nếu đã wildcard thì bỏ qua prefix/suffix/whole
                    // vì chúng mâu thuẫn logic
                    return result;
                }

                // --- Whole ---
                if (config.MatchWhole)
                {
                    result &= c.Equals(q, comparison);
                }

                // --- Prefix ---
                if (config.Prefix)
                {
                    result &= c.StartsWith(q, comparison);
                }

                // --- Suffix ---
                if (config.Suffix)
                {
                    result &= c.EndsWith(q, comparison);
                }

                // --- Advanced (fuzzy) ---
                if (config.Advanced)
                {
                    result &= MatchSubsequence(c, q, config.MatchCase);
                }

                // --- Default ---
                if (!config.MatchWhole &&
                    !config.Prefix &&
                    !config.Suffix &&
                    !config.Advanced)
                {
                    result &= c.Contains(q, comparison);
                }

                return result;
            };
        }

        // --- Normalize ---
        private static string Normalize(string input, FilterConfigState config)
        {
            if (config.Ignpunct)
                input = new string(input.Where(ch => !char.IsPunctuation(ch) && !char.IsSymbol(ch)).ToArray());

            if (config.Ignpspace)
                input = new string(input.Where(ch => !char.IsWhiteSpace(ch)).ToArray());

            if (!config.MatchCase)
                input = input.ToLowerInvariant();

            return input;
        }

        // --- Wildcard: * (nhiều ký tự), ? (1 ký tự) ---
        private static bool MatchWildcard(string text, string pattern, bool matchCase)
        {
            var regexPattern = "^" + Regex.Escape(pattern)
                .Replace(@"\*", ".*")
                .Replace(@"\?", ".") + "$";

            var options = matchCase ? RegexOptions.None : RegexOptions.IgnoreCase;

            return Regex.IsMatch(text, regexPattern, options);
        }

        // --- Fuzzy: subsequence (kiểu VS Code) ---
        // "GAU" match "GetActiveUnit" vì G→A→U xuất hiện theo đúng thứ tự
        private static bool MatchSubsequence(string text, string query, bool matchCase)
        {
            int ti = 0, qi = 0;

            while (ti < text.Length && qi < query.Length)
            {
                var tc = matchCase ? text[ti] : char.ToLowerInvariant(text[ti]);
                var qc = matchCase ? query[qi] : char.ToLowerInvariant(query[qi]);

                if (tc == qc) qi++;
                ti++;
            }

            return qi == query.Length;
        }
    }
}
