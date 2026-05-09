namespace ScriptSupport.ViewModels
{
    public class ScriptStore : IDisposable
    {
        private IReadOnlyList<string> _allPaths = Array.Empty<string>();
        private Dictionary<ulong, IReadOnlyList<string>> _cardScripts = new();
        public IReadOnlyDictionary<ulong, IReadOnlyList<string>> CardScripts => _cardScripts;
        public IReadOnlyList<string> AllPaths => _allPaths;

        public void Set(Dictionary<ulong, IReadOnlyList<string>> scripts)
        {
            var allPaths = (IReadOnlyList<string>)scripts.SelectMany(c => c.Value).ToList();

            Interlocked.Exchange(ref _allPaths, allPaths);
            Interlocked.Exchange(ref _cardScripts, scripts);
        }
        public void Dispose()
        {
            Interlocked.Exchange(ref _allPaths, Array.Empty<string>());
            Interlocked.Exchange(ref _cardScripts, new Dictionary<ulong, IReadOnlyList<string>>());
        }
    }
}
