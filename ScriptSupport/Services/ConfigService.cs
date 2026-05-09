using System.IO;
using System.Text.Json;
using System.Data.Common;
using System.Data.SQLite;
using ScriptSupport.Stores;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;
using ScriptSupport.Models.Settings;

namespace ScriptSupport.Services
{
    public class ConfigService : IConfigInterface
    {
        private readonly AppEnvironment _env;
        private readonly ConfigStore _store;
        private readonly ICardInterface _cardService;
        private readonly IScriptInterface _scriptService;
        private readonly IImageCardInterface _imageCardService;
        private readonly ILanguageInterface _languageService;
        private readonly IUIConfigInterface _uiConfigService;
        private readonly ICardElelemtInterface _cardElelemtService;
        private readonly ISpecialCharInterface _specialCharService;
        private readonly IFilterConfigInterface _filterConfigService;
        private readonly ICodeEditConfigInterface _codeEditorConfigService;
        public ConfigService(AppEnvironment env, ConfigStore store,
            ICardInterface cardService, IScriptInterface scriptService,
            IImageCardInterface imageCardService,
            ILanguageInterface lang, IUIConfigInterface uiConfigService,
            ICardElelemtInterface cardElelemtService, ISpecialCharInterface specialCharService,
            IFilterConfigInterface iFilterConfigService, ICodeEditConfigInterface codeEditorConfigService)
        {
            _env = env;
            _store = store;
            _cardService = cardService;
            _scriptService = scriptService;
            _imageCardService = imageCardService;

            _languageService = lang;
            _uiConfigService = uiConfigService;
            _store.SettingFilePath = Path.Combine(_env.ConfigFolderPath, "AppSetting.db");
            _cardElelemtService = cardElelemtService;
            _specialCharService = specialCharService;
            _filterConfigService = iFilterConfigService;
            _codeEditorConfigService = codeEditorConfigService;
        }

