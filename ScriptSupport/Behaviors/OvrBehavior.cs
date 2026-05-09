using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using ICSharpCode.AvalonEdit;

namespace ScriptSupport.Behaviors
{
    public class OvrBehavior : Behavior<TextEditor>
    {
        #region IsOverstrikeMode
        public static readonly DependencyProperty IsOverstrikeModeProperty =
            DependencyProperty.Register(
                nameof(IsOverstrikeMode), typeof(bool), typeof(OvrBehavior),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnIsOverstrikeModeChanged)); // VM → View

        public bool IsOverstrikeMode
        {
            get => (bool)GetValue(IsOverstrikeModeProperty);
            set => SetValue(IsOverstrikeModeProperty, value);
        }
        #endregion

        #region Attach / Detach
        protected override void OnAttached()
        {
            base.OnAttached();
            // View → VM: detect khi user nhấn Insert
            AssociatedObject.TextArea.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextArea.PreviewKeyDown -= OnPreviewKeyDown;
        }
        #endregion

        #region Handlers
        // View → VM: user nhấn Insert key
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Insert) return;
            IsOverstrikeMode = !IsOverstrikeMode;
            // không set e.Handled — để AvalonEdit tự xử lý toggle OverstrikeMode nội bộ
        }

        // VM → View: set OverstrikeMode trực tiếp vào TextArea
        private static void OnIsOverstrikeModeChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not OvrBehavior behavior) return;
            if (behavior.AssociatedObject is null) return;

            behavior.AssociatedObject.TextArea.OverstrikeMode = (bool)e.NewValue;
        }
        #endregion
    }
}
