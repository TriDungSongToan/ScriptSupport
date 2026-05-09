using Microsoft.Extensions.DependencyInjection;
using ScriptSupport.ViewModels;

namespace ScriptSupport.Factorys
{
    public class DocumentFactory
    {
        private readonly IServiceProvider _services;

        public DocumentFactory(IServiceProvider services)
        {
            _services = services;
        }

        public DocumentViewModel CreateDocument()
        {
            return _services.GetRequiredService<DocumentViewModel>();
        }
    }
}
