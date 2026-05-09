using ScriptSupport.Models;
using ScriptSupport.Interfaces;

namespace ScriptSupport.Services
{
    public class ItemsSourceService : IItemsSourceInterface
    {
        public IReadOnlyList<string> FontSizeList { get; } = new List<string>
        {
            "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48"
        };

        public IReadOnlyList<CmbItems> IndentOptions { get; } = new List<CmbItems>
        {
            new CmbItems {Name = "Spaces", ShortName = "SPC" },
            new CmbItems {Name = "Tabs", ShortName = "TAB" }
        };

        public IReadOnlyList<CmbItems> NewLineOptions { get; } = new List<CmbItems>
        {
            new CmbItems {Name = "CRLF (Windows)", ShortName = "CRLF" },
            new CmbItems {Name = "LF (Unix)", ShortName = "LF" },
            new CmbItems {Name = "CR (Mac)", ShortName = "CR" }
        };
    }
}
