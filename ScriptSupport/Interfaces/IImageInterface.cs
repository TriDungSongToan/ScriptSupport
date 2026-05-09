using System.Windows.Media;
using ScriptSupport.Models;

namespace ScriptSupport.Interfaces
{
    public interface IImageAppInterface
    {
        Task<ImageSource> Get(AppImage image);
        Task LoadAsync();
    }
    public interface IImageCardInterface
    {
        Task<(bool Success, string Message)> LoadCardImagesAsync();
        IReadOnlyList<string>? GetImagePath(ulong cardId);
    }
}
