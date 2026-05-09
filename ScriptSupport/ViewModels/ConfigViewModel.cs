using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScriptSupport.Environment;
using ScriptSupport.Services;

namespace ScriptSupport.ViewModels
{
    public class ConfigViewModel // : INotifyPropertyChanged, IDisposable
    {
        //private static readonly Lazy<ConfigViewModel> _instance = new Lazy<ConfigViewModel>(() => new ConfigViewModel());
        //public static ConfigViewModel Instance => _instance.Value;
        //private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        //public string SettingFilePath { get; set; }

        //private ScriptSupport.Models.Settings.UserSetting _userSetting = new();
        //public ScriptSupport.Models.Settings.UserSetting userSetting
        //{
        //    get => _userSetting;
        //    set
        //    {
        //        if (_userSetting != value)
        //        {
        //            _userSetting = value;
        //            OnPropertyChanged(nameof(userSetting));
        //        }
        //    }
        //}

        //private ScriptSupport.Models.Settings.DisplaySetting _displaySetting = new();
        //public ScriptSupport.Models.Settings.DisplaySetting displaySetting
        //{
        //    get => _displaySetting;
        //    set
        //    {
        //        if (_displaySetting != value)
        //        {
        //            _displaySetting = value;
        //            OnPropertyChanged(nameof(displaySetting));
        //        }
        //    }
        //}

        //private ScriptSupport.Models.Settings.DataHandlingSetting _dataHandlingSetting = new();
        //public ScriptSupport.Models.Settings.DataHandlingSetting dataHandlingSetting
        //{
        //    get => _dataHandlingSetting;
        //    set
        //    {
        //        if (_dataHandlingSetting != value)
        //        {
        //            _dataHandlingSetting = value;
        //            OnPropertyChanged(nameof(dataHandlingSetting));
        //        }
        //    }
        //}

        //private ScriptSupport.Models.Settings.CodeEditSetting _codeEditSetting = new();
        //public ScriptSupport.Models.Settings.CodeEditSetting codeEditSetting
        //{
        //    get => _codeEditSetting;
        //    set
        //    {
        //        if (_codeEditSetting != value)
        //        {
        //            _codeEditSetting = value;
        //            OnPropertyChanged(nameof(codeEditSetting));
        //        }
        //    }
        //}

        //public  ConfigViewModel()
        //{
            //SettingFilePath = Path.Combine(App.Services.GetRequiredService<AppEnvironment>().ConfigFolderPath, "AppSetting.db");
            //userSetting = new Models.Settings.UserSetting();
            //displaySetting = new Models.Settings.DisplaySetting();
            //dataHandlingSetting = new Models.Settings.DataHandlingSetting();
            //codeEditSetting = new Models.Settings.CodeEditSetting();
        //}

        /// <summary>
        /// Setting File
        /// </summary>
        //public (bool, string) LoadSettingFIle()
        //{
        //    if (string.IsNullOrEmpty(SettingFilePath) || !System.IO.File.Exists(SettingFilePath))
        //    {
        //        var (resultCreate, messageCreate) = CreateFileServices.CreateSetting(AppEnvironment.Instance.ConfigFolderPath, "AppSetting.db");
        //        if (!resultCreate) return (false, messageCreate);
        //    }

        //    string connectionString = $"Data Source={SettingFilePath};Version=3;";

        //    try
        //    {
        //        userSetting = LoadSetting<ScriptSupport.Models.Settings.UserSetting>("UserSetting", connectionString);
        //        displaySetting = LoadSetting<ScriptSupport.Models.Settings.DisplaySetting>("DisplaySetting", connectionString);
        //        dataHandlingSetting = LoadSetting<ScriptSupport.Models.Settings.DataHandlingSetting>("DataHandlingSetting", connectionString);
        //        codeEditSetting = LoadSetting<ScriptSupport.Models.Settings.CodeEditSetting>("CodeEditSetting", connectionString);

        //        return (true, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}
        //public T LoadSetting<T>(string key, string _connectionString) where T : new()
        //{
        //    using var connection = new SQLiteConnection(_connectionString);
        //    connection.Open();

        //    string query = "SELECT Value FROM Settings WHERE Key = @key";

        //    using var command = new SQLiteCommand(query, connection);
        //    command.Parameters.AddWithValue("@key", key);

        //    var result = command.ExecuteScalar();
        //    if (result is not string json || string.IsNullOrWhiteSpace(json)) return new T();
        //    if (result == null) return new T(); // Trả về instance mới nếu chưa có

