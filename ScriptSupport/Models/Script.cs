namespace ScriptSupport.Models
{
    public class ScriptItem
    {
        public string FullPath { get; set; } = string.Empty;
        public string Name => string.IsNullOrEmpty(FullPath) ? string.Empty : System.IO.Path.GetFileName(FullPath);
        public IReadOnlyList<int> LineNumbers { get; set; } = [];
    }
}
