using ICSharpCode.AvalonEdit;
using ScriptSupport.Editor.Completion;
using ScriptSupport.Editor.Hover;

namespace ScriptSupport.Interfaces
{
    public interface IEditorServiceFactory
    {
        CompletionService CreateCompletion(TextEditor editor);
        HoverService CreateHover(TextEditor editor);
    }
}
