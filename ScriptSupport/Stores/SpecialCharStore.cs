using Character.Core.Models;

namespace ScriptSupport.Stores
{
    public class SpecialCharStore : IDisposable
    {
        private List<CharacterItem> _charItems = new();
        private List<TagItem> _tagItems = new();
        public event Action? DataChanged;
        public IReadOnlyList<CharacterItem> CharItems => _charItems;
        public IReadOnlyList<TagItem> TagItems => _tagItems;

        public void SetCharItems(List<CharacterItem> charItems)
        {
            if (charItems == null) return;
            Interlocked.Exchange(ref _charItems, charItems);
            DataChanged?.Invoke();
        }
        public void SetTagItems(List<TagItem> tagItems)
        {
            if (tagItems == null) return;
            Interlocked.Exchange(ref _tagItems, tagItems);
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _charItems, new List<CharacterItem>());
            Interlocked.Exchange(ref _tagItems, new List<TagItem>());
        }
    }
}
