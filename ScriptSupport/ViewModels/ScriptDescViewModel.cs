using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ScriptSupport.Models;
using ScriptSupport.States;
using ScriptSupport.Services;
using ScriptSupport.Commands;
using ScriptSupport.Interfaces;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.ViewModels
{
    public class ScriptDescViewModel : BaseViewModel, IDisposable
    {
        public UIConfigState UIConfig { get; }
        public EditorCommandsService EditorCommands { get; }
        private readonly HighlightState _highlightState;
        private readonly ILauncherInterface _launcherService;
        private readonly IDialogInterface _dialogService;
        public IHighlightingDefinition? SyntaxHighlighting => _highlightState.Current;

        private TextDocument? _document;
        public TextDocument? Document
        {
            get => _document;
            set => SetProperty(ref _document, value);
        }

        public ICommand? LinkClickedCommand { get; set; }

        public ScriptDescViewModel(UIConfigState uiConfig, EditorCommandsService editorCommands,
            HighlightState highlightState, ILauncherInterface launcherService, IDialogInterface dialogService)
        {
            UIConfig = uiConfig;
            EditorCommands = editorCommands;
            _highlightState = highlightState;
            _launcherService = launcherService;
            _dialogService = dialogService;

            LinkClickedCommand = new RelayCommand<string>(OnLinkClicked);
        }
        private void OnLinkClicked(string? rawUrl)
        {
            if (string.IsNullOrWhiteSpace(rawUrl)) return;
            var (success, errorMessage) = _launcherService.OpenLink(rawUrl);
            if (!success)
            {
                var request = new MessageBoxRequest
                {
                    Title = CMess.error.ToText(),
                    IconType = MessageBoxIconType.Error,
                    Message = errorMessage,
                    Buttons = new[] { CMess.ok.ToText() },
                    DefaultButtonIndex = 0,
                    ResponseSource = null
                };
                _dialogService.ShowMessage(request);
            }
        }
        public void SetDocument(TextDocument document)
        {
            Document = document;
        }
        public void Clear()
        {
            Document = null;
        }
        public void Dispose()
        {
            Document = null;
        }
    }
}
