using ICSharpCode.AvalonEdit.Document;
using Scrapiyard.Core.Models;

namespace ScriptSupport.Editor.Hover
{
    public interface ISymbolResolver
    {
        CompletionSymbol? ResolveExpression(string expression);
        CompletionSymbol? Resolve(TextDocument document, int offset);

    }
}
