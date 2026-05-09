using System.Windows;
using ICSharpCode.AvalonEdit;
using Microsoft.Xaml.Behaviors;

namespace ScriptSupport.Behaviors
{
    public class CaretBehavior : Behavior<TextEditor>
    {
        #region CaretLine
        public static readonly DependencyProperty CaretLineProperty =
            DependencyProperty.Register(
                nameof(CaretLine), typeof(int), typeof(CaretBehavior),
                new FrameworkPropertyMetadata(1,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int CaretLine
        {
            get => (int)GetValue(CaretLineProperty);
            set => SetValue(CaretLineProperty, value);
        }
        #endregion

        #region CaretColumn
        public static readonly DependencyProperty CaretColumnProperty =
            DependencyProperty.Register(
                nameof(CaretColumn), typeof(int), typeof(CaretBehavior),
                new FrameworkPropertyMetadata(1,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int CaretColumn
        {
            get => (int)GetValue(CaretColumnProperty);
            set => SetValue(CaretColumnProperty, value);
        }
        #endregion

        #region CaretOffset
        public static readonly DependencyProperty CaretOffsetProperty =
            DependencyProperty.Register(
                nameof(CaretOffset), typeof(int), typeof(CaretBehavior),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public int CaretOffset
        {
            get => (int)GetValue(CaretOffsetProperty);
            set => SetValue(CaretOffsetProperty, value);
        }
        #endregion

        #region Attach / Detach
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextArea.Caret.PositionChanged += OnPositionChanged;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextArea.Caret.PositionChanged -= OnPositionChanged;
        }
        #endregion

        #region Handler
        private void OnPositionChanged(object? sender, EventArgs e)
        {
            var caret = AssociatedObject.TextArea.Caret;
            CaretLine = caret.Line;
            CaretColumn = caret.Column;
            CaretOffset = caret.Offset;
        }
        #endregion
    }
}
