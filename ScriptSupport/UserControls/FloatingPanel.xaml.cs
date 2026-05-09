using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using ScriptSupport.ViewModels;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for FloatingPanel.xaml
    /// </summary>
    public partial class FloatingPanel : UserControl
    {
        private bool _isDragging;
        private Point _dragStartOffset;

        public FloatingPanel(FloatingPanelViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void DragHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _dragStartOffset = e.GetPosition(this);
            DragHandle.CaptureMouse();
        }

        private void DragHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;
            if (Parent is not Canvas canvas) return;

            var pos = e.GetPosition(canvas);
            double left = Math.Clamp(pos.X - _dragStartOffset.X, 0, canvas.ActualWidth - ActualWidth);
            double top = Math.Clamp(pos.Y - _dragStartOffset.Y, 0, canvas.ActualHeight - ActualHeight);

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
        }

        private void DragHandle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            DragHandle.ReleaseMouseCapture();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
