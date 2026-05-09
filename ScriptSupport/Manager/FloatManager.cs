using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using ScriptSupport.States;
using ScriptSupport.Factorys;
using ScriptSupport.ViewModels;
using ScriptSupport.UserControls;
using ScriptSupport.Localization;

namespace ScriptSupport.Manager
{
    public class FloatManager
    {
        public UIConfigState UIConfig { get; }
        private readonly Canvas _canvas;
        private readonly PanelFactory _factory;

        private readonly Dictionary<Type, FloatingPanel> _openedPanels = new();
        public FloatManager(UIConfigState _uIConfig, Canvas canvas, PanelFactory factory)
        {
            UIConfig = _uIConfig;
            _canvas = canvas;
            _factory = factory;
        }

        public void Show(Type vmType)
        {
            if (_openedPanels.TryGetValue(vmType, out var existing))
            {
                existing.Visibility = Visibility.Visible;
                return;
            }

            var content = _factory.Create(vmType);

            var vm = new FloatingPanelViewModel(UIConfig)
            {
                Title = GetTitle(vmType),
                PanelContent = content
            };

            var panel = new FloatingPanel(vm)
            {

            };

            vm.RequestClose = () =>
            {
                _openedPanels.Remove(vmType);
                _canvas.Children.Remove(panel);
            };

            _canvas.Children.Add(panel);
            _openedPanels[vmType] = panel;
        }
        private string GetTitle(Type vmType)
        {
            var attr = vmType.GetCustomAttribute<PanelTitleKeyAttribute>();

            return attr != null ? attr.Key.ToText() : vmType.Name;
        }
    }
}
