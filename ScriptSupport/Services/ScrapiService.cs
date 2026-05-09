using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using ScriptSupport.Stores;
using ScriptSupport.States;
using ScriptSupport.Models;
using ScriptSupport.Environment;
using ScriptSupport.Interfaces;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.Services
{
    public class ScrapiService : IScrapiInterface, IDisposable
    {
        private readonly AppEnvironment _env;
        private readonly ConfigStore _config;
        private readonly ScrapiStore _scrapiStore;
        private FilterConfigState _filterConfig { get; }

        public ScrapiService(AppEnvironment env, ConfigStore config, ScrapiStore scrapiStore, FilterConfigState filterConfig)
        {
            _env = env;
            _config = config;
            _scrapiStore = scrapiStore;
            _filterConfig = filterConfig;
        }

        #region Load Data
        public async Task<(bool Success, string Message)> LoadScrapisAsync()
        {
            string ScrapiFolderPath = _env.ScrapiyardFolderPath;
            string dataSourcePath = System.IO.Path.Combine(ScrapiFolderPath, "api");

            if (string.IsNullOrEmpty(dataSourcePath) || !Directory.Exists(dataSourcePath))
                return (false, CMess.dataSourceMiss.ToText());

            var (dict, message) = await LoadScrapis(dataSourcePath);
            if (dict == null) return (false, message);
            _scrapiStore.Set(dict);
            return (true, string.Empty);
        }

        public async Task<(Dictionary<string, IReadOnlyList<string>>?, string)> LoadScrapis(string root)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root)) return (null, CMess.dataSourceMiss.ToText());

            try
            {
                int workers = Math.Max(2, System.Environment.ProcessorCount * 2);
                var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(4096)
                {
                    SingleWriter = true,
                    SingleReader = false,
                    FullMode = BoundedChannelFullMode.Wait
                });

                // Workers
                var workerTasks = new Task<Dictionary<string, List<string>>>[workers];
                for (int w = 0; w < workers; w++)
                {
                    workerTasks[w] = Task.Run(async () =>
                    {
                        var localDict = new Dictionary<string, List<string>>(4096, StringComparer.OrdinalIgnoreCase);
                        var reader = channel.Reader;

                        while (await reader.WaitToReadAsync())
                        {
                            while (reader.TryRead(out var path))
                            {
                                var fileName = Path.GetFileName(path);

                                // check ".yml" (case-insensitive)
                                if (fileName.Length < 5 ||
                                    fileName[^4] != '.' ||
                                    ((fileName[^3] | 32) != 'y') ||
                                    ((fileName[^2] | 32) != 'm') ||
                                    ((fileName[^1] | 32) != 'l'))
                                    continue;

                                var key = Path.GetFileNameWithoutExtension(fileName);
                                if (string.IsNullOrEmpty(key)) continue;

                                if (!localDict.TryGetValue(key, out var list))
                                {
                                    list = new List<string>(1);
                                    localDict[key] = list;
                                }
                                list.Add(path);

                            }
                        }
                        return localDict;

                    });
                }

                await Task.Run(() =>
                {
                    var stack = new Stack<string>();
                    stack.Push(root);

                    while (stack.Count > 0)
                    {
                        var dir = stack.Pop();
                        try
                        {
                            foreach (var subDir in Directory.EnumerateDirectories(dir))
                                stack.Push(subDir);
                            foreach (var file in Directory.EnumerateFiles(dir, "*.yml"))
                                channel.Writer.TryWrite(file);
                        }
                        catch { }
                    }
                    channel.Writer.Complete();
                });

                await Task.WhenAll(workerTasks);

                var final = new Dictionary<string, IReadOnlyList<string>>(32768, StringComparer.OrdinalIgnoreCase);
                foreach (var task in workerTasks)
                {
                    foreach (var kv in task.Result)
                    {
                        if (!final.TryGetValue(kv.Key, out var list))
                            final[kv.Key] = kv.Value;
                        else
                        {
                            var merged = new List<string>(list.Count + kv.Value.Count);
                            merged.AddRange(list);
                            merged.AddRange(kv.Value);
                            final[kv.Key] = merged;
                        }
                    }
                }
                return (final, string.Empty);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public async Task<(Dictionary<string, IReadOnlyList<string>>?, string)> LoadScrapis1(string root)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root)) return (null, CMess.dataSourceMiss.ToText());

            try
            {
                var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(4096)
                {
                    SingleWriter = false,
                    SingleReader = false,
                    FullMode = BoundedChannelFullMode.Wait
                });

                int workerCount = Math.Max(2, System.Environment.ProcessorCount);

                var workerTasks = Enumerable.Range(0, workerCount).Select(_ => Task.Run(async () =>
                {
                    var local = new Dictionary<string, List<string>>(4096, StringComparer.OrdinalIgnoreCase);

                    await foreach (var path in channel.Reader.ReadAllAsync())
                    {
                        var name = Path.GetFileNameWithoutExtension(path);
                        if (string.IsNullOrEmpty(name)) continue;

                        if (!local.TryGetValue(name, out var list))
                            local[name] = list = new List<string>(1);

                        list.Add(path);
                    }

                    return local;
                })).ToArray();

                await ProduceParallelYmlAsync(root, channel.Writer);

                await Task.WhenAll(workerTasks);

                var final = new Dictionary<string, IReadOnlyList<string>>(32768, StringComparer.OrdinalIgnoreCase);

                foreach (var task in workerTasks)
                {
                    foreach (var kv in task.Result)
                    {
                        if (!final.TryGetValue(kv.Key, out var existing))
                        {
                            final[kv.Key] = kv.Value;
                        }
                        else
                        {
                            ((List<string>)existing).AddRange(kv.Value);
                        }
                    }
                }

                return (final, string.Empty);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
        private static async Task ProduceParallelYmlAsync(string root, ChannelWriter<string> writer)
        {
            try
            {
                var dirs = new List<string> { root };
                try { dirs.AddRange(Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories)); }
                catch { }

                await Parallel.ForEachAsync(dirs,
                    new ParallelOptions { MaxDegreeOfParallelism = Math.Max(2, System.Environment.ProcessorCount) },
                    async (dir, ct) =>
                    {
                        IEnumerable<string> files;
                        try { files = Directory.EnumerateFiles(dir, "*.yml"); }
                        catch { return; }

                        foreach (var file in files)
                            await writer.WriteAsync(file, ct);
                    });
            }
            finally
            {
                writer.Complete();
            }
        }
        #endregion

        #region Filter
        private const int BatchSize = 50;

        private Regex BuildRegex(string query)
        {
            var options = RegexOptions.Compiled;
            if (!_filterConfig.MatchCase)
                options |= RegexOptions.IgnoreCase;

            var pattern = _filterConfig.Wildcards
                ? Regex.Escape(query).Replace(@"\*", ".*").Replace(@"\?", ".")
                : Regex.Escape(query);

            if (_filterConfig.MatchWhole)
                pattern = $@"\b{pattern}\b";

            return new Regex(pattern, options);
        }

        public async Task SearchFileNames(string query, IProgress<List<FileItem>> progress, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query)) return;

            int? maxItems = _filterConfig.MaxItems;
            var regex = BuildRegex(query);

            var channel = Channel.CreateUnbounded<FileItem>(
                new UnboundedChannelOptions { SingleReader = true });

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var token = linkedCts.Token;
            int totalFound = 0;

            var producer = Task.Run(async () =>
            {
                try
                {
                    await Parallel.ForEachAsync(
                        _scrapiStore.AllPaths,
                        new ParallelOptions
                        {
                            CancellationToken = token,
                            MaxDegreeOfParallelism = System.Environment.ProcessorCount
                        },
                        async (filePath, innerCt) =>
                        {
                            if (token.IsCancellationRequested) return;

                            try
                            {
                                var fileName = Path.GetFileName(filePath);

                                if (!regex.IsMatch(fileName)) return;

                                if (maxItems.HasValue && Volatile.Read(ref totalFound) >= maxItems.Value)
                                {
                                    linkedCts.Cancel();
                                    return;
                                }

                                await channel.Writer.WriteAsync(new FileItem { FullPath = filePath }, innerCt);

                                if (maxItems.HasValue)
                                {
                                    int current = Interlocked.Increment(ref totalFound);

                                    if (current >= maxItems.Value)
                                    {
                                        linkedCts.Cancel();
                                        return;
                                    }
                                }
                            }
                            catch (OperationCanceledException) { }
                            catch { }
                        });
                }
                finally
                {
                    channel.Writer.TryComplete();
                }
            }, token);

            var batch = new List<FileItem>(BatchSize);

            try
            {
                await foreach (var item in channel.Reader.ReadAllAsync(token))
                {
                    batch.Add(item);

                    if (batch.Count >= BatchSize)
                    {
                        progress.Report(new List<FileItem>(batch));
                        batch.Clear();
                    }

                    if (maxItems.HasValue && totalFound >= maxItems.Value)
                        break;
                }
            }
            catch (OperationCanceledException) { }

            if (batch.Count > 0)
                progress.Report(new List<FileItem>(batch));

            await producer;
        }
        public async Task SearchFileContent(string query, IProgress<List<FileItem>> progress, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query)) return;

            int? maxItems = _filterConfig.MaxItems;
            var regex = BuildRegex(query);

            var channel = Channel.CreateUnbounded<FileItem>(
                new UnboundedChannelOptions { SingleReader = true });

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var token = linkedCts.Token;
            int totalFound = 0;

            var producer = Task.Run(async () =>
            {
                try
                {
                    await Parallel.ForEachAsync(
                        _scrapiStore.AllPaths,
                        new ParallelOptions
                        {
                            CancellationToken = token,
                            MaxDegreeOfParallelism = System.Environment.ProcessorCount
                        },
                        async (filePath, innerCt) =>
                        {
                            if (token.IsCancellationRequested) return;

                            try
                            {
                                foreach (var line in File.ReadLines(filePath))
                                {
                                    innerCt.ThrowIfCancellationRequested();

                                    if (!regex.IsMatch(line)) continue;

                                    // Nếu đã đạt max thì stop luôn
                                    if (maxItems.HasValue && Volatile.Read(ref totalFound) >= maxItems.Value)
                                    {
                                        linkedCts.Cancel();
                                        return;
                                    }

                                    await channel.Writer.WriteAsync(new FileItem { FullPath = filePath }, innerCt);

                                    if (maxItems.HasValue)
                                    {
                                        int current = Interlocked.Increment(ref totalFound);

                                        if (current >= maxItems.Value)
                                        {
                                            linkedCts.Cancel();
                                            return;
                                        }
                                    }
                                    break;
                                }
                            }
                            catch (OperationCanceledException) { }
                            catch { }
                        });
                }
                finally
                {
                    channel.Writer.TryComplete();
                }
            }, token);

            var batch = new List<FileItem>(BatchSize);
            try
            {
                await foreach (var item in channel.Reader.ReadAllAsync(token))
                {
                    batch.Add(item);

                    if (batch.Count >= BatchSize)
                    {
                        progress.Report(new List<FileItem>(batch));
                        batch.Clear();
                    }

                    if (maxItems.HasValue && totalFound >= maxItems.Value) break;
                }
            }
            catch (OperationCanceledException) { }

            if (batch.Count > 0) progress.Report(new List<FileItem>(batch));
            await producer;
        }
        #endregion

        #region Get Data
        public IReadOnlyList<string>? GetListScrapi(string name)
        {
            if (_scrapiStore.Scrapis.TryGetValue(name, out var scripts))
            {
                return scripts;
            }
            return null;
        }
        #endregion

        public void Dispose()
        {
            //_scrapiStore.Dispose();
        }
    }
}
