using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ScriptSupport.ViewModels;
using ScriptSupport.UserControls;

namespace ScriptSupport.Factorys
{
    public class PanelFactory
    {
        private readonly IServiceProvider _provider;

        public PanelFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public UIElement Create(Type vmType)
        {
            return vmType switch
            {
                Type t when t == typeof(SpecialCharViewModel) => _provider.GetRequiredService<SpecialCharacter>(),

                _ => throw new InvalidOperationException($"No panel registered for ViewModel type {vmType}.")
            };
        }
    }
}
