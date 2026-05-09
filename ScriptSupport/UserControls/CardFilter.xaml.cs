using System.Windows.Controls;
using ScriptSupport.ViewModels;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for CardFilter.xaml
    /// </summary>
    public partial class CardFilter : UserControl
    {
        public CardFilter(CardFilterViewModel cardFilterVM,
            CardFilterText filterText, CardFilterData filterData, CardInformation info)
        {
            InitializeComponent();
            DataContext = cardFilterVM;
            CardTextContent.Content = filterText;
            CardDataContent.Content = filterData;
            CardInfoContent.Content = info;
        }

    }
}
