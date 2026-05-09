using ScriptSupport.States;

namespace ScriptSupport.ViewModels
{
    public class CardFilterViewModel : BaseViewModel, IDisposable
    {
        #region Fields
        public UIConfigState UIConfig { get; }

        public CardTextFilterViewModel TextFilterVM { get; set; }
        public CardDataFilterViewModel DataFilterVM { get; set; }
        public CardInfoViewModel CardInfoVM { get; set; }
        #endregion

        #region Constructor
        public CardFilterViewModel(UIConfigState uiConfig, 
            CardTextFilterViewModel textFilterVM, CardDataFilterViewModel dataFilterVM, CardInfoViewModel infoVM)
        {
            UIConfig = uiConfig;
            TextFilterVM = textFilterVM;
            DataFilterVM = dataFilterVM;
            CardInfoVM = infoVM;
        }
        #endregion

        public void Dispose()
        {

        }
    }
}
