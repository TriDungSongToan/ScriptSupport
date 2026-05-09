using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Rendering;

namespace ScriptSupport.Generator
{
    public class HyperlinkElement : VisualLineElementGenerator
    {
        public Action<string>? OnLinkClicked { get; set; }
        private static readonly Regex LinkRegex =
            new(@"\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);

        private Match FindMatch(int startOffset)
        {
            var line = CurrentContext.VisualLine.LastDocumentLine;
            int lineStart = line.Offset;
            string lineText = CurrentContext.Document.GetText(lineStart, line.Length);

            // Tìm match trong line, nhưng chỉ từ vị trí (startOffset - lineStart)
            int searchStart = startOffset - lineStart;
            return LinkRegex.Match(lineText, searchStart);
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            var match = FindMatch(startOffset);
            if (!match.Success) return -1;

            int lineStart = CurrentContext.VisualLine.LastDocumentLine.Offset;
            return lineStart + match.Index;
        }

        public override VisualLineElement? ConstructElement(int offset)
        {
            var match = FindMatch(offset);
            if (!match.Success) return null;

            int lineStart = CurrentContext.VisualLine.LastDocumentLine.Offset;
            if (lineStart + match.Index != offset) return null;

            string header = match.Groups[1].Value;
            string url = match.Groups[2].Value;

            var hyperlink = new Hyperlink(new Run(header))
            {
                NavigateUri = new Uri(url, UriKind.RelativeOrAbsolute)
            };
            hyperlink.RequestNavigate += OnRequestNavigate;

            // InlineObjectElement nhận UIElement → dùng TextBlock làm UIElement
            var textBlock = new TextBlock(hyperlink)
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            return new InlineObjectElement(match.Length, textBlock);
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string url = e.Uri.OriginalString;
            OnLinkClicked?.Invoke(url);
            e.Handled = true;
        }
    }
}
