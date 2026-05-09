using ScriptSupport.Localization;

namespace ScriptSupport.Interfaces
{
    public interface ILanguageInterface
    {
        event EventHandler? LanguageChanged;
        void LoadLanguage(string languageCode);
        string GetTranslation(Language key);
        string GetTranslation(uint key);
    }
}
