using System.Windows;
using System.Windows.Controls;
using ScriptSupport.Generator;
using ScriptSupport.ViewModels;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for ScriptDescription.xaml
    /// </summary>
    public partial class ScriptDescription : UserControl
    {
        public ScriptDescription()
        {
            InitializeComponent();
            DataContextChanged += ScriptDescription_DataContextChanged;
        }

        private void ScriptDescription_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ScriptDescViewModel vm)
            {
                var generator = new HyperlinkElement
                {
                    OnLinkClicked = url => vm.LinkClickedCommand?.Execute(url)
                };
                ScriptDescriptionPane.TextArea.TextView.ElementGenerators.Add(generator);
            }
        }
    }
}
