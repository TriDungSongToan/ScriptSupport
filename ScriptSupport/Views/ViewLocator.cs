using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ScriptSupport.Interfaces;
using ScriptSupport.ViewModels;
using ScriptSupport.UserControls;

namespace ScriptSupport.Views
{
    public class ViewLocator : IViewLocator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, Type> _mappings = new();

        public ViewLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            RegisterMappings();
        }

        private void RegisterMappings()
        {
            //_mappings[typeof(MainViewModel)] = typeof(MainWindowEmbedded);
            //_mappings[typeof(MainViewModel)] = typeof(MainWindowStandalone);
            _mappings[typeof(MainViewModel)] = typeof(MainUserControl);

            _mappings[typeof(LinkMarkerViewModel)] = typeof(LinkMarkerControl);
            _mappings[typeof(CardTextFilterViewModel)] = typeof(CardFilterText);
            _mappings[typeof(CardDataFilterViewModel)] = typeof(CardFilterData);
            _mappings[typeof(CardInfoViewModel)] = typeof(CardInformation);
            _mappings[typeof(CardFilterViewModel)] = typeof(CardFilter);
            _mappings[typeof(ResultViewModel)] = typeof(ResultView);
        }

        public UserControl GetView<TViewModel>() where TViewModel : BaseViewModel
        => GetView(typeof(TViewModel));

        public UserControl GetView(Type viewModelType)
        {
            if (!_mappings.TryGetValue(viewModelType, out var viewType))
                throw new InvalidOperationException($"No view registered for {viewModelType.Name}");

            return (UserControl)_serviceProvider.GetRequiredService(viewType);
        }
    }
}
