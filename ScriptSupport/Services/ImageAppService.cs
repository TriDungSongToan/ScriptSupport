using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ScriptSupport.Models;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;

namespace ScriptSupport.Services
{
    public class ImagePathResolver
    {
        private readonly AppEnvironment _env;
        public ImagePathResolver(AppEnvironment env)
        {
            _env = env;
        }
        public Dictionary<AppImage, string> Build()
        {
            var imgPath = Path.Combine(_env.BaseDirectory, "Images");

            return new Dictionary<AppImage, string>
            {
                { AppImage.Error, Path.Combine(imgPath,"Message","Error.png") },
                { AppImage.Information, Path.Combine(imgPath,"Message","Information.png") },
                { AppImage.Notification, Path.Combine(imgPath,"Message","Notification.png") },
                { AppImage.Question, Path.Combine(imgPath,"Message","Question.png") },
                { AppImage.Warning, Path.Combine(imgPath,"Message","Warning.png") },

                { AppImage.Blank, Path.Combine(imgPath,"Blank.png") },
                { AppImage.Logo, Path.Combine(imgPath,"Logo.ico") },
                { AppImage.LevelStar, "pack://application:,,,/ScriptSupport;component/Images/DataEditor/Level.ico" },
                { AppImage.RankStar, "pack://application:,,,/ScriptSupport;component/Images/DataEditor/Rank.ico" },
                { AppImage.LevelRankStar, "pack://application:,,,/ScriptSupport;component/Images/DataEditor/LevelRank.ico" }
            };
        }
    }

    public class ImageAppService : IImageAppInterface
    {
        private readonly Dictionary<AppImage, ImageSource> _cache = new();
        private readonly Dictionary<AppImage, string> _paths;
        public ImageAppService(ImagePathResolver resolver)
        {
            _paths = resolver.Build();
        }

        public async Task LoadAsync()
        {
            var tasks = _paths.Select(kvp => Task.Run(async () =>
            {
                var image = await LoadImageAsync(kvp.Value);
                _cache[kvp.Key] = image;
            }));

            await Task.WhenAll(tasks);
        }
        public async Task<ImageSource> Get(AppImage image)
        {
            if (_cache.TryGetValue(image, out var img)) return img;

            if (_paths.TryGetValue(image, out var path))
            {
                img = await LoadImageAsync(path);
                _cache[image] = img;
                return img;
            }

            return CreateFallback();
        }
        private Task<ImageSource> LoadImageAsync(string path)
        {
            try
            {
                var bmp = new BitmapImage(new Uri(path, UriKind.Absolute));
                bmp.Freeze();
                return Task.FromResult<ImageSource>(bmp);
            }
            catch
            {
                return Task.FromResult<ImageSource>(CreateFallback());
            }
        }

        private BitmapSource CreateFallback()
        {
            var wb = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Bgra32, null);
            wb.Freeze();
            return wb;
        }
    }
}
