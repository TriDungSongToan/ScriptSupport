using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using ScriptSupport.Generator;
using ScriptSupport.ViewModels;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for ResultView.xaml
    /// </summary>
    public partial class ResultView : UserControl
    {
        public ResultView(ResultViewModel vm)
        {
            InitializeComponent();
            DataContextChanged += ResultView_DataContextChanged;
            DataContext = vm;
        }

        private void ResultView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ResultViewModel vm)
            {
                var brush = vm.UIConfig.ThemeColor as SolidColorBrush;
                if (brush != null)
                {
                    Color colorScrapiyard = brush.Color;
                    colorScrapiyard.A = 150;
                    Color colorCard = brush.Color;
                    colorCard.A = 180;

                    EditorScrapiyard.TextArea.SelectionBrush = new SolidColorBrush(colorScrapiyard);
                    EditorScrapiyard.TextArea.Caret.CaretBrush = vm.UIConfig.ThemeColor;

                    EditorCard.TextArea.SelectionBrush = new SolidColorBrush(colorScrapiyard);
                    EditorCard.TextArea.Caret.CaretBrush = vm.UIConfig.ThemeColor;

                    rtxtCard.SelectionBrush = new SolidColorBrush(colorCard);
                    rtxtCard.CaretBrush = vm.UIConfig.ThemeColor;
                }

                var generator = new HyperlinkElement
                {
                    OnLinkClicked = url => vm.LinkClickedCommand?.Execute(url)
                };
                EditorScrapiyard.TextArea.TextView.ElementGenerators.Add(generator);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ResultViewModel vm)
            {
                vm.Dispose();
            }
        }
    }
}
