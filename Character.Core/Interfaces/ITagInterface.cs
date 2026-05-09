using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Character.Core.Models;

namespace Character.Core.Interfaces
{
    public interface ITagInterface
    {
        Task<(List<TagItem>?, string)> ReadLinesAsync(string filePath, CancellationToken cancellationToken = default);
        Task<(bool, string)> WriteLinesAsync(List<TagItem> items, string filePath, CancellationToken cancellationToken = default);
        HashSet<TagItem> ExtractTags(IEnumerable<CharacterItem> items);
    }
}
