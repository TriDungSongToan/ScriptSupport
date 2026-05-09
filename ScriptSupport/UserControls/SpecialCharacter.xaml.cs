using System.Windows.Controls;
using ScriptSupport.ViewModels;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for SpecialCharacter.xaml
    /// </summary>
    public partial class SpecialCharacter : UserControl
    {
        public SpecialCharacter(SpecialCharViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
#if DEBUG
        public SpecialCharacter()
        {
            InitializeComponent();
        }
#endif
    }
}
