using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using ICSharpCode.AvalonEdit;

namespace ScriptSupport.Behaviors
{
    public class ScrollSyncBehavior : Behavior<TextEditor>
    {
        #region Properties
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                nameof(HorizontalOffset), typeof(double), typeof(ScrollSyncBehavior),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnHorizontalOffsetChanged)); // VM → View

        public double HorizontalOffset
        {
            get => (double)GetValue(HorizontalOffsetProperty);
            set => SetValue(HorizontalOffsetProperty, value);
        }

        public static readonly DependencyProperty ScrollableWidthProperty =
            DependencyProperty.Register(
                nameof(ScrollableWidth), typeof(double), typeof(ScrollSyncBehavior),
                new FrameworkPropertyMetadata(0.0));

        public double ScrollableWidth
        {
            get => (double)GetValue(ScrollableWidthProperty);
            set => SetValue(ScrollableWidthProperty, value);
        }

        public static readonly DependencyProperty ViewportWidthProperty =
            DependencyProperty.Register(
                nameof(ViewportWidth), typeof(double), typeof(ScrollSyncBehavior),
                new FrameworkPropertyMetadata(0.0));

        public double ViewportWidth
        {
            get => (double)GetValue(ViewportWidthProperty);
            set => SetValue(ViewportWidthProperty, value);
        }
        #endregion

        #region Internal state
        private ScrollViewer? _scrollViewer;
        private bool _isUpdating; // anti-loop guard
        #endregion

        #region Attach / Detach
        protected override void OnAttached()
        {
            base.OnAttached();

            // ScrollViewer chưa có trong Visual Tree lúc Attach
            if (AssociatedObject.IsLoaded)
                Initialize();
            else
                AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnLoaded;

            if (_scrollViewer != null)
                _scrollViewer.ScrollChanged -= OnScrollChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= OnLoaded;
            Initialize();
        }

        private void Initialize()
        {
            _scrollViewer = FindScrollViewer(AssociatedObject);
            if (_scrollViewer is null) return;

            _scrollViewer.ScrollChanged += OnScrollChanged;

            // Sync giá trị ban đầu
            SyncFromScrollViewer();
        }
        #endregion

        #region View → VM
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Chỉ xử lý khi horizontal thay đổi
            if (e.HorizontalChange == 0 && e.ExtentWidthChange == 0) return;
            if (_isUpdating) return;

            SyncFromScrollViewer();
        }

        private void SyncFromScrollViewer()
        {
            if (_scrollViewer is null) return;

            _isUpdating = true;
            HorizontalOffset = _scrollViewer.HorizontalOffset;
            ScrollableWidth = _scrollViewer.ScrollableWidth;
            ViewportWidth = _scrollViewer.ViewportWidth;
            _isUpdating = false;
        }
        #endregion

        #region VM → View
        private static void OnHorizontalOffsetChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not ScrollSyncBehavior behavior) return;
            if (behavior._isUpdating) return;
            if (behavior._scrollViewer is null) return;

            behavior._scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }
        #endregion

        #region Helper
        private static ScrollViewer? FindScrollViewer(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is ScrollViewer sv) return sv;
                var result = FindScrollViewer(child);
                if (result is not null) return result;
            }
            return null;
        }
        #endregion
    }
}
