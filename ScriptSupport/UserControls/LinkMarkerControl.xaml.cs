using System.Windows.Controls;
using ScriptSupport.ViewModels;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for LinkMarkerControl.xaml
    /// </summary>
    public partial class LinkMarkerControl : UserControl
    {
        public LinkMarkerControl(LinkMarkerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
