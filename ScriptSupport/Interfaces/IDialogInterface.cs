using ScriptSupport.Models;
using ScriptSupport.ViewModels;

namespace ScriptSupport.Interfaces
{
    public interface IInitializable<T>
    {
        void Initialize(T parameter);
    }
    public interface IDialogInterface
    {
        #region Window
        void Show<TViewModel>() where TViewModel : BaseViewModel;
        bool? ShowDialog<TViewModel>() where TViewModel : BaseViewModel;
        void Show<TViewModel, TParam>(TParam param)
            where TViewModel : BaseViewModel, IInitializable<TParam>;
        bool? ShowDialog<TViewModel, TParam>(TParam param)
            where TViewModel : BaseViewModel, IInitializable<TParam>;
        #endregion

        #region MessageBox
        Task<int> ShowMessage(MessageBoxRequest request);
        #endregion

        #region Open Dialogs
        string OpenCardList();
        string OpenDataBase();
        string OpenDeck();
        string OpenRare();
        string OpenGenesys();
        string OpenScript();
        IEnumerable<string> OpenScripts();
        string OpenCeds();
        string OpenExcel();
        string OpenLua(string filter = "");
        string OpenConf(string filter = "");
        string OpenBanList();
        string OpenImage();
        string OpenVideo();
        string OpenText();
        string OpenFile();
        string OpenFolder(string title = "");
        #endregion

        #region Save Dialogs
        string SaveDataBase();
        string SaveDeck();
        string SaveScript();
        string SaveRes();
        string SaveText();
        string SaveCeds();
        string SaveZip();
        string SaveExcel();
        string SaveBanList();
        #endregion
    }
}
