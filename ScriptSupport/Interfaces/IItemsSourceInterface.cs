using ScriptSupport.Models;

namespace ScriptSupport.Interfaces
{
    public interface IItemsSourceInterface
    {
        IReadOnlyList<string> FontSizeList { get; }
        IReadOnlyList<CmbItems> IndentOptions { get; }
        IReadOnlyList<CmbItems> NewLineOptions { get; }
    }
}
