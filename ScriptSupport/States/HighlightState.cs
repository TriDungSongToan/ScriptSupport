using ICSharpCode.AvalonEdit.Highlighting;
using ScriptSupport.ViewModels;

namespace ScriptSupport.States
{
    public class HighlightState : BaseViewModel
    {
        private IHighlightingDefinition? _current;
        public IHighlightingDefinition? Current
        {
            get => _current;
            set { _current = value; OnPropertyChanged(); }
        }
    }
}
