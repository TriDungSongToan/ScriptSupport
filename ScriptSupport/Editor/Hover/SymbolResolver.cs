using ICSharpCode.AvalonEdit.Document;
using Scrapiyard.Core.Models;
using ScriptSupport.Interfaces;

namespace ScriptSupport.Editor.Hover
{
    public sealed class SymbolResolver : ISymbolResolver
    {
        private readonly IScrapiyardInterface _scrapiyardService;
        public SymbolResolver(IScrapiyardInterface scrapiyardService)
        {
            _scrapiyardService = scrapiyardService;
        }
        public CompletionSymbol? Resolve(TextDocument document, int offset)
        {
            if (document == null || offset < 0 || offset >= document.TextLength) return null;

            var token = GetTokenAtOffset(document, offset);
            if (string.IsNullOrEmpty(token)) return null;

            return ResolveSymbol(token);
        }
        private static string GetTokenAtOffset(TextDocument document, int offset)
        {
            int start = offset;
            int end = offset;

            // Nếu đang đứng giữa chữ, lùi lại 1
            if (start > 0 && IsIdentifierChar(document.GetCharAt(start - 1))) start--;

            // Scan backward
            while (start > 0 && IsIdentifierChar(document.GetCharAt(start - 1))) start--;

            // Scan forward
            while (end < document.TextLength && IsIdentifierChar(document.GetCharAt(end))) end++;

            if (start == end) return string.Empty;

            return document.GetText(start, end - start);
        }

        private static bool IsIdentifierChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        private CompletionSymbol? ResolveSymbol(string token)
        {
            IReadOnlyList<CompletionSymbol>? candidates = _scrapiyardService.FindByName(token.ToLowerInvariant());

            if (candidates == null || candidates.Count == 0) return null;

            if (candidates.Count == 1) return candidates[0];

            // Nhiều symbol trùng tên → xử lý tiếp
            return ResolveBestCandidate(candidates);
        }

        private static CompletionSymbol ResolveBestCandidate(IReadOnlyList<CompletionSymbol> candidates)
        {
            // Ưu tiên function > constant > enum member
            return candidates.OrderByDescending(GetKindPriority).First();
        }

        private static int GetKindPriority(CompletionSymbol s)
        {
            return s.Kind switch
            {
                SymbolKind.Function => 100,
                SymbolKind.Constant => 80,
                SymbolKind.EnumMember => 60,
                SymbolKind.Enum => 50,
                _ => 0
            };
        }

        public CompletionSymbol? ResolveExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression)) return null;

            var parts = expression.Split('.');
            if (parts == null || parts.Length == 0) return null;

            CompletionSymbol? current = ResolveRoot(parts[0]);
            if (current == null) return null;

            for (int i = 1; i < parts.Length; i++)
            {
                current = ResolveMember(current, parts[i]);
                if (current == null) return null;
            }

            return current;
        }

        private CompletionSymbol? ResolveRoot(string name)
        {
            IReadOnlyList<CompletionSymbol>? list = _scrapiyardService.FindByName(name.ToLowerInvariant());

            if (list == null || list.Count == 0) return null;

            return list.Count == 1 ? list[0] : ResolveBestCandidate(list);
        }

        private CompletionSymbol? ResolveMember(CompletionSymbol parent, string name)
        {
            if (parent == null) return null;

            // 1. direct members (enum, namespace, graph)
            if (parent.Members != null)
            {
                var direct = parent.Members
                    .FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (direct != null)
                    return direct;
            }

            // 2. TYPE-AWARE: function return type
            if (parent.Kind == SymbolKind.Function &&
                parent.Overloads != null)
            {
                foreach (var ov in parent.Overloads)
                {
                    foreach (var ret in ov.Returns)
                    {
                        foreach (var typeName in ret.Types)
                        {
                            var match = ResolveRoot(typeName);

                            if (match?.Members != null)
                            {
                                var member = match.Members
                                    .FirstOrDefault(m =>
                                        m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                                if (member != null)
                                    return member;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
