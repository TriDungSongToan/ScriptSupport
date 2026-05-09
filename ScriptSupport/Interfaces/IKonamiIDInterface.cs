namespace ScriptSupport.Interfaces
{
    public interface IKonamiIDInterface
    {
        Task<(bool Success, string Message)> LoadKonamiIDAsync();
        int? GetOfficialKonamiID(ulong password);
        int? GetRushKonamiID(string name);

        (bool Success, string Message) BuildKonamiDBUrl(ulong id, string name);
        (bool Success, string Message) BuildYuGiPediaUrl(ulong id, string name);
        (bool Success, string Message) BuildYGOResourcesUrl(ulong id, string name);

    }
}