        public async Task<(bool Success, string Message)> LoadConfigAsync()
        {
            if (string.IsNullOrEmpty(_store.SettingFilePath) || !System.IO.File.Exists(_store.SettingFilePath))
            {
                var (resultCreate, messageCreate) =
                    await CreateFileServices.CreateSetting(_env.ConfigFolderPath, "AppSetting.db");
                if (!resultCreate) return (false, messageCreate);
            }

            string connectionString = $"Data Source={_store.SettingFilePath};Version=3;";
            try
            {
                var userTask = LoadSetting<UserSetting>("UserSetting", connectionString);
                var displayTask = LoadSetting<DisplaySetting>("DisplaySetting", connectionString);
                var dataTask = LoadSetting<DataHandlingSetting>("DataHandlingSetting", connectionString);
                var codeTask = LoadSetting<CodeEditSetting>("CodeEditSetting", connectionString);
                var filterTask = LoadSetting<FilterSetting>("FilterSetting", connectionString);
                await Task.WhenAll(userTask, displayTask, dataTask, codeTask, filterTask);

                _store.UserSetting = await userTask;
                _store.DisplaySetting = await displayTask;
                _store.DataHandlingSetting = await dataTask;
                _store.CodeEditSetting = await codeTask;
                _store.FilterSetting = await filterTask;

                return (true, "Config loaded successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error loading config: {ex.Message}");
            }
        }
        public async Task<(bool Success, string Message)> SaveConfigAsync()
        {
            if (string.IsNullOrEmpty(_store.SettingFilePath) || !System.IO.File.Exists(_store.SettingFilePath))
            {
                var (resultCreate, messageCreate) =
                    await CreateFileServices.CreateSetting(_env.ConfigFolderPath, "AppSetting.db");
                if (!resultCreate) return (false, messageCreate);
            }

            string connectionString = $"Data Source={_store.SettingFilePath};Version=3;";
            var jsonOptions = new JsonSerializerOptions { WriteIndented = false };

            await using var connection = new SQLiteConnection(connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await InsertOrReplaceSetting(connection, transaction, "UserSetting", _store.UserSetting, jsonOptions);
                await InsertOrReplaceSetting(connection, transaction, "DisplaySetting", _store.DisplaySetting, jsonOptions);
                await InsertOrReplaceSetting(connection, transaction, "DataHandlingSetting", _store.DataHandlingSetting, jsonOptions);
                await InsertOrReplaceSetting(connection, transaction, "CodeEditSetting", _store.CodeEditSetting, jsonOptions);
                await InsertOrReplaceSetting(connection, transaction, "FilterSetting", _store.FilterSetting, jsonOptions);
                await transaction.CommitAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }
        public async Task<(bool Success, string Message)> ResetConfigAsync()
        {
            if (string.IsNullOrEmpty(_store.SettingFilePath) || !System.IO.File.Exists(_store.SettingFilePath))
            {
                var (resultCreate, messageCreate) =
                    await CreateFileServices.CreateSetting(_env.ConfigFolderPath, "AppSetting.db");
                if (!resultCreate) return (false, messageCreate);
            }

            string connectionString = $"Data Source={_store.SettingFilePath};Version=3;";

            await using var connection = new SQLiteConnection(connectionString);
            await connection.OpenAsync();

            await using var transaction = (SQLiteTransaction)await connection.BeginTransactionAsync();

            try
            {
                // Xoá toàn bộ settings
                await using (var deleteCmd = new SQLiteCommand("DELETE FROM Settings", connection, transaction))
                {
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                // Tạo lại default settings trong cùng transaction
                await CreateFileServices.CreateDefaultSettings(connection, transaction);
                await transaction.CommitAsync();

                return await LoadConfigAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }
        public async Task<(bool Success, string Message)> ApplyConfigAsync()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_store.UserSetting.Language))
                    _languageService.LoadLanguage(_store.UserSetting.Language);
                var (UISuccess, UIMessage) = await _uiConfigService.LoadAsync();
                var (FilSuccess, FilMessage) = await _filterConfigService.LoadAsync();
                var (CardSuccess, CardMessage) = await _cardElelemtService.LoadAllCardElement();
                var (SuccessSpecialChar, MessageSpecialChar) = await _specialCharService.LoadChar();

                var (SuccessDB, MessageDB) = await _cardService.LoadCardDBAsync();
                var (SuccessSc, MessageSc) = await _scriptService.LoadScriptsAsync();
                var (SuccessImg, MessageImg) = await _imageCardService.LoadCardImagesAsync();

                _codeEditorConfigService.NotifyChanged();

                var results = new (bool Success, string Message)[]
                {
                    (UISuccess, UIMessage),
                    (FilSuccess, FilMessage),
                    (CardSuccess, CardMessage),
                    (SuccessSpecialChar, MessageSpecialChar),
                    (SuccessDB, MessageDB),
                    (SuccessSc, MessageSc),
                    (SuccessImg, MessageImg)
                };

                var errors = results.Where(x => !x.Success).Select(x => x.Message).ToList();

                if (errors.Any()) return (false, string.Join("\n", errors));
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<T> LoadSetting<T>(string key, string connectionString) where T : new()
        {
            await using var connection = new SQLiteConnection(connectionString);
            await connection.OpenAsync();

            const string query = "SELECT Value FROM Settings WHERE Key = @key";

            await using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@key", key);

            var result = await command.ExecuteScalarAsync();

            if (result is not string json || string.IsNullOrWhiteSpace(json))
                return new T();

            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }
        private static async Task InsertOrReplaceSetting<T>(SQLiteConnection connection, DbTransaction transaction,
            string key, T settingObject, JsonSerializerOptions options)
        {
            string json = JsonSerializer.Serialize(settingObject, options);

            const string query = @"INSERT OR REPLACE INTO Settings 
                           (Key, Value, LastModified)
                           VALUES (@key, @value, @modified)";

            await using var command = new SQLiteCommand(query, connection);
            command.Transaction = (SQLiteTransaction)transaction;

            command.Parameters.AddWithValue("@key", key);
            command.Parameters.AddWithValue("@value", json);
            command.Parameters.AddWithValue("@modified", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
        }
    }
}
