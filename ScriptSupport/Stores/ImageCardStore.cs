namespace ScriptSupport.Stores
{
    public class ImageCardStore : IDisposable
    {
        private Dictionary<ulong, IReadOnlyList<string>> _cardImages = new();
        public IReadOnlyDictionary<ulong, IReadOnlyList<string>> CardImages => _cardImages;

        public void SetCardImages(Dictionary<ulong, IReadOnlyList<string>> cardImages)
        {
            Interlocked.Exchange(ref _cardImages, cardImages);
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _cardImages, new Dictionary<ulong, IReadOnlyList<string>>());
        }
    }
}
