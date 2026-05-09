namespace ScriptSupport.Interfaces
{
    public interface IConfigInterface
    {
        Task<(bool Success, string Message)> LoadConfigAsync();
        Task<(bool Success, string Message)> SaveConfigAsync();
        Task<(bool Success, string Message)> ResetConfigAsync();
        Task<(bool Success, string Message)> ApplyConfigAsync();
    }
}
