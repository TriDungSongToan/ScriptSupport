using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using Scrapiyard.Core.Models;
using ScriptSupport.ViewModels;
using Scrapiyard.Core.Services;
using ScriptSupport.UserControls;

namespace ScriptSupport.Editor.Analysis
{
    public interface ISymbolDescriptionPresenter
    {
        FrameworkElement Create(CompletionSymbol symbol);
    }
    public sealed class ScriptDescriptionPresenter : ISymbolDescriptionPresenter
    {
        private readonly Func<ScriptDescViewModel> _vmFactory;
        public ScriptDescriptionPresenter(Func<ScriptDescViewModel> vmFactory)
        {
            _vmFactory = vmFactory;
        }
        public FrameworkElement Create(CompletionSymbol symbol)
        {
            TextDocument document = symbol == null
                ? new TextDocument(string.Empty)
                : new TextDocument(SymbolDescriptionBuilder.Build(symbol));

            // 3. Create ViewModel (KHÔNG dùng constructor)
            var vm = _vmFactory();
            vm.SetDocument(document);

            // 4. Bind vào View
            return new ScriptDescription
            {
                DataContext = vm
            };
        }
    }
}
