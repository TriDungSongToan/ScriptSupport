using System.IO;
using System.Data.SQLite;
using ScriptSupport.Stores;
using ScriptSupport.Helpers;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.Services
{
    public class KonamiIDService : IKonamiIDInterface
    {
        private readonly AppEnvironment _appEnvironment;
        private readonly KonamiIDStore _konamiIDStore;

        public KonamiIDService(AppEnvironment appEnvironment, KonamiIDStore konamiIDStore)
        {
            _appEnvironment = appEnvironment;
            _konamiIDStore = konamiIDStore;
        }

        #region Load
        public async Task<(bool Success, string Message)> LoadKonamiIDAsync()
        {
            var (official, rush, error) = await LoadKonamiID();
            if (official == null || rush == null) return (false, error);

            if (official != null) _konamiIDStore.SetOfficialData(official);
            if (rush != null) _konamiIDStore.SetRushData(rush);
            if (!string.IsNullOrWhiteSpace(error)) await File.AppendAllTextAsync(_appEnvironment.ErrorLogFilePath, error + System.Environment.NewLine);


            return (true, string.Empty);
        }
        public async Task<(Dictionary<ulong, int>?, Dictionary<string, int>?, string)> LoadKonamiID()
        {
            string dbFilePath = _appEnvironment.KonamiIDFilePath;
            if (!System.IO.File.Exists(dbFilePath)) return (null, null, $"{CMess.fileNotExit.ToText()} GetKonamiID.cdb");

            try
            {
                Dictionary<ulong, int> _officialMapID = new();
                Dictionary<string, int> _rushMapID = new();

                using (var connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT konami_id, password FROM dataOfficial";
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int konamiID = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0));
                                ulong password = reader.IsDBNull(1) ? 0UL : unchecked((ulong)Convert.ToUInt64(reader.GetValue(1)));
                                _officialMapID[password] = konamiID;
                            }
                        }
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT konami_id, name FROM dataRush";
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int konamiID = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0));
                                string name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                _rushMapID[name] = konamiID;
                            }
                        }
                    }
                }

                return (_officialMapID, _rushMapID, string.Empty);
            }
            catch (Exception ex)
            {
                return (null, null, ex.Message);
            }
        }
        #endregion

        #region Get Data
        public int? GetOfficialKonamiID(ulong password)
        {
            if (_konamiIDStore.OfficialCardIDs.TryGetValue(password, out int konamiID))
            {
                return konamiID;
            }
            return null;
        }
        public int? GetRushKonamiID(string name)
        {
            if (name.EndsWith(" (Rush)")) name = name.Substring(0, name.Length - " (Rush)".Length);
            if (_konamiIDStore.RushCardIDs.TryGetValue(name, out int konamiID))
            {
                return konamiID;
            }
            return null;
        }
        #endregion

        #region Build Url
        public (bool Success, string Message) BuildKonamiDBUrl(ulong id, string name)
        {
            int? konamiID = null;
            string URL = string.Empty;

            if (id > 600000000) return (false, CMess.konamiIDnotFou.ToText());
            else if (id < 100000000)
            {
                konamiID = GetOfficialKonamiID(id);
                if (konamiID != null) URL = $"https://www.db.yugioh-card.com/yugiohdb/card_search.action?ope=2&request_locale=ja&cid={konamiID}";
            }
            else if (id >= 160000000 && id < 300000000)
            {
                konamiID = GetRushKonamiID(name);
                if (konamiID != null) URL = $"https://www.db.yugioh-card.com/rushdb/card_search.action?ope=2&request_locale=ja&cid={konamiID}";
            }
            else return (false, CMess.konamiIDnotFou.ToText());

            return (true, URL);
        }
        public (bool Success, string Message) BuildYuGiPediaUrl(ulong id, string name)
        {
            if (id >= 600000000 || string.IsNullOrWhiteSpace(name)) return (false, CMess.yugiPedianotFou.ToText());

            string processed = NameReplaceHelper.ProcessName(name, id);

            if (string.IsNullOrWhiteSpace(processed)) return (false, CMess.yugiPedianotFou.ToText());

            string url = $"https://yugipedia.com/wiki/{processed}";
            return (true, url);
        }
        public (bool Success, string Message) BuildYGOResourcesUrl(ulong id, string name)
        {
            int? konamiID;

            if (id >= 600000000) return (false, CMess.konamiIDnotFou.ToText());
            else if (id < 100000000) konamiID = GetOfficialKonamiID(id);
            else if (id >= 160000000 && id < 300000000) konamiID = GetRushKonamiID(name);
            else return (false, CMess.konamiIDnotFou.ToText());

            if (konamiID.HasValue)
            {
                string URL = $"https://db.ygoresources.com/card#{konamiID}";
                return (true, URL);
            }
            else return (false, CMess.konamiIDnotFou.ToText());
        }
        #endregion
    }
}
