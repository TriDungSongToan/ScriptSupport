using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Scrapiyard.Core.Models;
using ScriptSupport.Editor.Hover;
using ScriptSupport.Editor.Analysis;
using ScriptSupport.Interfaces;

namespace ScriptSupport.Editor.Completion
{
    public sealed class CompletionService : IDisposable
    {
        private readonly TextEditor _editor;
        private readonly IScrapiyardInterface _scrapiyardService;
        private readonly ISymbolResolver _resolver;
        private readonly ISymbolDescriptionPresenter _presenter;
        private CompletionWindow? _completionWindow;

        private string _lastPrefix = string.Empty;

        public CompletionService(TextEditor editor, IScrapiyardInterface scrapiyardService,
            ISymbolDescriptionPresenter presenter)
        {
            _editor = editor;
            _scrapiyardService = scrapiyardService;
            _presenter = presenter;
            _resolver = new SymbolResolver(_scrapiyardService);
        }

        public void ShowCompletion()
        {
            CloseCompletion();

            string prefix = GetCurrentPrefix();
            _completionWindow = new CompletionWindow(_editor.TextArea);
            var data = _completionWindow.CompletionList.CompletionData;

            IReadOnlyList<CompletionSymbol> listSymbol = _scrapiyardService.SearchPrefix(prefix);
            foreach (var symbol in listSymbol)
            {
                data.Add(new CompletionDataAdapter(symbol, _presenter));
            }
            if (data.Count == 0) return;
            _completionWindow.Closed += (_, __) => _completionWindow = null;
            _completionWindow.Show();

            if (!string.IsNullOrEmpty(prefix))
                _completionWindow.CompletionList.SelectItem(prefix);
        }

        private void CloseCompletion()
        {
            _completionWindow?.Close();
            _completionWindow = null;
            _lastPrefix = string.Empty;
        }

        private string GetCurrentPrefix()
        {
            var doc = _editor.Document;
            var offset = _editor.CaretOffset;

            if (offset == 0) return string.Empty;
            int start = offset - 1;
            while (start >= 0)
            {
                char c = doc.GetCharAt(start);
                if (!char.IsLetterOrDigit(c) && c != '_') break;
                start--;
            }
            start++;
            return doc.GetText(start, offset - start);
        }

        public void OnTextEntered(char enteredChar)
        {
            if (!IsIdentifierChar(enteredChar) && enteredChar != '.')
            {
                CloseCompletion();
                return;
            }

            var context = AnalyzeContext();

            if (string.IsNullOrEmpty(context.Prefix) && !context.IsDotCompletion)
            {
                CloseCompletion();
                return;
            }

            ShowOrUpdateCompletion(context);
        }

        private void ShowOrUpdateCompletion(CompletionContext context)
        {
            if (_completionWindow == null)
            {
                _completionWindow = new CompletionWindow(_editor.TextArea);
                _completionWindow.Closed += (_, __) => _completionWindow = null;
                //_completionWindow.Show();
            }

            UpdateCompletionList(context);

            if (_completionWindow != null && !_completionWindow.IsVisible)
                _completionWindow.Show();
        }

        private void UpdateCompletionList(CompletionContext context)
        {
            if (_completionWindow == null) return;

            var data = _completionWindow.CompletionList.CompletionData;
            data.Clear();

            if (context.IsDotCompletion)
            {
                AddDotCompletionItems(context, data);
            }
            else
            {
                AddGlobalCompletionItems(context, data);
            }
            if (data.Count > 0)
            {
                _completionWindow.CompletionList.SelectItem(context.Prefix);
            }
            else
            {
                CloseCompletion();
            }
        }
        private void AddGlobalCompletionItems(CompletionContext context, IList<ICompletionData> data)
        {
            IReadOnlyList<CompletionSymbol> listSymbol = _scrapiyardService.SearchPrefix(context.Prefix);

            foreach (var symbol in listSymbol)
            {
                data.Add(new CompletionDataAdapter(symbol, _presenter));
            }
        }


        private void AddDotCompletionItems(CompletionContext context, IList<ICompletionData> data)
        {
            if (string.IsNullOrEmpty(context.Qualifier)) return;

            string qualifier = context.Qualifier;

            // 1. Resolve semantic symbol (QUAN TRỌNG NHẤT)
            var symbol = _resolver.ResolveExpression(qualifier);

            // 2. Fallback: nếu không resolve được → thử namespace lookup nhanh
            if (symbol == null)
            {
                IReadOnlyList<CompletionSymbol>? listSymbol = _scrapiyardService.FindByNameSpace(qualifier);
                if (listSymbol == null) return;

                foreach (var item in listSymbol)
                {
                    if (!IsMatchPrefix(item, context.Prefix)) continue;
                    data.Add(new CompletionDataAdapter(item, _presenter));
                }
                return;
            }

            // 3. Enum members / graph members (PRIMARY PATH)
            if (symbol.Members != null && symbol.Members.Count > 0)
            {
                foreach (var member in symbol.Members)
                {
                    if (!IsMatchPrefix(member, context.Prefix)) continue;
                    data.Add(new CompletionDataAdapter(member, _presenter));
                }

                return;
            }

            // 4. Fallback: nếu symbol không có graph → thử namespace children
            IReadOnlyList<CompletionSymbol>? namespaceSymbols = _scrapiyardService.FindByNameSpace(qualifier);
            if (namespaceSymbols == null) return;

            foreach (var s in namespaceSymbols)
            {
                if (!IsMatchPrefix(s, context.Prefix)) continue;
                data.Add(new CompletionDataAdapter(s, _presenter));
            }
        }

        private static bool IsMatchPrefix(CompletionSymbol symbol, string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) return true;

            return symbol.Name.StartsWith(
                prefix,
                StringComparison.OrdinalIgnoreCase);
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseCompletion();
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                var context = AnalyzeContext();

                if (string.IsNullOrEmpty(context.Prefix) && !context.IsDotCompletion)
                {
                    CloseCompletion();
                    return;
                }

                ShowOrUpdateCompletion(context);
            }
        }

        private static bool IsIdentifierChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        private CompletionContext AnalyzeContext()
        {
            var doc = _editor.Document;
            int offset = _editor.CaretOffset;

            if (offset == 0) return new CompletionContext(null, string.Empty);

            int i = offset - 1;

            // 1. Parse prefix (right side)
            while (i >= 0)
            {
                char c = doc.GetCharAt(i);
                if (!char.IsLetterOrDigit(c) && c != '_') break;
                i--;
            }

            int prefixStart = i + 1;
            string prefix = doc.GetText(prefixStart, offset - prefixStart);

            // 2. Check dot
            if (i < 0 || doc.GetCharAt(i) != '.') return new CompletionContext(null, prefix);

            // 3. Parse qualifier (left side)
            i--; // skip dot
            int end = i;

            while (i >= 0)
            {
                char c = doc.GetCharAt(i);
                if (!char.IsLetterOrDigit(c) && c != '_') break;
                i--;
            }

            int qualifierStart = i + 1;
            string qualifier = doc.GetText(qualifierStart, end - qualifierStart + 1);

            return new CompletionContext(qualifier, prefix);
        }

        public void Dispose()
        {
            CloseCompletion();
        }
    }
}
