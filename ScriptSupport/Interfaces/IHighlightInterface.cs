namespace ScriptSupport.Interfaces
{
    public interface IHighlightInterface
    {
        Task LoadAsync(string xshdPath);
        Task ReloadAsync();
        void Dispose();
    }
}
