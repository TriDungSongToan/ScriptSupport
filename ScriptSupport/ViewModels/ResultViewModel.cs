using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Collections.Specialized;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Scrapiyard.Core.Models;
using ScriptSupport.Models;
using ScriptSupport.States;
using ScriptSupport.Commands;
using ScriptSupport.Services;
using ScriptSupport.Interfaces;
using ScriptSupport.Collections;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.ViewModels
{
    public class ResultViewModel : BaseViewModel, IDisposable
    {
        #region Fields
        public UIConfigState UIConfig { get; }
        public EditorCommandsService EditorCommands { get; }
        private readonly HighlightState _highlightState;
        public ResultState Result { get; }
        private readonly IResultInterface _resultService;
        private readonly ILauncherInterface _launcherService;
        private readonly IDialogInterface _dialogService;

        public IHighlightingDefinition? SyntaxHighlighting => _highlightState.Current;
        #endregion

        #region Property
        private TextDocument? _resultCardDocument;
        public TextDocument? ResultCardDocument
        {
            get => _resultCardDocument;
            set => SetProperty(ref _resultCardDocument, value);
        }
        private FlowDocument _resultCardDocument1 = new FlowDocument();
        public FlowDocument ResultCardDocument1
        {
            get => _resultCardDocument1;
            set => SetProperty(ref _resultCardDocument1, value);
        }

        private TextDocument? _resultScrapiyardDocument;
        public TextDocument? ResultScrapiyardDocument
        {
            get => _resultScrapiyardDocument;
            set => SetProperty(ref _resultScrapiyardDocument, value);
        }

        private BulkObservableCollection<CardText>? _cardTexts;
        public BulkObservableCollection<CardText>? CardTexts
        {
            get => _cardTexts;
            set
            {
                if (SetProperty(ref _cardTexts, value))
                {
                    if (_cardTexts != null)
                    {
                        _cardTexts.CollectionChanged -= CardTexts_CollectionChanged;
                        _cardTexts.CollectionChanged += CardTexts_CollectionChanged;
                    }
                    OnCardTextChanged();
                }
            }
        }
        private CardText? _selectedCardText;
        public CardText? SelectedCardText
        {
            get => _selectedCardText;
            set
            {
                if (SetProperty(ref _selectedCardText, value))
                {
                    OnSelectedCardTextChanged();
                }
            }
        }

        private CompletionSymbol? _resultScrapiyard = new CompletionSymbol();
        public CompletionSymbol? ResultScrapiyard
        {
            get => _resultScrapiyard;
            set
            {
                if (SetProperty(ref _resultScrapiyard, value))
                {
                    OnResultScrapiyardChanged();
                }
            }
        }

        private int _activateTab;
        public int ActivateTab
        {
            get => _activateTab;
            set => SetProperty(ref _activateTab, value);
        }
        #endregion

        #region Commands
        public ICommand? LinkClickedCommand { get; set; }
        #endregion

        #region Constructor
        public ResultViewModel(UIConfigState uiConfig, EditorCommandsService editorCommands, ResultState result,
            IResultInterface resultService, ILauncherInterface launcherService,
            IDialogInterface dialogService, HighlightState highlightState)
        {
            UIConfig = uiConfig;
            EditorCommands = editorCommands;
            Result = result;
            _resultService = resultService;
            _launcherService = launcherService;
            _dialogService = dialogService;
            _highlightState = highlightState;

            LinkClickedCommand = new RelayCommand<string>(OnLinkClicked);

            ResultCardDocument = result.ResultCardDocument ??= new TextDocument();
            ResultScrapiyardDocument = result.ResultScrapiyardDocument ??= new TextDocument();
            CardTexts = result.ResultCardTexts ??= new BulkObservableCollection<CardText>();

            InitializeEvent();
        }
        private void InitializeEvent()
        {
            Result.PropertyChanged += Result_PropertyChanged;
            if (ResultCardDocument != null)
                ResultCardDocument.TextChanged += ResultCardDocument_TextChanged;
            if (ResultScrapiyardDocument != null)
                ResultScrapiyardDocument.TextChanged += ResultScrapiDocument_TextChanged;
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
        #endregion

        #region Event
        private void Result_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ResultState.ResultCardDocument))
            {
                if (ResultCardDocument != null) ResultCardDocument.TextChanged -= ResultCardDocument_TextChanged;
                ResultCardDocument = Result.ResultCardDocument ??= new TextDocument();
                ResultCardDocument.TextChanged += ResultCardDocument_TextChanged;
                ActivateTab = 0;
            }
            if (e.PropertyName == nameof(ResultState.ResultCardDocument1))
            {
                ResultCardDocument1 = Result.ResultCardDocument1 ?? new FlowDocument();
                ActivateTab = 0;
            }
            else if (e.PropertyName == nameof(ResultState.ResultScrapiyardDocument))
            {
                if (ResultScrapiyardDocument != null) ResultScrapiyardDocument.TextChanged -= ResultScrapiDocument_TextChanged;
                ResultScrapiyardDocument = Result.ResultScrapiyardDocument ??= new TextDocument();
                ResultScrapiyardDocument.TextChanged += ResultScrapiDocument_TextChanged;
                ActivateTab = 1;
            }
            else if (e.PropertyName == nameof(ResultState.ResultCardTexts))
            {
                CardTexts = Result.ResultCardTexts;
            }
            else if (e.PropertyName == nameof(ResultState.ResultScrapiyard))
            {
                ResultScrapiyard = Result.ResultScrapiyard;
            }
        }
        private void ResultCardDocument_TextChanged(object? sender, EventArgs e)
        {
            ActivateTab = 0;
        }
        private void ResultScrapiDocument_TextChanged(object? sender, EventArgs e)
        {
            ActivateTab = 1;
        }
        private void CardTexts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnCardTextChanged();
        }
        private void OnCardTextChanged()
        {
            if (CardTexts == null || !CardTexts.Any())
            {
                SelectedCardText = null;
                return;
            }
            var first = CardTexts.FirstOrDefault();

            if (SelectedCardText == null || !CardTexts.Contains(SelectedCardText))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SelectedCardText = first;
                }), System.Windows.Threading.DispatcherPriority.Loaded);
                
            }
        }
        private void OnSelectedCardTextChanged()
        {
            _resultService.BuildResultCardDocument(SelectedCardText);
        }
        private void OnResultScrapiyardChanged()
        {
            _resultService.BuildResultScrapiyardDocument(ResultScrapiyard);
        }
        #endregion

        public void Dispose()
        {
            if (ResultCardDocument != null) ResultCardDocument.TextChanged -= ResultCardDocument_TextChanged;
            if (ResultScrapiyardDocument != null) ResultScrapiyardDocument.TextChanged -= ResultScrapiDocument_TextChanged;
            if (Result != null)  Result.PropertyChanged -= Result_PropertyChanged;
            if (_cardTexts != null) _cardTexts.CollectionChanged -= CardTexts_CollectionChanged;
        }
    }
}
