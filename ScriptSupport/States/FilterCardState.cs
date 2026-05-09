using ScriptSupport.Models;
using ScriptSupport.ViewModels;

namespace ScriptSupport.States
{
    public class FilterCardState : BaseViewModel
    {
        private CardDataFilter _filterCardData = new CardDataFilter();
        public CardDataFilter FilterCardData
        {
            get => _filterCardData;
            set => SetProperty(ref _filterCardData, value);
        }

        private CardTextFilter _filterCardText = new CardTextFilter();
        public CardTextFilter FilterCardText
        {
            get => _filterCardText;
            set => SetProperty(ref _filterCardText, value);
        }
        private bool _link = false;
        public bool Link
        {
            get => _link;
            set => SetProperty(ref _link, value);
        }
        public FilterCardState()
        {
            _filterCardData.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName?.EndsWith("Text") == true) return;
                OnPropertyChanged(nameof(FilterCardData));
            };
            _filterCardText.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName?.EndsWith("Text") == true) return;
                OnPropertyChanged(nameof(FilterCardText));
            };
        }

        public event Action? FilterCommitted;
        public void Commit() => FilterCommitted?.Invoke();
    }
}