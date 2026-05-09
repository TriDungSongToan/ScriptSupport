namespace ScriptSupport.Interfaces
{
    public interface IFilterConfigInterface
    {
        Task<(bool Success, string Message)> LoadAsync();
    }
}
