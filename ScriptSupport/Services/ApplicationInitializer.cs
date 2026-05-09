using ScriptSupport.Stores;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;
using ScriptSupport.Localization;

namespace ScriptSupport.Services
{
    public class ApplicationInitializer : IApplicationInitializer
    {
        private readonly IDataFolderInterface _dataFolder;
        private readonly IConfigInterface _config;
        private readonly ILanguageInterface _language;
        private readonly ConfigStore _store;
        private readonly AppRuntimeConfig _runtimeConfig;

        public ApplicationInitializer(IDataFolderInterface dataFolder, IConfigInterface config,
            ILanguageInterface language, ConfigStore store, AppRuntimeConfig runtimeConfig)
        {
            _dataFolder = dataFolder;
            _config = config;
            _language = language;
            _store = store;
            _runtimeConfig = runtimeConfig;
        }

        public async Task InitializeAsync(string[] args)
        {
            // Check & clone
            await _dataFolder.CheckCardDataFolder();

            // Load config
            var (success, message) = await _config.LoadConfigAsync();
            if (!success) throw new Exception(message);

            // Bridge static
            LanguageProvider.Current = _language;

            // Load language
            string languageCode = _store.UserSetting?.Language ?? _runtimeConfig.DefaultLanguage;

            try
            {
                _language.LoadLanguage(languageCode);
            }
            catch
            {
                _language.LoadLanguage(_runtimeConfig.DefaultLanguage);
            }
        }

    }
}
