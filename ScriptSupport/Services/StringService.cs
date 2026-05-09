using ScriptSupport.Manager;
using ScriptSupport.Interfaces;

namespace ScriptSupport.Services
{
    public class StringService : IStringInterface
    {
        private readonly DocumentManager _documentManager;
        public StringService(DocumentManager documentManager)
        {
            _documentManager = documentManager;
        }

        public (bool, string) SetClipboard(string input)
        {
            try
            {
                System.Windows.Clipboard.SetText(input);
                return (true, "Text copied to clipboard successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to copy text to clipboard: {ex.Message}");
            }
        }
        public void InsertAtCaret(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            _documentManager.InsertAtCaret(text);
        }
    }
}
