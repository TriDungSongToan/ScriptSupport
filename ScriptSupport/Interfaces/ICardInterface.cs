using ScriptSupport.Models;

namespace ScriptSupport.Interfaces
{
    public interface ICardInterface
    {
        Task<(bool Success, string Message)> LoadCardDBAsync();
        Task<List<CardText>?> ApplyFilterAsync();
        CardData? GetCardDataByID(ulong cardID);
        IReadOnlyList<CardData>? GetListCardDataByAlias(ulong alias);
        IReadOnlyList<CardText>? GetListCardTextByID(ulong cardID);
        List<ulong> GetListIDByID(ulong id);
    }
}
