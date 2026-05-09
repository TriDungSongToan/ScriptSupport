using Scrapiyard.Core.Models;
using ScriptSupport.Models;

namespace ScriptSupport.Interfaces
{
    public interface IResultInterface
    {
        void BuildResultCardDocument(CardText? card);
        void BuildResultScrapiyardDocument(CompletionSymbol? completionSymbol);
        void BuildResultID(CardText? card);
        void BuildResultID(ulong cardID);
        void BuildResultCardTexts(CardText? card);
        void BuildResultCardTexts(ulong cardID);
        void BuildResultImageCards(CardText? card);
        void BuildResultImageCards(ulong cardID);
        void BuildResultImageCards(List<ulong> ids);
        List<FileItem> GetListImageCards(ulong id);
        void BuildResultCardData(CardText? card);
        void BuildResultCardData(ulong id);
        void BuildResultScrapiyard(CompletionSymbol? completionSymbol);
    }
}
