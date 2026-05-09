using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

using ScriptSupport.States;

namespace ScriptSupport.Theming
{
    public class LineHighlightRenderer : IBackgroundRenderer
    {
        private readonly TextView _textView;
        private readonly UIConfigState _uiConfig;
        private List<int> _lines = new();

        public LineHighlightRenderer(TextView textView, UIConfigState uiConfig)
        {
            _textView = textView;
            _uiConfig = uiConfig;
        }

        public KnownLayer Layer => KnownLayer.Background;

        public void SetLines(IEnumerable<int> lines)
        {
            _lines = lines?.Where(l => l > 0).Distinct().ToList() ?? new List<int>();
            _textView.InvalidateLayer(Layer);
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView?.Document == null) return;
            if (_lines == null || _lines.Count == 0) return;

            var brush = _uiConfig.ThemeColor.Clone();
            brush.Opacity = 0.4;
            brush.Freeze();

            foreach (var lineNumber in _lines)
            {
                if (lineNumber <= 0 || lineNumber > textView.Document.LineCount) continue;

                var docLine = textView.Document.GetLineByNumber(lineNumber);
                if (docLine == null) continue;

                var rects = BackgroundGeometryBuilder.GetRectsForSegment(textView, docLine);

                foreach (var rect in rects)
                {
                    var fullRect = new Rect(0, rect.Top, textView.ActualWidth, rect.Height);
                    drawingContext.DrawRectangle(brush, null, fullRect);
                }
            }
            //if (_line <= 0) return;
            //if (textView?.Document == null) return;
            //if (_line > textView.Document.LineCount) return;

            //var docLine = textView.Document.GetLineByNumber(_line);
            //if (docLine == null) return;

            //var brush = _uiConfig.ThemeColor.Clone();
            //brush.Opacity = 0.4;
            //brush.Freeze();

            //var rects = BackgroundGeometryBuilder.GetRectsForSegment(textView, docLine);

            //foreach (var rect in rects)
            //{
            //    var fullRect = new Rect(0, rect.Top, textView.ActualWidth, rect.Height);
            //    drawingContext.DrawRectangle(brush, null, fullRect);
            //}
        }
    }
}
