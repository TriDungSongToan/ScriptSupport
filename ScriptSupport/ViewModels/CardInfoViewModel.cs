using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using System.ComponentModel;
using ScriptSupport.Models;
using ScriptSupport.States;
using ScriptSupport.Stores;
using ScriptSupport.Commands;
using ScriptSupport.Interfaces;
using ScriptSupport.Collections;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.ViewModels
{
    public class CardInfoViewModel : BaseViewModel, IDisposable
    {
        #region Fields
        public UIConfigState UIConfig { get; }
        private CardElementStore CardElement { get; }
        private ResultState Result { get; }
        private readonly IImageAppInterface _imageAppService;
        private readonly IImageCardInterface _imageCardService;
        private readonly IDialogInterface _dialogService;
        private readonly IKonamiIDInterface _konamiIDService;
        private readonly ILauncherInterface _launcherService;
        #endregion

        #region Properties
        private List<ulong>? _cardId;
        public List<ulong>? CardId
        {
            get => _cardId;
            set => SetProperty(ref _cardId, value);
        }
        private CardInformation _cardInfo;
        public CardInformation CardInfo
        {
            get => _cardInfo;
            set
            {
                if (SetProperty(ref _cardInfo, value))
                {
                    RefreshCommandWeb();
                }
            }
        }

        private BulkObservableCollection<CardText>? _cardTexts = new();
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
                    NextCardTextCommand?.RaiseCanExecuteChanged();
                    PrevCardTextCommand?.RaiseCanExecuteChanged();
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
                    OnSelectedCardTextChanged(SelectedCardText);
                    NextCardTextCommand?.RaiseCanExecuteChanged();
                    PrevCardTextCommand?.RaiseCanExecuteChanged();
                    CardDBCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        private CardData? _cardData;
        public CardData? CardData
        {
            get => _cardData;
            set
            {
                if (SetProperty(ref _cardData, value))
                {
                    _ = OnCardDataChanged(CardData);
                }
            }
        }

        private string _selectedCardName = "Card Name";
        public string SelectedCardName
        {
            get => _selectedCardName;
            set => SetProperty(ref _selectedCardName, value);
        }

        private BulkObservableCollection<FileItem>? _imageCards = new();
        public BulkObservableCollection<FileItem>? ImageCards
        {
            get => _imageCards;
            set
            {
                if (SetProperty(ref _imageCards, value))
                {
                    if (_imageCards != null)
                    {
                        _imageCards.CollectionChanged -= ImageCards_CollectionChanged;
                        _imageCards.CollectionChanged += ImageCards_CollectionChanged;
                    }
                }
            }
        }
        private FileItem? _selectedImage;
        public FileItem? SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (SetProperty(ref _selectedImage, value))
                {
                    _ = LoadSelectedImageSource(_selectedImage?.FullPath);
                }
            }
        }
        private ImageSource? _selectedImageSource;
        public ImageSource? SelectedImageSource
        {
            get => _selectedImageSource;
            private set => SetProperty(ref _selectedImageSource, value);
        }
        private ImageSource? _imageLevel;
        public ImageSource? ImageLevel
        {
            get => _imageLevel;
            private set => SetProperty(ref _imageLevel, value);
        }

        private CardItemInfo _currentInfo;
        public CardItemInfo CurrentInfo
        {
            get => _currentInfo;
            set => SetProperty(ref _currentInfo, value);
        }
        #endregion

        #region Commands
        public RelayCommand? NextCardTextCommand { get; private set; }
        public RelayCommand? PrevCardTextCommand { get; private set; }

        public RelayCommand? CardDBCommand { get; set; }
        public RelayCommand? KonamiDBCommand { get; set; }
        public RelayCommand? YuGiPediaCommand { get; set; }
        public RelayCommand? YGOResourcesCommand { get; set; }
        #endregion

        #region Constructor
        public CardInfoViewModel(UIConfigState uiConfig, CardElementStore cardElement, ResultState result,
            IImageAppInterface imageAppService, IImageCardInterface imageCardService,
            IDialogInterface dialogService,
            IKonamiIDInterface konamiIDService, ILauncherInterface launcherService)
        {
            UIConfig = uiConfig;
            CardElement = cardElement;
            Result = result;
            _cardInfo = new CardInformation(CardElement);
            CardInfo = new CardInformation(CardElement);
            _imageAppService = imageAppService;
            _imageCardService = imageCardService;
            _dialogService = dialogService;
            _konamiIDService = konamiIDService;
            _launcherService = launcherService;

            InitializeCommand();
            InitializeEvent();
            
        }
        private void InitializeCommand()
        {
            NextCardTextCommand = new RelayCommand(_ => NextCardText(), _ => CanNextCardText());
            PrevCardTextCommand = new RelayCommand(_ => PrevCardText(), _ => CanPrevCardText());

            CardDBCommand = new RelayCommand(_ => CardDataBase(), _ => CanLaunchFile());
            KonamiDBCommand = new RelayCommand(async _ => await KonamiDatabase(), _ => CanLaunchWeb());
            YuGiPediaCommand = new RelayCommand(async _ => await YuGiPedia(), _ => CanLaunchWeb());
            YGOResourcesCommand = new RelayCommand(async _ => await YGOResources(), _ => CanLaunchWeb());
        }
        private void InitializeEvent()
        {
            Result.PropertyChanged += Result_PropertyChanged;
            CardInfo.PropertyChanged += CardInfo_PropertyChanged;
        }
        #endregion

        #region Event Handlers
        private void Result_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ResultState.ResultID):
                    CardId = Result.ResultID;
                    break;
                case nameof(ResultState.ResultCardTexts):
                    CardTexts = Result.ResultCardTexts;
                    OnCardTextChanged();
                    break;
                case nameof(ResultState.ResultImageCards):
                    ImageCards = Result.ResultImageCards;
                    OnImageCardsChanged();
                    break;
                case nameof(ResultState.ResultCardData):
                    CardData = Result.ResultCardData;
                    break;
            }
        }
        private void CardInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshCommandFile();
            RefreshCommandWeb();
        }
        private void CardTexts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnCardTextChanged();
            NextCardTextCommand?.RaiseCanExecuteChanged();
            PrevCardTextCommand?.RaiseCanExecuteChanged();
        }
        private void ImageCards_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnImageCardsChanged();
        }
        #endregion

        #region Command
        private void NextCardText()
        {
            if (_cardTexts == null || _selectedCardText == null) return;
            var index = _cardTexts.IndexOf(_selectedCardText);
            if (index >= 0 && index < _cardTexts.Count - 1)
            {
                SelectedCardText = _cardTexts[index + 1];
            }
        }
        private bool CanNextCardText()
        {
            if (_cardTexts == null || _selectedCardText == null) return false;
            var index = _cardTexts.IndexOf(_selectedCardText);
            return index >= 0 && index < _cardTexts.Count - 1;
        }
        private void PrevCardText()
        {
            if (_cardTexts == null || _selectedCardText == null) return;
            var index = _cardTexts.IndexOf(_selectedCardText);
            if (index > 0)
            {
                SelectedCardText = _cardTexts[index - 1];
            }
        }
        private bool CanPrevCardText()
        {
            if (_cardTexts == null || _selectedCardText == null) return false;
            var index = _cardTexts.IndexOf(_selectedCardText);
            return index > 0;
        }

        private void CardDataBase()
        {
            if (SelectedCardText == null) return;
            if (string.IsNullOrWhiteSpace(SelectedCardText.DBPath) || !File.Exists(SelectedCardText.DBPath)) return;

            var (Result, ErrorMessage) = _launcherService.OpenFileOrFolder(SelectedCardText.DBPath);
            if (!Result)
            {
                var request = new MessageBoxRequest
                {
                    Title = CMess.error.ToText(),
                    IconType = MessageBoxIconType.Error,
                    Message = $"{CMess.errorOcc.ToText()} {ErrorMessage}",
                    Buttons = new[] { CMess.ok.ToText() },
                    ResponseSource = null
                };
                _dialogService.ShowMessage(request);
            }
        }
        private async Task KonamiDatabase()
        {
            if (CardInfo == null || string.IsNullOrWhiteSpace(CardInfo.CardName)) return;
            if (CardData == null || CardData.id <= 0) return;

            var URL = _konamiIDService.BuildKonamiDBUrl(CardData.id, CardInfo.CardName);
            if (URL.Success)
            {
                var (Success, ErrorMessage) = _launcherService.OpenWeb(URL.Message);
                if (!Success)
                {
                    var request = new MessageBoxRequest
                    {
                        Title = CMess.error.ToText(),
                        IconType = MessageBoxIconType.Error,
                        Message = $"{CMess.errorOcc.ToText()} {ErrorMessage}",
                        Buttons = new[] { CMess.ok.ToText() },
                        ResponseSource = null
                    };
                    await _dialogService.ShowMessage(request);
                }
            }
            else
            {
                var request = new MessageBoxRequest
                {
                    Title = CMess.error.ToText(),
                    IconType = MessageBoxIconType.Error,
                    Message = $"{CMess.errorOcc.ToText()} {URL.Message}",
                    Buttons = new[] { CMess.ok.ToText() },
                    ResponseSource = null
                };
                await _dialogService.ShowMessage(request);
            }
        }
        private async Task YuGiPedia()
        {
            if (CardInfo == null || string.IsNullOrWhiteSpace(CardInfo.CardName)) return;
            if (CardData == null || CardData.id <= 0) return;

            var URL = _konamiIDService.BuildYuGiPediaUrl(CardData.id, CardInfo.CardName);
            if (URL.Success)
            {
                var (Success, ErrorMessage) = _launcherService.OpenWeb(URL.Message);
                if (!Success)
                {
                    var request = new MessageBoxRequest
                    {
                        Title = CMess.error.ToText(),
                        IconType = MessageBoxIconType.Error,
                        Message = $"{CMess.errorOcc.ToText()} {ErrorMessage}",
                        Buttons = new[] { CMess.ok.ToText() },
                        ResponseSource = null
                    };
                    await _dialogService.ShowMessage(request);
                }
            }
            else
            {
                var request = new MessageBoxRequest
                {
                    Title = CMess.error.ToText(),
                    IconType = MessageBoxIconType.Error,
                    Message = $"{CMess.errorOcc.ToText()} {URL.Message}",
                    Buttons = new[] { CMess.ok.ToText() },
                    ResponseSource = null
                };
                await _dialogService.ShowMessage(request);
            }
        }
        private async Task YGOResources()
        {
            if (CardInfo == null || string.IsNullOrWhiteSpace(CardInfo.CardName)) return;
            if (CardData == null || CardData.id <= 0) return;

            var URL = _konamiIDService.BuildYGOResourcesUrl(CardData.id, CardInfo.CardName);
            if (URL.Success)
            {
                var (Success, ErrorMessage) = _launcherService.OpenWeb(URL.Message);
                if (!Success)
                {
                    var request = new MessageBoxRequest
                    {
                        Title = CMess.error.ToText(),
                        IconType = MessageBoxIconType.Error,
                        Message = $"{CMess.errorOcc.ToText()} {ErrorMessage}",
                        Buttons = new[] { CMess.ok.ToText() },
                        ResponseSource = null
                    };
                    await _dialogService.ShowMessage(request);
                }
            }
            else
            {
                var request = new MessageBoxRequest
                {
                    Title = CMess.error.ToText(),
                    IconType = MessageBoxIconType.Error,
                    Message = $"{CMess.errorOcc.ToText()} {URL.Message}",
                    Buttons = new[] { CMess.ok.ToText() },
                    ResponseSource = null
                };
                await _dialogService.ShowMessage(request);
            }
        }

        private void RefreshCommandWeb()
        {
            KonamiDBCommand?.RaiseCanExecuteChanged();
            YuGiPediaCommand?.RaiseCanExecuteChanged();
            YGOResourcesCommand?.RaiseCanExecuteChanged();
        }
        private bool CanLaunchWeb()
        {
            return (!string.IsNullOrWhiteSpace(CardInfo.CardName) && CardData != null && CardData.id > 0);
        }
        private void RefreshCommandFile()
        {
            CardDBCommand?.RaiseCanExecuteChanged();
        }
        private bool CanLaunchFile()
        {
            return (SelectedCardText != null &&
                !string.IsNullOrWhiteSpace(SelectedCardText.DBPath) &&
                System.IO.File.Exists(SelectedCardText.DBPath));
        }
        #endregion

        #region Changed
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
        private void OnImageCardsChanged()
        {
            if (ImageCards == null || !ImageCards.Any())
            {
                SelectedImage = null;
                return;
            }

            var first = ImageCards.FirstOrDefault();

            if (SelectedImage == null || !ImageCards.Contains(SelectedImage))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SelectedImage = first;
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }
        private void OnSelectedCardTextChanged(CardText? cardText)
        {
            CardInfo.SetViewText(SelectedCardText);
            SelectedCardName = SelectedCardText?.name ?? "Card Name";
        }
        private async Task OnCardDataChanged(CardData? CardData)
        {
            CurrentInfo = new CardItemInfo(CardData?.type ?? 0);
            CardInfo.SetViewData(CardData);

            if (CardData == null) ImageLevel = null;
            else
            {
                if (CurrentInfo.IsXyz && CurrentInfo.IsNonXyz) ImageLevel = await _imageAppService.Get(AppImage.LevelRankStar);
                else if (CurrentInfo.IsXyz && !CurrentInfo.IsNonXyz) ImageLevel = await _imageAppService.Get(AppImage.RankStar);
                else ImageLevel = await _imageAppService.Get(AppImage.LevelStar);
            }
            RefreshCommandWeb();
        }
        #endregion

        private async Task LoadSelectedImageSource(string? path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    SelectedImageSource = bitmap;
                    return;
                }
                catch { }
            }
            SelectedImageSource = await _imageAppService.Get(AppImage.Blank);
        }

        public void Dispose()
        {
            
            if (_cardTexts != null) _cardTexts.CollectionChanged -= CardTexts_CollectionChanged;
            if (_imageCards != null) _imageCards.CollectionChanged -= ImageCards_CollectionChanged;
            Result.PropertyChanged -= Result_PropertyChanged;
            CardInfo.PropertyChanged -= CardInfo_PropertyChanged;

            NextCardTextCommand = null;
            PrevCardTextCommand = null;

            CardDBCommand = null;
            KonamiDBCommand = null;
            YuGiPediaCommand = null;
            YGOResourcesCommand = null;
        }
    }
}
