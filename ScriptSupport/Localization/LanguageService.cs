using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;
using System.ComponentModel;
using ScriptSupport.Interfaces;
using ScriptSupport.Converters;

namespace ScriptSupport.Localization
{
    public class LanguageService : ILanguageInterface
    {
        private readonly Dictionary<uint, string> _translations = new();

        public event EventHandler? LanguageChanged;

        public void LoadLanguage(string languageCode)
        {
            string filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $@"data\CardData\Language\{languageCode}\AppLanguage.txt");

            _translations.Clear();

            foreach (var line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('\t', 2);
                if (parts.Length != 2) continue;

                if (parts[0].StartsWith("0x") &&
                    uint.TryParse(parts[0][2..],
                    NumberStyles.HexNumber, null, out uint code))
                {
                    _translations[code] = parts[1].Trim();
                }
            }

            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        public string GetTranslation(Language key)
        {
            return GetTranslation((uint)key);
        }

        public string GetTranslation(uint key)
        {
            return _translations.TryGetValue(key, out var v)
                ? v
                : $"[0x{key:X}]";
        }
    }
    public static class LanguageProvider
    {
        public static ILanguageInterface Current { get; set; } = null!;
    }
    public static class LanguageExtensions
    {
        public static string ToText(this Language language)
        {
            return LanguageProvider.Current.GetTranslation(language);
        }
    }
    public static class UIntExtensions
    {
        public static string ToLanguageString(this uint code)
        {
            return LanguageProvider.Current.GetTranslation(code);
        }
    }
    class LanguageBindingSource : INotifyPropertyChanged
    {
        private readonly ILanguageInterface _service;
        private readonly Language _key;

        public LanguageBindingSource(ILanguageInterface service, Language key)
        {
            _service = service;
            _key = key;

            _service.LanguageChanged += (_, _) =>
            {
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(Value)));
            };
        }

        public string Value => _service.GetTranslation(_key);

        public event PropertyChangedEventHandler? PropertyChanged;
    }
    [MarkupExtensionReturnType(typeof(object))]
    public class LangExtension : MarkupExtension
    {
        public Language Key { get; set; }
        public BindingBase[]? Args { get; set; }
        public LangExtension() { }
        public LangExtension(Language key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return Key.ToString();

            var multi = new MultiBinding
            {
                Converter = new LangMultiConverter(),
                Mode = BindingMode.OneWay
            };

            // 1. format string
            multi.Bindings.Add(new Binding($"[{Key}]")
            {
                Source = LocalizationProxy.Instance
            });

            // 2. args (dynamic)
            if (Args != null)
            {
                foreach (var arg in Args)
                    multi.Bindings.Add(arg);
            }

            return multi.ProvideValue(serviceProvider);





            //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //    return Key.ToString();

            //var binding = new Binding($"[{Key}]")
            //{
            //    Source = LocalizationProxy.Instance,
            //    Mode = BindingMode.OneWay
            //};
            //return binding.ProvideValue(serviceProvider);






            //var lang = App.Services.GetRequiredService<ILanguageInterface>();
            //var binding = new Binding(nameof(LanguageBindingSource.Value))
            //{
            //    Source = new LanguageBindingSource(lang, Key),
            //    Mode = BindingMode.OneWay
            //};
            //return binding.ProvideValue(serviceProvider);
        }
    }
}
