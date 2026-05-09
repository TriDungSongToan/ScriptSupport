using ScriptSupport.Models.Settings;
using ScriptSupport.Stores;
using ScriptSupport.Interfaces;

namespace ScriptSupport.Services
{
    public class CodeEditConfigService : ICodeEditConfigInterface
    {
        private readonly ConfigStore _store;

        public CodeEditConfigService(ConfigStore store)
        {
            _store = store;
        }

        public CodeEditSetting Current => _store.CodeEditSetting;
        public event EventHandler? SettingChanged;

        public void NotifyChanged() => SettingChanged?.Invoke(this, EventArgs.Empty);
    }
}
