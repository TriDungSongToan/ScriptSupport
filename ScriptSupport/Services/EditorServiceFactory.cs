using ICSharpCode.AvalonEdit;
using ScriptSupport.Editor.Analysis;
using ScriptSupport.Editor.Completion;
using ScriptSupport.Editor.Hover;
using ScriptSupport.Interfaces;
using ScriptSupport.ViewModels;

namespace ScriptSupport.Services
{
    public sealed class EditorServiceFactory : IEditorServiceFactory
    {
        private readonly IScrapiyardInterface _scrapiyardService;
        private readonly ISymbolDescriptionPresenter _presenter;
        private readonly Func<ScriptDescViewModel> _vmFactory;

        public EditorServiceFactory(IScrapiyardInterface scrapiyardService,
            ISymbolDescriptionPresenter presenter, Func<ScriptDescViewModel> vmFactory)
        {
            _scrapiyardService = scrapiyardService;
            _presenter = presenter;
            _vmFactory = vmFactory;
        }

        public CompletionService CreateCompletion(TextEditor editor)
            => new CompletionService(editor, _scrapiyardService, _presenter);
        public HoverService CreateHover(TextEditor editor)
            => new HoverService(editor, _scrapiyardService, _vmFactory);

    }
}
