using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit;

namespace ScriptSupport.Helpers
{
    public static class EditorHelper
    {
        public static string GetSelectedText(object? control)
        {
            return control switch
            {
                TextBox tb => tb.SelectedText,
                RichTextBox rtb => new TextRange(rtb.Selection.Start, rtb.Selection.End).Text,
                TextEditor editor => editor.SelectedText,
                _ => string.Empty
            };
        }

        public static void SetSelectedText(object? control, string text)
        {
            switch (control)
            {
                case TextBox tb:
                    tb.SelectedText = text;
                    break;
                case RichTextBox rtb:
                    rtb.Selection.Text = text;
                    break;
                case TextEditor editor:
                    editor.SelectedText = text;
                    break;
            }
        }

        public static void Cut(object? control)
        {
            switch (control)
            {
                case TextBox tb: tb.Cut(); break;
                case RichTextBox rtb: rtb.Cut(); break;
                case TextEditor editor: editor.Cut(); break;
            }
        }

        public static void Copy(object? control)
        {
            switch (control)
            {
                case TextBox tb: tb.Copy(); break;
                case RichTextBox rtb: rtb.Copy(); break;
                case TextEditor editor: editor.Copy(); break;
            }
        }

        public static void Paste(object? control)
        {
            switch (control)
            {
                case TextBox tb: tb.Paste(); break;
                case RichTextBox rtb: rtb.Paste(); break;
                case TextEditor editor: editor.Paste(); break;
            }
        }
    }
}