        //    return JsonSerializer.Deserialize<T>(json) ?? new T();
        //}

        //public (bool, string) SaveSingleSettingFile<T>(string key, T settingObject)
        //{
        //    if (string.IsNullOrEmpty(SettingFilePath) || !System.IO.File.Exists(SettingFilePath))
        //    {
        //        var (resultCreate, messageCreate) = CreateFileServices.CreateSetting(AppEnvironment.Instance.ConfigFolderPath, "AppSetting.db");
        //        if (!resultCreate) return (false, messageCreate);
        //    }

        //    string connectionString = $"Data Source={SettingFilePath};Version=3;";

        //    var jsonOptions = new JsonSerializerOptions{ WriteIndented = false };

        //    try
        //    {
        //        using var connection = new SQLiteConnection(connectionString);
        //        connection.Open();

        //        string query = @"INSERT OR REPLACE INTO Settings
        //             (Key, Value, LastModified)
        //             VALUES (@key, @value, @modified)";

        //        using var command = new SQLiteCommand(query, connection);
        //        command.Parameters.AddWithValue("@key", key);
        //        command.Parameters.AddWithValue("@value", JsonSerializer.Serialize(settingObject, jsonOptions));
        //        command.Parameters.AddWithValue("@modified", DateTime.Now);

        //        command.ExecuteNonQuery();
        //        return (true, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}
        //public (bool, string) SaveAllSettingFile()
        //{
        //    if (string.IsNullOrEmpty(SettingFilePath) || !System.IO.File.Exists(SettingFilePath))
        //    {
        //        var (resultCreate, messageCreate) = CreateFileServices.CreateSetting(AppEnvironment.Instance.ConfigFolderPath, "AppSetting.db");
        //        if (!resultCreate) return (false, messageCreate);
        //    }

        //    string connectionString = $"Data Source={SettingFilePath};Version=3;";
        //    var jsonOptions = new JsonSerializerOptions{ WriteIndented = false };

        //    using var connection = new SQLiteConnection(connectionString);
        //    connection.Open();
        //    using var transaction = connection.BeginTransaction();
        //    try
        //    {
        //        InsertOrReplaceSetting(connection, "UserSetting", userSetting, jsonOptions);
        //        InsertOrReplaceSetting(connection, "DisplaySetting", displaySetting, jsonOptions);
        //        InsertOrReplaceSetting(connection, "DataHandlingSetting", dataHandlingSetting, jsonOptions);
        //        InsertOrReplaceSetting(connection, "CodeEditSetting", codeEditSetting, jsonOptions);

        //        transaction.Commit();
        //        return (true, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();
        //        return (false, ex.Message);
        //    }
        //}
        //private static void InsertOrReplaceSetting<T>(SQLiteConnection connection, string key, T settingObject, JsonSerializerOptions options)
        //{
        //    string json = JsonSerializer.Serialize(settingObject, options);

        //    string query = @"INSERT OR REPLACE INTO Settings 
        //             (Key, Value, LastModified)
        //             VALUES (@key, @value, @modified)";

        //    using var command = new SQLiteCommand(query, connection);
        //    command.Parameters.AddWithValue("@key", key);
        //    command.Parameters.AddWithValue("@value", json);
        //    command.Parameters.AddWithValue("@modified", DateTime.Now);

        //    command.ExecuteNonQuery();
        //}

        //public async Task<(bool, string)> ResetSettingFile()
        //{
        //    if (string.IsNullOrEmpty(SettingFilePath) || !System.IO.File.Exists(SettingFilePath))
        //    {
        //        var (resultCreate, messageCreate) = await CreateFileServices.CreateSetting(AppEnvironment.Instance.ConfigFolderPath, "AppSetting.db");
        //        if (!resultCreate) return (false, messageCreate);
        //    }

        //    string connectionString = $"Data Source={SettingFilePath};Version=3;";
        //    using var connection = new SQLiteConnection(connectionString);
        //    connection.Open();
        //    using var transaction = connection.BeginTransaction();

        //    try
        //    {
        //        using (var deleteCmd = new SQLiteCommand("DELETE FROM Settings", connection))
        //            deleteCmd.ExecuteNonQuery();

        //        await CreateFileServices.CreateDefaultSettings(connection);
        //        transaction.Commit();
        //        return (true, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();
        //        return (false, ex.Message);
        //    }
        //}

        //public void Dispose()
        //{
        //    _semaphore?.Dispose();
        //}

        //public event PropertyChangedEventHandler? PropertyChanged;
        //protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}

