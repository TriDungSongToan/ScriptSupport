using System.IO;
using System.Text.Json;
using System.Data.SQLite;
using System.Data.Common;

namespace ScriptSupport.Services
{
    public static class CreateFileServices
    {
        public static async Task<(bool, string)> CreateSetting(string folderPath, string fileName)
        {
            try
            {
                Directory.CreateDirectory(folderPath);

                string dbPath = Path.Combine(folderPath, fileName);
                string connectionString = $"Data Source={dbPath};Version=3;";

                await using var connection = new SQLiteConnection(connectionString);
                await connection.OpenAsync();

                const string createTable = @"CREATE TABLE IF NOT EXISTS Settings (
            Key TEXT PRIMARY KEY,
            Value TEXT NOT NULL,
            LastModified DATETIME DEFAULT CURRENT_TIMESTAMP)";

                await using (var command = new SQLiteCommand(createTable, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                const string checkQuery = "SELECT COUNT(*) FROM Settings";

                await using var checkCommand = new SQLiteCommand(checkQuery, connection);
                var result = await checkCommand.ExecuteScalarAsync();
                long count = result is long l ? l : 0;

                if (count == 0)
                {
                    await using var transaction = (SQLiteTransaction)await connection.BeginTransactionAsync();

                    try
                    {
                        await CreateDefaultSettings(connection, transaction);
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

                return (true, "Setting Database created and initialized successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public static async Task CreateDefaultSettings(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };

            await InsertSetting(connection, transaction, "CodeEditSetting", new Models.Settings.CodeEditSetting(), jsonOptions);
            await InsertSetting(connection, transaction, "DataHandlingSetting", new Models.Settings.DataHandlingSetting(), jsonOptions);
            await InsertSetting(connection, transaction, "DisplaySetting", new Models.Settings.DisplaySetting(), jsonOptions);
            await InsertSetting(connection, transaction, "UserSetting", new Models.Settings.UserSetting(), jsonOptions);
            await InsertSetting(connection, transaction, "FilterSetting", new Models.Settings.FilterSetting(), jsonOptions);
        }
        private static async Task InsertSetting<T>(SQLiteConnection connection, DbTransaction transaction,
            string key, T settingObject, JsonSerializerOptions options)
        {
            string json = JsonSerializer.Serialize(settingObject, options);

            const string insertQuery = @"INSERT INTO Settings 
                                (Key, Value, LastModified) 
                                VALUES (@key, @value, @modified)";

            await using var command = connection.CreateCommand();
            command.CommandText = insertQuery;
            command.Transaction = (SQLiteTransaction)transaction;

            command.Parameters.AddWithValue("@key", key);
            command.Parameters.AddWithValue("@value", json);
            command.Parameters.AddWithValue("@modified", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
        }
    }
}
