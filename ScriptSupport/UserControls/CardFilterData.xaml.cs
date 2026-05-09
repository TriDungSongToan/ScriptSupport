using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using ScriptSupport.ViewModels;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for CardFilterData.xaml
    /// </summary>
    public partial class CardFilterData : UserControl
    {
        public CardFilterData(CardDataFilterViewModel vm, LinkMarkerControl linkControl)
        {
            InitializeComponent();
            DataContext = vm;
            LinkMarkerContent.Content = linkControl;
        }

        private void MultiSelectComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not Sdl.MultiSelectComboBox.Themes.Generic.MultiSelectComboBox multiSelectComboBox) return;
            if (multiSelectComboBox.IsDropDownOpen == true) return;
            // Lấy ScrollViewer bên trong dropdown
            var scrollViewer = FindVisualChild<ScrollViewer>(multiSelectComboBox);
            if (scrollViewer == null) return;

            bool scrollAtTop = scrollViewer.VerticalOffset == 0 && e.Delta > 0;
            bool scrollAtBottom = scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight && e.Delta < 0;

            if (scrollAtTop || scrollAtBottom)
            {
                // Propagate event lên ScrollViewer cha
                var parentScrollViewer = FindParent<ScrollViewer>(multiSelectComboBox);
                if (parentScrollViewer != null)
                {
                    parentScrollViewer.ScrollToVerticalOffset(parentScrollViewer.VerticalOffset - e.Delta);
                    e.Handled = true;
                }
            }
        }
        public static T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t) return t;
                T? childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }
        public static T? FindParent<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null && !(parent is T))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as T;
        }

    }
}
