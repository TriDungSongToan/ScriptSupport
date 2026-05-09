namespace ScriptSupport.Interfaces
{
    public interface IApplicationInitializer
    {
        Task InitializeAsync(string[] args);
    }
}
