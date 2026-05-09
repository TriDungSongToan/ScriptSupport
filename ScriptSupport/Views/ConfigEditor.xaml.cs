using System.Windows;
using System.Windows.Input;
using ScriptSupport.ViewModels;

namespace ScriptSupport.Views
{
    /// <summary>
    /// Interaction logic for ConfigEditor.xaml
    /// </summary>
    public partial class ConfigEditor : Window
    {
        public ConfigEditor()
        {
            InitializeComponent();
        }

        private void blsetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //if (DataContext is IDisposable disposable)
            //{
            //    disposable.Dispose();
            //}
        }

        private void btncacels_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
