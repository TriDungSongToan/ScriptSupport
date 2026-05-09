using System.Windows.Controls;
using ScriptSupport.ViewModels;

namespace ScriptSupport.Interfaces
{
    public interface IViewLocator
    {
        UserControl GetView(Type viewModelType);
        UserControl GetView<TViewModel>() where TViewModel : BaseViewModel;
    }
}
