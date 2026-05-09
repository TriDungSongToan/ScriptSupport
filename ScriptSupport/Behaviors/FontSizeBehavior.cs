using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using ICSharpCode.AvalonEdit;

namespace ScriptSupport.Behaviors
{
    public class FontSizeBehavior : Behavior<TextEditor>
    {
        #region FontSize
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
                nameof(FontSize), typeof(double), typeof(FontSizeBehavior),
                new FrameworkPropertyMetadata(12.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        #endregion

        #region MinFontSize / MaxFontSize
        public static readonly DependencyProperty MinFontSizeProperty =
            DependencyProperty.Register(
                nameof(MinFontSize), typeof(double), typeof(FontSizeBehavior),
                new PropertyMetadata(6.0));

        public double MinFontSize
        {
            get => (double)GetValue(MinFontSizeProperty);
            set => SetValue(MinFontSizeProperty, value);
        }

        public static readonly DependencyProperty MaxFontSizeProperty =
            DependencyProperty.Register(
                nameof(MaxFontSize), typeof(double), typeof(FontSizeBehavior),
                new PropertyMetadata(72.0));

        public double MaxFontSize
        {
            get => (double)GetValue(MaxFontSizeProperty);
            set => SetValue(MaxFontSizeProperty, value);
        }
        #endregion

        #region Attach / Detach
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        }
        #endregion

        #region Handler
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) &&
                !Keyboard.IsKeyDown(Key.RightCtrl)) return;

            double next = FontSize + (e.Delta > 0 ? 1 : -1);
            FontSize = Math.Clamp(next, MinFontSize, MaxFontSize);

            e.Handled = true; // chặn TextEditor scroll khi Ctrl+Scroll
        }
        #endregion
    }
}
