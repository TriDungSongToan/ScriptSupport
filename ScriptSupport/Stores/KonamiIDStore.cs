namespace ScriptSupport.Stores
{
    public class KonamiIDStore : IDisposable
    {
        private Dictionary<ulong, int> _officialCardIDs = new();
        private Dictionary<string, int> _rushCardIDs = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<ulong, int> OfficialCardIDs => _officialCardIDs;
        public IReadOnlyDictionary<string, int> RushCardIDs => _rushCardIDs;

        public void SetOfficialData(Dictionary<ulong, int> data)
        {
            Interlocked.Exchange(ref _officialCardIDs, data ?? new());
        }

        public void SetRushData(Dictionary<string, int> data)
        {
            Interlocked.Exchange(ref _rushCardIDs, data ?? new());
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _officialCardIDs, new());
            Interlocked.Exchange(ref _rushCardIDs, new());
        }
    }
}
