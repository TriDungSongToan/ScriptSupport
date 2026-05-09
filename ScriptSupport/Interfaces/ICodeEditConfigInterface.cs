using ScriptSupport.Models.Settings;

namespace ScriptSupport.Interfaces
{
    public interface ICodeEditConfigInterface
    {
        CodeEditSetting Current { get; }
        event EventHandler? SettingChanged;
        void NotifyChanged();
    }
}
