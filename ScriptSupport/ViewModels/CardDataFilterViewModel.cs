using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Specialized;
using ScriptSupport.Collections;
using ScriptSupport.Interfaces;
using ScriptSupport.Models;
using ScriptSupport.States;
using ScriptSupport.Stores;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.ViewModels
{
    public class CardDataFilterViewModel : BaseViewModel, IDisposable
    {
        #region Fields
        public UIConfigState UIConfig { get; }
        public CardElementStore CardElement { get; }
        public FilterCardState FilterState { get; }
        public LinkMarkerViewModel LinkMarkerVM { get; set; }

        private readonly IImageAppInterface _imageCache;
        #endregion

        #region Propertys
        private CardDataFilter? _cardDataFilterView;
        public CardDataFilter? CardDataFilterView
        {
            get => _cardDataFilterView;
            set => SetProperty(ref _cardDataFilterView, value);
        }

        private CardItemInfo CurrentInfo;

        #region Binding View
        private bool _isPendulum = false;
        public bool IsPendulum
        {
            get => _isPendulum;
            set => SetProperty(ref _isPendulum, value);
        }
        private bool _isLink = false;
        public bool IsLink
        {
            get => _isLink;
            set
            {
                if (SetProperty(ref _isLink, value))
                    FilterState.Link = value;
            }
        }
        private bool _isSkill = false;
        public bool IsSkill
        {
            get => _isSkill;
            set => SetProperty(ref _isSkill, value);
        }
        #endregion

        private string _lvRkLabel = string.Empty;
        public string LvRkLabel
        {
            get => _lvRkLabel;
            set => SetProperty(ref _lvRkLabel, value);
        }
        private ImageSource? _lvRkImg;
        public ImageSource? LvRkImg
        {
            get => _lvRkImg;
            set => SetProperty(ref _lvRkImg, value);
        }

        public BulkObservableCollection<RuleItem> SelectedRuleItems { get; set; } = new();
        public BulkObservableCollection<TypeItem> SelectedTypeItems { get; set; } = new();
        public BulkObservableCollection<RaceItem> SelectedRaceItems { get; set; } = new();
        public BulkObservableCollection<CharItem> SelectedCharItems { get; set; } = new();
        public BulkObservableCollection<AttributeItem> SelectedAttributeItems { get; set; } = new();
        public BulkObservableCollection<SetCodeItem> SelectedSetCodeItems { get; set; } = new();
        public BulkObservableCollection<CategoryItem> SelectedCategoryItems { get; set; } = new();
        public BulkObservableCollection<FlagItem> SelectedFlagItems { get; set; } = new();
        #endregion

        #region Constructor
        public CardDataFilterViewModel(UIConfigState uiConfig, CardElementStore cardStore,
            FilterCardState filterState, LinkMarkerViewModel linkMarkerViewModel,
            IImageAppInterface ImageCache)
        {
            UIConfig = uiConfig;
            CardElement = cardStore;
            FilterState = filterState;
            LinkMarkerVM = linkMarkerViewModel;
            _imageCache = ImageCache;
            CardDataFilterView = filterState.FilterCardData;
            InitializeEvent();
            _ = InitializeIcon();
        }

        private void InitializeEvent()
        {
            SelectedRuleItems.CollectionChanged += SelectedRuleItems_CollectionChanged;
            SelectedTypeItems.CollectionChanged += SelectedTypeItems_CollectionChanged;
            SelectedRaceItems.CollectionChanged += SelectedRaceItems_CollectionChanged;
            SelectedCharItems.CollectionChanged += SelectedCharItems_CollectionChanged;
            SelectedAttributeItems.CollectionChanged += SelectedAttributeItems_CollectionChanged;
            SelectedSetCodeItems.CollectionChanged += SelectedSetCodeItems_CollectionChanged;
            SelectedCategoryItems.CollectionChanged += SelectedCategoryItems_CollectionChanged;
            SelectedFlagItems.CollectionChanged += SelectedFlagItems_CollectionChanged;

            LinkMarkerVM.PropertyChanged += CurrentLinkMarkerVM_PropertyChanged;
            FilterState.PropertyChanged += FilterState_PropertyChanged;
        }
        private async Task InitializeIcon()
        {
            try
            {
                LvRkLabel = ResolveLvRkLabel(false, false);
                LvRkImg = await ResolveIcon(false, false);
            }
            catch { }
        }
        #endregion

        #region Event
        private void SelectedRuleItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CardDataFilterView == null) return;
            OnPropertyChanged(nameof(SelectedRuleItems));
            CardDataFilterView.ot = Combine(SelectedRuleItems, x => x.RuleCode);
        }
        private void SelectedTypeItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CardDataFilterView == null) return;
            OnPropertyChanged(nameof(SelectedTypeItems));
            CardDataFilterView.Type = Combine(SelectedTypeItems, x => x.TypeCode);
            CurrentInfo = new CardItemInfo(CardDataFilterView.Type);
            _ = UpdateUI();
            CardDataFilterView.Race = CurrentInfo.IsSkill
                ? CardDataFilterView.Race = Combine(SelectedCharItems, x => x.CharCode)
                : CardDataFilterView.Race = Combine(SelectedRaceItems, x => x.RaceCode);
        }
        private void SelectedRaceItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CardDataFilterView == null) return;
            if (CurrentInfo.IsSkill) return;
            OnPropertyChanged(nameof(SelectedRaceItems));
            CardDataFilterView.Race = Combine(SelectedRaceItems, x => x.RaceCode);
        }
        private void SelectedCharItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CardDataFilterView == null) return;
            if (!CurrentInfo.IsSkill) return;
            OnPropertyChanged(nameof(SelectedCharItems));
            CardDataFilterView.Race = Combine(SelectedCharItems, x => x.CharCode);
        }
        private void SelectedAttributeItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CardDataFilterView == null) return;
            OnPropertyChanged(nameof(SelectedAttributeItems));
            CardDataFilterView.Attribute = Combine(SelectedAttributeItems, x => x.AttributeCode);
        }
        private void SelectedSetCodeItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CardDataFilterView == null) return;
            OnPropertyChanged(nameof(SelectedSetCodeItems));
            CardDataFilterView.SetCode = Combine(SelectedSetCodeItems, x => x.SetCode);
        }
        private void SelectedCategoryItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CardDataFilterView == null) return;
            OnPropertyChanged(nameof(SelectedCategoryItems));
            CardDataFilterView.Category = Combine(SelectedCategoryItems, x => x.CategoryCode);
        }
        private void SelectedFlagItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CardDataFilterView == null) return;
            OnPropertyChanged(nameof(SelectedFlagItems));
            CardDataFilterView.Flag = Combine(SelectedFlagItems, x => x.FlagCode);
        }
        private void CurrentLinkMarkerVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (CardDataFilterView == null) return;
            if (e.PropertyName == nameof(LinkMarkerViewModel.LinkMakers))
            {
                CardDataFilterView.LinkMaker = LinkMarkerVM.LinkMakers;
            }
        }
        private void FilterState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ////////////
        }
        #endregion

        private async Task UpdateUI()
        {
            IsPendulum = CurrentInfo.IsPendulum;
            IsLink = CurrentInfo.IsLink;
            IsSkill = CurrentInfo.IsSkill;
            LvRkImg = await ResolveIcon(CurrentInfo.IsXyz, CurrentInfo.IsNonXyz);
            LvRkLabel = ResolveLvRkLabel(CurrentInfo.IsXyz, CurrentInfo.IsNonXyz);
        }
        private async Task<ImageSource?> ResolveIcon(bool IsXyz, bool IsNonXyz)
        {
            if (IsXyz && IsNonXyz) return await _imageCache.Get(AppImage.LevelRankStar);
            else if (IsXyz && !IsNonXyz) return await _imageCache.Get(AppImage.RankStar);
            else return await _imageCache.Get(AppImage.LevelStar);
        }
        private string ResolveLvRkLabel(bool IsXyz, bool IsNonXyz)
        {
            if (IsXyz && IsNonXyz) return $"{CMess.Level.ToText()}/{CMess.Rank.ToText()}";
            else if (IsXyz && !IsNonXyz) return CMess.Rank.ToText();
            else return CMess.Level.ToText();
        }
        private ulong Combine<T>(IEnumerable<T> items, Func<T, ulong> selector)
        {
            ulong result = 0;
            if (items == null) return result;
            foreach (var item in items)
            {
                result |= selector(item);
            }
            return result;
        }

        public void Dispose()
        {
            SelectedRuleItems.CollectionChanged -= SelectedRuleItems_CollectionChanged;
            SelectedTypeItems.CollectionChanged -= SelectedTypeItems_CollectionChanged;
            SelectedRaceItems.CollectionChanged -= SelectedRaceItems_CollectionChanged;
            SelectedCharItems.CollectionChanged -= SelectedCharItems_CollectionChanged;
            SelectedAttributeItems.CollectionChanged -= SelectedAttributeItems_CollectionChanged;
            SelectedSetCodeItems.CollectionChanged -= SelectedSetCodeItems_CollectionChanged;
            SelectedCategoryItems.CollectionChanged -= SelectedCategoryItems_CollectionChanged;
            SelectedFlagItems.CollectionChanged -= SelectedFlagItems_CollectionChanged;

            FilterState.PropertyChanged -= FilterState_PropertyChanged;

        }
    }
}
