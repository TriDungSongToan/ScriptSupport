using System.Windows;
using System.Windows.Media;
using ScriptSupport.ViewModels;

namespace ScriptSupport.States
{
    public class UIConfigState : BaseViewModel
    {
        private Brush _background = Brushes.White;
        public Brush Background
        {
            get => _background;
            set => SetProperty(ref _background, value);
        }
        private Brush _solidBackground = Brushes.Black;
        public Brush SolidBackground
        {
            get => _solidBackground;
            set => SetProperty(ref _solidBackground, value);
        }

        private Brush _foreground = Brushes.Black;
        public Brush Foreground
        {
            get => _foreground;
            set => SetProperty(ref _foreground, value);
        }

        private Brush _themeColor = Brushes.Purple;
        public Brush ThemeColor
        {
            get => _themeColor;
            set => SetProperty(ref _themeColor, value);
        }

        private FontFamily _fontFamily = new("Consolas");
        public FontFamily FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        private int _fontSize = 14;
        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        private FlowDirection _flowDirection = FlowDirection.LeftToRight;
        public FlowDirection FlowDirection
        {
            get => _flowDirection;
            set => SetProperty(ref _flowDirection, value);
        }

        private TextAlignment _textAlignment = TextAlignment.Left;
        public TextAlignment TextAlignment
        {
            get => _textAlignment;
            set => SetProperty(ref _textAlignment, value);
        }

        private HorizontalAlignment _hintAlignment;
        public HorizontalAlignment HintAlignment
        {
            get => _hintAlignment;
            set => SetProperty(ref _hintAlignment, value);
        }
    }
}
