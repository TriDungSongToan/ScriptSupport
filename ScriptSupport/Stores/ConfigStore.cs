using ScriptSupport.Models.Settings;

namespace ScriptSupport.Stores
{
    public class ConfigStore : IDisposable
    {
        public string SettingFilePath { get; set; } = string.Empty;
        public UserSetting UserSetting { get; set; } = new();
        public DisplaySetting DisplaySetting { get; set; } = new();
        public FilterSetting FilterSetting { get; set; } = new();
        public DataHandlingSetting DataHandlingSetting { get; set; } = new();
        public CodeEditSetting CodeEditSetting { get; set; } = new();

        public void Dispose()
        {
            UserSetting = new();
            DisplaySetting = new();
            FilterSetting = new();
            DataHandlingSetting = new();
            CodeEditSetting = new();
        }
    }
}
