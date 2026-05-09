namespace ScriptSupport.Interfaces
{
    public interface IUIConfigInterface
    {
        Task<(bool Success, string Message)> LoadAsync();
    }
}
