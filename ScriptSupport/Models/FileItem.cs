namespace ScriptSupport.Models
{
    public class FileItem
    {
        public string FullPath { get; set; } = string.Empty;
        public string Name => System.IO.Path.GetFileName(FullPath);
    }
}
