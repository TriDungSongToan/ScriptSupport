using System.Windows.Controls;
using ScriptSupport.ViewModels;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for CardFilterText.xaml
    /// </summary>
    public partial class CardFilterText : UserControl
    {
        public CardFilterText(CardTextFilterViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
