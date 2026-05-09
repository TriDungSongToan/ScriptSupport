using ScriptSupport.States;
using ScriptSupport.Commands;

namespace ScriptSupport.ViewModels
{
    public class FloatingPanelViewModel : BaseViewModel
    {
        public UIConfigState UIConfig { get; }
        public object? PanelContent { get; set; }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public RelayCommand CloseCommand { get; }
        public Action? RequestClose { get; set; }

        public FloatingPanelViewModel(UIConfigState config)
        {
            UIConfig = config;
            CloseCommand = new RelayCommand(() => RequestClose?.Invoke());
        }
    }
}
