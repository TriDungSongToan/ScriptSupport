using System.Windows.Controls;
using ScriptSupport.ViewModels;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for CardInformation.xaml
    /// </summary>
    public partial class CardInformation : UserControl
    {
        public CardInformation(CardInfoViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
