using Character.Core.Models;

namespace ScriptSupport.Interfaces
{
    public interface ISpecialCharInterface
    {
        Task<(bool Success, string Message)> LoadChar();
        IEnumerable<CharacterItem> Filter(CharacterFilter filter);
    }
}
