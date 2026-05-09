namespace ScriptSupport.Interfaces
{
    public interface IStringInterface
    {
        (bool, string) SetClipboard(string input);
        void InsertAtCaret(string text);
    }
}
