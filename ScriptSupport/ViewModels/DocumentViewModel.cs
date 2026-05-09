using System.IO;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ScriptSupport.Models;
using ScriptSupport.States;
using ScriptSupport.Manager;
using ScriptSupport.Theming;
using ScriptSupport.Services;
using ScriptSupport.Commands;
using ScriptSupport.Interfaces;
using ScriptSupport.Collections;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.ViewModels
{
    public class DocumentViewModel : BaseViewModel, IDisposable
    {
        #region Fields
        public UIConfigState UIConfig { get; }
        public EditorCommandsService EditorCommands { get; }
        private readonly DocumentManager _manager;
        private readonly HighlightState _highlightState;
        private readonly IItemsSourceInterface _itemsSourceService;
        public IEditorServiceFactory EditorServiceFactory { get; }
        private bool _isLoading;
        #endregion

        #region Properties
        public TextDocument Document { get; }
        public string? FilePath { get; set; }

        public IHighlightingDefinition? SyntaxHighlighting => _highlightState.Current;
        private LineHighlightRenderer? _highlightRenderer;

        private BulkObservableCollection<int> _highlightLines = new();
        public BulkObservableCollection<int> HighlightLines
        {
            get => _highlightLines;
            set
            {
                _highlightLines = value ?? new BulkObservableCollection<int>();
                _highlightRenderer?.SetLines(_highlightLines);
            }
        }

        private int _caretOffset;
        public int CaretOffset
        {
            get => _caretOffset;
            set
            {
                SetProperty(ref _caretOffset, value);
                System.Diagnostics.Debug.WriteLine($"[CaretOffset] = {value}");
            }
        }
        #endregion

        #region Header
        private string _title = "";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }
        private bool _isPreview;
        public bool IsPreview
        {
            get => _isPreview;
            set => SetProperty(ref _isPreview, value);
        }
        #endregion

        #region Foodter
        public IReadOnlyList<string> FontSizeList { get; }
        public IReadOnlyList<CmbItems> IndentOptions { get; }
        public IReadOnlyList<CmbItems> NewLineOptions { get; }


        private double _fontSize = 12;
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }
        private int _caretLine = 1;
        public int CaretLine
        {
            get => _caretLine;
            set => SetProperty(ref _caretLine, value);
        }
        private int _caretColumn = 1;
        public int CaretColumn
        {
            get => _caretColumn;
            set => SetProperty(ref _caretColumn, value);
        }


        private double _horizontalOffset;
        public double HorizontalOffset
        {
            get => _horizontalOffset;
            set => SetProperty(ref _horizontalOffset, value);
        }
        private double _scrollableWidth;
        public double ScrollableWidth
        {
            get => _scrollableWidth;
            set => SetProperty(ref _scrollableWidth, value);
        }
        private double _viewportWidth;
        public double ViewportWidth
        {
            get => _viewportWidth;
            set => SetProperty(ref _viewportWidth, value);
        }


        private bool _isOverstrikeMode;
        public bool IsOverstrikeMode
        {
            get => _isOverstrikeMode;
            set => SetProperty(ref _isOverstrikeMode, value);
        }


        private CmbItems _selectedIndentOption;
        public CmbItems SelectedIndentOption
        {
            get => _selectedIndentOption;
            set
            {
                if (SetProperty(ref _selectedIndentOption, value))
                    ApplyIndentOption(value);
            }
        }
        private CmbItems _selectedNewLineOption;
        public CmbItems SelectedNewLineOption
        {
            get => _selectedNewLineOption;
            set
            {
                if (SetProperty(ref _selectedNewLineOption, value))
                    ApplyNewLineOption(value);
            }
        }


        private IndentOption _useSpaces = IndentOption.Spaces;
        public IndentOption UseSpaces
        {
            get => _useSpaces;
            set => SetProperty(ref _useSpaces, value);
        }
        private LineEnding _lineEnding = LineEnding.CRLF;
        public LineEnding LineEnding
        {
            get => _lineEnding;
            set => SetProperty(ref _lineEnding, value);
        }
        #endregion

        #region Commands
        private ICommand? _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new RelayCommand(_ =>
        {
            _ = _manager.CloseDocument(this);
        });
        #endregion

        #region Contructor
        public DocumentViewModel(DocumentManager manager, UIConfigState uiConfig, EditorCommandsService editorCommands,
            HighlightState highlightState, IItemsSourceInterface itemsSourceService,
            IEditorServiceFactory editorServiceFactory)
        {
            _itemsSourceService = itemsSourceService;
            EditorServiceFactory = editorServiceFactory;

            FontSizeList = _itemsSourceService.FontSizeList;
            IndentOptions = _itemsSourceService.IndentOptions;
            NewLineOptions = _itemsSourceService.NewLineOptions;

            UIConfig = uiConfig;
            EditorCommands = editorCommands;
            _manager = manager;
            _highlightState = highlightState;
            Document = new TextDocument();
            _isLoading = true;

            FontSize = UIConfig.FontSize;
            _selectedIndentOption = IndentOptions[0];
            _selectedNewLineOption = NewLineOptions[0];

            Document.TextChanged += Document_TextChanged;
            _highlightState.PropertyChanged += HighlightState_PropertyChanged;
        }
        #endregion

        #region Methods
        private void ApplyIndentOption(CmbItems item)
        {
            UseSpaces = item.ShortName switch
            {
                "SPC" => IndentOption.Spaces,
                "TAB" => IndentOption.Tabs,
                _ => IndentOption.Spaces
            };
        }
        private void ApplyNewLineOption(CmbItems item)
        {
            LineEnding = item.ShortName switch
            {
                "CRLF" => LineEnding.CRLF,
                "LF" => LineEnding.LF,
                "CR" => LineEnding.CR,
                _ => LineEnding.CRLF
            };
        }
        public void LoadText(string text)
        {
            _isLoading = true;
            Document.Text = text;
            _isLoading = false;
        }
        public void OpenFile(string path)
        {
            FilePath = path;
            Title = Path.GetFileName(path);
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path)) return;
            LoadText(File.ReadAllText(path));
        }
        public void OpenEmpty(string title)
        {
            Title = title;
        }
        public void OpenFileEmpty()
        {
            FilePath = string.Empty;
            Title = CMess.luaFileNotFou.ToText();
            _isLoading = false;
            Document.Text = string.Empty;
            _isLoading = true;
        }
        private void Document_TextChanged(object? sender, EventArgs e)
        {
            if (_isLoading) return;
            if (!_isDirty)
            {
                IsDirty = true;
                _manager.PromotePreview(this);
            }
        }
        private void HighlightState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HighlightState.Current))
                OnPropertyChanged(nameof(SyntaxHighlighting));
        }
        public void AttachRenderer(LineHighlightRenderer renderer)
        {
            _highlightRenderer = renderer;
        }

        public void InsertText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                Document.Insert(_caretOffset, text);
            });
        }
        #endregion

        public void Dispose()
        {
            Document.TextChanged -= Document_TextChanged;
            _highlightState.PropertyChanged -= HighlightState_PropertyChanged;
        }
    }
}
