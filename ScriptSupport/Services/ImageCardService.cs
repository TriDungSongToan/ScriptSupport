using System.IO;
using ScriptSupport.Stores;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.Services
{
    public class ImageCardService : IImageCardInterface
    {
        private readonly AppEnvironment _env;
        private readonly ConfigStore _config;
        private readonly ImageCardStore _imageCardStore;

        public ImageCardService(AppEnvironment env, ConfigStore config, ImageCardStore imageCardStore)
        {
            _env = env;
            _config = config;
            _imageCardStore = imageCardStore;
        }

        public async Task<(bool Success, string Message)> LoadCardImagesAsync()
        {
            string dataSourcePath = _config.UserSetting.DataSource;
            if (string.IsNullOrEmpty(dataSourcePath) || !Directory.Exists(dataSourcePath))
                return (false, CMess.dataSourceMiss.ToText());

            var (dict, message) = await LoadCardImage(dataSourcePath);
            if (dict == null) return (false, message);
            _imageCardStore.SetCardImages(dict);
            return (true, string.Empty);
        }
        public async Task<(Dictionary<ulong, IReadOnlyList<string>>?, string)> LoadCardImage(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath) || !Directory.Exists(dataSourcePath))
                return (null, CMess.dataSourceMiss.ToText());

            try
            {
                var result = await Task.Run(async () =>
                {
                    int workers = Math.Max(2, System.Environment.ProcessorCount);
                    var channel = System.Threading.Channels.Channel.CreateUnbounded<string>(
                        new System.Threading.Channels.UnboundedChannelOptions
                        {
                            SingleWriter = true,
                            SingleReader = false
                        });
                    var workerTasks = new Task<Dictionary<ulong, List<string>>>[workers];

                    for (int w = 0; w < workers; w++)
                    {
                        workerTasks[w] = Task.Run(async () =>
                        {
                            var local = new Dictionary<ulong, List<string>>(4096);
                            var reader = channel.Reader;

                            while (await reader.WaitToReadAsync())
                            {
                                while (reader.TryRead(out var path))
                                {
                                    var name = Path.GetFileName(path);

                                    if (name.Length < 5) continue;
                                    bool validExt = (name[^4] == '.' &&
                                        ((name[^3] | 32) == 'p') &&
                                        ((name[^2] | 32) == 'n') &&
                                        ((name[^1] | 32) == 'g')) ||
                                        (name[^4] == '.' &&
                                        ((name[^3] | 32) == 'j') &&
                                        ((name[^2] | 32) == 'p') &&
                                        ((name[^1] | 32) == 'g')) ||
                                        (name.Length > 5 &&
                                        name[^5] == '.' &&
                                        ((name[^4] | 32) == 'j') &&
                                        ((name[^3] | 32) == 'p') &&
                                        ((name[^2] | 32) == 'e') &&
                                        ((name[^1] | 32) == 'g'));

                                    if (!validExt) continue;

                                    ulong id = 0;
                                    int i = 0;

                                    while (i < name.Length)
                                    {
                                        uint d = (uint)(name[i] - '0');
                                        if (d > 9) break;
                                        id = id * 10 + d;
                                        i++;
                                    }

                                    if (i == 0) continue;
                                    if (!local.TryGetValue(id, out var list))
                                    {
                                        list = new List<string>(1);
                                        local[id] = list;
                                    }
                                    list.Add(path);
                                }
                            }
                            return local;
                        });
                    }

                    // Producer: traverse directories
                    var stack = new Stack<string>();
                    stack.Push(dataSourcePath);
                    while (stack.Count > 0)
                    {
                        var dir = stack.Pop();
                        try
                        {
                            foreach (var sub in Directory.EnumerateDirectories(dir))
                                stack.Push(sub);

                            foreach (var file in Directory.EnumerateFiles(dir))
                                channel.Writer.TryWrite(file);
                        }
                        catch { }
                    }

                    channel.Writer.Complete();
                    await Task.WhenAll(workerTasks);

                    // Merge dictionaries
                    var final = new Dictionary<ulong, IReadOnlyList<string>>(32768);
                    foreach (var task in workerTasks)
                    {
                        foreach (var kv in task.Result)
                        {
                            if (!final.TryGetValue(kv.Key, out var list))
                            {
                                final[kv.Key] = kv.Value;
                            }
                            else
                            {
                                var merged = new List<string>(list.Count + kv.Value.Count);
                                merged.AddRange(list);
                                merged.AddRange(kv.Value);
                                final[kv.Key] = merged;
                            }
                        }
                    }
                    return final;
                });

                return (result, "");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
        public IReadOnlyList<string>? GetImagePath(ulong cardId)
        {
            var cardImages = _imageCardStore.CardImages;
            if (cardImages != null && cardImages.TryGetValue(cardId, out var images))
                return images;

            return null;
        }

    }
}
