namespace ScriptSupport.Stores
{
    public class ScrapiStore : IDisposable
    {
        private IReadOnlyList<string> _allPaths = Array.Empty<string>();
        private Dictionary<string, IReadOnlyList<string>> _scrapis = new(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Scrapis => _scrapis;
        public IReadOnlyList<string> AllPaths => _allPaths;

        public void Set(Dictionary<string, IReadOnlyList<string>> scrapies)
        {
            var allPaths = (IReadOnlyList<string>)scrapies.SelectMany(c => c.Value).ToList();
            Interlocked.Exchange(ref _allPaths, allPaths);
            Interlocked.Exchange(ref _scrapis, scrapies);
        }
        public void Dispose()
        {
            Interlocked.Exchange(ref _allPaths, Array.Empty<string>());
            Interlocked.Exchange(ref _scrapis, new Dictionary<string, IReadOnlyList<string>>());
        }
    }
}
