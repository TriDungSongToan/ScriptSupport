using System.IO;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;

namespace ScriptSupport.Services
{
    public class DataFolderService : IDataFolderInterface
    {
        private readonly AppEnvironment _env;
        private readonly AppRuntimeConfig _runtimeConfig;
        public DataFolderService(AppEnvironment env, AppRuntimeConfig runtimeConfig)
        {
            _env = env;
            _runtimeConfig = runtimeConfig;
        }

        public async Task<bool> CheckCardDataFolder()
        {
            var dataFolder = _env.DataFolderPath;

            if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);

            string cardDataPath = Path.Combine(dataFolder, "CardData");

            if (!Directory.Exists(cardDataPath))
            {
                var (hasUpdate, message) = await GitHubService.CheckForUpdatesAsync(cardDataPath, _runtimeConfig.CardDataURL);

                if (!string.IsNullOrEmpty(message))
                {

                }

                return hasUpdate;
            }

            return false;
        }
        public async Task<(bool, string)> CheckScrapiyardFolder()
        {
            var dataFolder = _env.DataFolderPath;

            if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);

            string cardDataPath = Path.Combine(dataFolder, "scrapiyard");

            if (!Directory.Exists(cardDataPath))
            {
                var (hasUpdate, message) = await GitHubService.CheckForUpdatesAsync(cardDataPath, _runtimeConfig.ScrapiyardURL);

                if (!string.IsNullOrEmpty(message))
                {

                }

                return (hasUpdate, message);
            }

            return (true, string.Empty);
        }

        public async Task<(bool, string)> CheckUpdateCardData()
        {
            var dataFolder = _env.DataFolderPath;

            if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);

            string cardDataPath = Path.Combine(dataFolder, "CardData");
            var (hasUpdate, message) = await GitHubService.CheckForUpdatesAsync(cardDataPath, _runtimeConfig.CardDataURL);
            return (hasUpdate, message);
        }
        public async Task<(bool, string)> CheckUpdateScrapiyard()
        {
            var dataFolder = _env.DataFolderPath;

            if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);

            string cardDataPath = Path.Combine(dataFolder, "scrapiyard");
            var (hasUpdate, message) = await GitHubService.CheckForUpdatesAsync(cardDataPath, _runtimeConfig.ScrapiyardURL);
            return (hasUpdate, message);
        }

    }
}
