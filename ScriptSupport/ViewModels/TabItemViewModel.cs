namespace ScriptSupport.ViewModels
{
    public class TabItemViewModel : BaseViewModel
    {
        private string _header = "";
        private string _content = "";
        private bool _isModified;
        private string? _filePath;

        public string Header
        {
            get => _isModified ? $"● {_header}" : _header;
            set => SetProperty(ref _header, value);
        }

        public string Content
        {
            get => _content;
            set
            {
                if (SetProperty(ref _content, value))
                    IsModified = true;
            }
        }

        public bool IsModified
        {
            get => _isModified;
            set
            {
                if (SetProperty(ref _isModified, value))
                    OnPropertyChanged(nameof(Header));
            }
        }

        public string? FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public TabType TabType { get; set; } = TabType.Script;
    }

    public enum TabType { Script, CardDesc, Scrapi }
}
