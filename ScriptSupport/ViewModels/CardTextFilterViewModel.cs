using System.ComponentModel;
using ScriptSupport.Models;
using ScriptSupport.States;
using ScriptSupport.Stores;

namespace ScriptSupport.ViewModels
{
    public class CardTextFilterViewModel : BaseViewModel, IDisposable
    {
        #region Fields
        public UIConfigState UIConfig { get; }
        public CardElementStore CardElement { get; }
        public FilterCardState FilterState { get; }
        #endregion

        #region Properties
        private CardTextFilter? _cardTextFilterView;
        public CardTextFilter? CardTextFilterView
        {
            get => _cardTextFilterView;
            set => SetProperty(ref _cardTextFilterView, value);
        }
        private CardDataFilter? _cardDataFilterView;
        public CardDataFilter? CardDataFilterView
        {
            get => _cardDataFilterView;
            set => SetProperty(ref _cardDataFilterView, value);
        }
        #endregion

        #region Constructor
        public CardTextFilterViewModel(UIConfigState uiConfig, CardElementStore cardElement, FilterCardState filterState)
        {
            UIConfig = uiConfig;
            CardElement = cardElement;
            FilterState = filterState;

            CardTextFilterView = filterState.FilterCardText;
            CardDataFilterView = filterState.FilterCardData;
            InitializeEvent();
        }
        private void InitializeEvent()
        {
            FilterState.PropertyChanged += FilterState_PropertyChanged;
        }
        private void FilterState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ////////////
        }
        #endregion

        public void Dispose()
        {
            FilterState.PropertyChanged -= FilterState_PropertyChanged;
        }
    }
}
