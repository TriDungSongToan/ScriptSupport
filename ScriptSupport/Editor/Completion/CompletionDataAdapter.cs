using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Scrapiyard.Core.Models;
using ScriptSupport.Editor.Analysis;

namespace ScriptSupport.Editor.Completion
{
    public sealed class CompletionDataAdapter : ICompletionData
    {
        private readonly ISymbolDescriptionPresenter _presenter;

        public CompletionSymbol Symbol { get; }
        public CompletionDataAdapter(CompletionSymbol symbol, ISymbolDescriptionPresenter presenter)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            _presenter = presenter;
        }

        public string Text => Symbol.Name;
        public object Content => Symbol.Name;
        public object Description => _presenter.Create(Symbol);

        public double Priority => GetPriority();
        public ImageSource? Image => IconProvider.GetIcon(Symbol.Kind);

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            var document = textArea.Document;
            int caretOffset = textArea.Caret.Offset;

            // Tìm vị trí bắt đầu của prefix (sau dấu . nếu có)
            int start = caretOffset - 1;
            while (start >= 0)
            {
                char c = document.GetCharAt(start);
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '.')
                {
                    start++;
                    break;
                }
                start--;
            }

            if (start < 0) start = 0;

            int length = caretOffset - start;
            string existing = document.GetText(start, length);
            if (existing.Contains('.') && !string.IsNullOrEmpty(Symbol.Namespace))
            {
                document.Replace(start, length, $"{Symbol.Namespace}.{Symbol.Name}");
            }
            else
            {
                document.Replace(start, length, Symbol.Name);
            }
        }

        private double GetPriority()
        {
            return Symbol.Kind switch
            {
                SymbolKind.Function => 100,
                SymbolKind.Constant => 80,
                SymbolKind.Enum => 70,
                SymbolKind.EnumMember => 60,
                _ => 0
            };
        }
    }
}
