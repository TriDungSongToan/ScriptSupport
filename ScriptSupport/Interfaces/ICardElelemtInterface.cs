namespace ScriptSupport.Interfaces
{
    public interface ICardElelemtInterface
    {
        Task<(bool Success, string Message)> LoadAllCardElement();
    }
}
