using System.IO;
using System.Windows;
using System.Windows.Media;
using ScriptSupport.Stores;
using ScriptSupport.States;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;

namespace ScriptSupport.Services
{
    public class UIConfigService : IUIConfigInterface
    {
        private readonly AppEnvironment _appEnvironment;
        private readonly UIConfigState UIState;
        private readonly ConfigStore ConfigStore;
        private readonly IHighlightInterface _highlightService;

        public UIConfigService(AppEnvironment appEnvironment, UIConfigState state, ConfigStore configStore, IHighlightInterface highlightService)
        {
            _appEnvironment = appEnvironment;
            UIState = state;
            ConfigStore = configStore;
            _highlightService = highlightService;
        }

        public async Task<(bool Success, string Message)> LoadAsync()
        {
            try
            {
                var display = ConfigStore.DisplaySetting;

                var solidColor = (Color)ColorConverter.ConvertFromString(display.Background)!;
                UIState.Background = new SolidColorBrush(solidColor!);
                UIState.SolidBackground = new SolidColorBrush(Color.FromArgb(255, solidColor.R, solidColor.G, solidColor.B));
                UIState.Foreground =  new SolidColorBrush((Color)ColorConverter.ConvertFromString(display.Foreground)!);

                var themeColors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Amber", "#FFC107" },
                    { "Blue", "#2196F3" },
                    { "BlueGrey", "#607D8B" },
                    { "Brown", "#795548" },
                    { "Cyan", "#00BCD4" },
                    { "DeepOrange", "#FF5722" },
                    { "DeepPurple", "#673AB7" },
                    { "Green", "#4CAF50" },
                    { "Grey", "#9E9E9E" },
                    { "Indigo", "#3F51B5" },
                    { "LightBlue", "#03A9F4" },
                    { "LightGreen", "#8BC34A" },
                    { "Lime", "#CDDC39" },
                    { "Orange", "#FF9800" },
                    { "Pink", "#E91E63" },
                    { "Purple", "#9C27B0" },
                    { "Red", "#F44336" },
                    { "Teal", "#009688" },
                    { "Yellow", "#FFEB3B" }
                };
                string hexCode = themeColors.TryGetValue(display.Theme, out var hex) ? hex : "#673AB7"; // fallback DeepPurple
                Color color = (Color)ColorConverter.ConvertFromString(hexCode);
                UIState.ThemeColor = new SolidColorBrush(color);
                UIState.FontFamily = new FontFamily(display.FontFamily);
                UIState.FontSize = display.FontSize ?? 14;

                UIState.FlowDirection = display.FlowDirectionC == 0 ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;

                ApplyTextAlignment(display.TextAlignmentC);
                ApplyTheme(display.Theme);
                await LoadHighlighting();

                await Task.CompletedTask;
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private void ApplyTextAlignment(int alignment)
        {
            switch (alignment)
            {
                case 1:
                    UIState.TextAlignment = TextAlignment.Right;
                    UIState.HintAlignment = HorizontalAlignment.Right;
                    break;
                case 2:
                    UIState.TextAlignment = TextAlignment.Justify;
                    UIState.HintAlignment = HorizontalAlignment.Left;
                    break;
                case 3:
                    UIState.TextAlignment = TextAlignment.Center;
                    UIState.HintAlignment = HorizontalAlignment.Center;
                    break;
                default:
                    UIState.TextAlignment = TextAlignment.Left;
                    UIState.HintAlignment = HorizontalAlignment.Left;
                    break;
            }
        }

        private void ApplyTheme(string theme)
        {
            var appResources = Application.Current.Resources;

            var oldTheme = appResources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("MaterialDesignColor"));

            if (oldTheme != null) appResources.MergedDictionaries.Remove(oldTheme);

            var themeUri = new Uri($"pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.{theme}.xaml",
                UriKind.Absolute);

            appResources.MergedDictionaries.Add(new ResourceDictionary { Source = themeUri });
        }

        private async Task LoadHighlighting()
        {
            string highLightFileName = ConfigStore.DisplaySetting.HighLight;
            if (string.IsNullOrWhiteSpace(highLightFileName)) return;
            string highLightFilePath = Path.Combine(_appEnvironment.HighLightFolderPath, $"{highLightFileName}.xshd");
            if (!System.IO.File.Exists(highLightFilePath)) return;
            await _highlightService.LoadAsync(highLightFilePath);
            await _highlightService.ReloadAsync();
        }
    }
}
