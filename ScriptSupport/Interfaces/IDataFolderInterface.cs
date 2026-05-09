namespace ScriptSupport.Interfaces
{
    public interface IDataFolderInterface
    {
        Task<bool> CheckCardDataFolder();
        Task<(bool, string)> CheckScrapiyardFolder();
        Task<(bool, string)> CheckUpdateCardData();
        Task<(bool, string)> CheckUpdateScrapiyard();
    }
}
