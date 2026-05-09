using System.Windows;
using System.Diagnostics;
using System.Collections.Specialized;
using Character.Core.Models;
using ScriptSupport.States;
using ScriptSupport.Stores;
using ScriptSupport.Commands;
using ScriptSupport.Interfaces;
using ScriptSupport.Collections;
using ScriptSupport.Localization;

namespace ScriptSupport.ViewModels
{
    [PanelTitleKey(Language.SpecialChar)]
    public class SpecialCharViewModel : BaseViewModel, IDisposable
    {
        #region Fields
        public UIConfigState UIConfig { get; }
        private readonly SpecialCharStore _store;
        private readonly ISpecialCharInterface _specialCharService;
        private readonly IStringInterface _stringService;

        private CancellationTokenSource? _searchCts;
        #endregion

        #region Properties
        public BulkObservableCollection<CharacterItem> CharacterItems { get; set; } = new();
        private CharacterItem? _selectedCharItem;
        public CharacterItem? SelectedCharItem
        {
            get => _selectedCharItem;
            set
            {
                if (SetProperty(ref _selectedCharItem, value))
                {
                    OnSelectedCharItemChanged();
                }
            }
        }
        private string _character = string.Empty;
        public string Character
        {
            get => _character;
            set
            {
                if (SetProperty(ref _character, value))
                {
                    InsertCommand?.RaiseCanExecuteChanged();
                    CopyCommand?.RaiseCanExecuteChanged();
                }
            }
        }
        private string _desc = string.Empty;
        public string Desc
        {
            get => _desc;
            set
            {
                if (SetProperty(ref _desc, value))
                {
                    SearchCommand();
                }
            }
        }
        public IEnumerable<CharacterGroup> CharacterGroups { get; }
        private CharacterGroup? _selectedGroup;
        public CharacterGroup? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (SetProperty(ref _selectedGroup, value))
                {
                    SearchCommand();
                }
            }
        }
        private string _subCategory = string.Empty;
        public string SubCategory
        {
            get => _subCategory;
            set
            {
                if (SetProperty(ref _subCategory, value))
                {
                    SearchCommand();
                }
            }
        }

        public BulkObservableCollection<TagItem> AvailableTags { get; set; } = new();
        public BulkObservableCollection<TagItem> SelectedAvailableTags { get; set; } = new();
        #endregion

        #region Commands
        public RelayCommand? InsertCommand { get; set; }
        public RelayCommand? CopyCommand { get; set; }
        public RelayCommand? ClearFilterCommand { get; set; }
        #endregion

        #region Constructor
        public SpecialCharViewModel(UIConfigState config, SpecialCharStore store,
            ISpecialCharInterface specialCharService, IStringInterface stringService)
        {
            UIConfig = config;
            _store = store;
            _specialCharService = specialCharService;
            _stringService = stringService;

            CharacterGroups = Enum.GetValues(typeof(CharacterGroup)).Cast<CharacterGroup>().ToList();
            SelectedGroup = CharacterGroup.All;
            var tagList = _store.TagItems;
            AvailableTags.AddRange(tagList);

            InitializeCommand();
            InitializeEvent();
        }

        private void InitializeCommand()
        {
            InsertCommand = new RelayCommand(_ => InsertCharacter(), _ => SelectedCharacterNotNull());
            CopyCommand = new RelayCommand(_ => CopyCharacter(), _ => SelectedCharacterNotNull());
            ClearFilterCommand = new RelayCommand(_ => ClearFilter());
        }
        private void InitializeEvent()
        {
            SelectedAvailableTags.CollectionChanged += SelectedAvailableTags_CollectionChanged;
            _store.DataChanged += OnStoreDataChanged;
        }
        #endregion

        #region Commands
        private void InsertCharacter()
        {
            if (!string.IsNullOrWhiteSpace(Character))
            {
                _stringService.InsertAtCaret(Character);
            }
        }
        private void CopyCharacter()
        {
            if (!string.IsNullOrWhiteSpace(Character))
            {
                _stringService.SetClipboard(Character);
            }
        }
        private bool SelectedCharacterNotNull()
        {
            return !string.IsNullOrEmpty(Character);
        }

        private void ClearFilter()
        {
            SelectedCharItem = null;
            Desc = string.Empty;
            SelectedGroup = CharacterGroup.All;
            SubCategory = string.Empty;
            SelectedAvailableTags.Clear();
        }
        #endregion

        #region Event
        private void OnSelectedCharItemChanged()
        {
            Character = SelectedCharItem?.Character ?? string.Empty;
        }
        private void SelectedAvailableTags_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SearchCommand();
        }
        private void OnStoreDataChanged()
        {
            Application.Current.Dispatcher.InvokeAsync(RefreshFromStore);
        }
        private void RefreshFromStore()
        {
            CharacterItems.Clear();
            CharacterItems.AddRange(_store.CharItems);

            AvailableTags.Clear();
            AvailableTags.AddRange(_store.TagItems);

            // Reset selection
            SelectedCharItem = null;
            SelectedGroup = CharacterGroup.All;
            Desc = string.Empty;
            SubCategory = string.Empty;
        }
        #endregion

        #region Filter
        public void SearchCommand()
        {
            Debug.WriteLine("SearchCommand triggered");
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            // Snapshot trên UI thread
            var filter = new CharacterFilter
            {
                SearchText = Desc,
                Group = SelectedGroup == null ? CharacterGroup.All : SelectedGroup,
                SubCategory = SubCategory,
                Tags = SelectedAvailableTags?.Select(t => t.Name).ToList() ?? new List<string>()
            };

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(200, token);
                    if (token.IsCancellationRequested) return;

                    var result = _specialCharService.Filter(filter);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        CharacterItems.Clear();
                        CharacterItems.AddRange(result);
                    });
                }
                catch (TaskCanceledException) { }
            });
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = null;
            SelectedAvailableTags.CollectionChanged -= SelectedAvailableTags_CollectionChanged;
            _store.DataChanged -= OnStoreDataChanged;
        }
        #endregion

    }
}
