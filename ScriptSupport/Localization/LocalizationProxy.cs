using System.Windows;
using System.ComponentModel;
using ScriptSupport.Interfaces;

namespace ScriptSupport.Localization
{
    public class LocalizationProxy : INotifyPropertyChanged
    {
        private static readonly LocalizationProxy _designInstance = new();
        public static LocalizationProxy Instance { get; private set; } = _designInstance;

        private readonly ILanguageInterface? _language;

        public LocalizationProxy(ILanguageInterface language)
        {
            _language = language;
            _language.LanguageChanged += OnLanguageChanged;
            Instance = this;
        }
        public LocalizationProxy()
        {
            _language = null;
        }
        private static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());
        public string this[Language key] => IsInDesignMode || _language is null ? $"[{key}]" : _language.GetTranslation(key);

        public string this[uint key] => IsInDesignMode || _language is null ? $"[0x{key:X}]" : _language.GetTranslation(key);

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs("Item[]")); // refresh all
        }
    }
}
