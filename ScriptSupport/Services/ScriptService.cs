using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using ScriptSupport.Stores;
using ScriptSupport.States;
using ScriptSupport.Models;
using ScriptSupport.ViewModels;
using ScriptSupport.Interfaces;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.Services
{
    public class ScriptService : IScriptInterface, IDisposable
    {
        private readonly ConfigStore _config;
        private readonly ScriptStore _scriptStore;
        private FilterConfigState _filterConfig { get; }

        public ScriptService(ConfigStore config, ScriptStore scriptStore, FilterConfigState filterConfig)
        {
            _config = config;
            _scriptStore = scriptStore;
            _filterConfig = filterConfig;
        }

        #region Load Data
        public async Task<(bool Success, string Message)> LoadScriptsAsync()
        {
            string dataSourcePath = _config.UserSetting.DataSource;
            if (string.IsNullOrEmpty(dataSourcePath) || !Directory.Exists(dataSourcePath))
                return (false, ScriptSupport.Localization.Language.dataSourceMiss.ToText());

            var (dict, message) = await LoadCardScript(dataSourcePath);
            if (dict == null) return (false, message);
            _scriptStore.Set(dict);
            return (true, string.Empty);
        }
        public async Task<(Dictionary<ulong, IReadOnlyList<string>>?, string)> LoadCardScript(string root)
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

                // Start worker tasks
                var workerTasks = new Task<Dictionary<ulong, List<string>>>[workers];
                for (int w = 0; w < workers; w++)
                {
                    workerTasks[w] = Task.Run(async () =>
                    {
                        var localDict = new Dictionary<ulong, List<string>>(4096);
                        var reader = channel.Reader;

                        while (await reader.WaitToReadAsync())
                        {
                            while (reader.TryRead(out var path))
                            {
                                var fileName = Path.GetFileName(path);

                                // Quick check: tối thiểu 6 ký tự, bắt đầu bằng 'c'
                                if (fileName.Length < 6 || (fileName[0] != 'c' && fileName[0] != 'C'))
                                    continue;

                                // Manual check extension ".lua" (case-insensitive)
                                if (fileName[^4] != '.' ||
                                    ((fileName[^3] | 32) != 'l') ||
                                    ((fileName[^2] | 32) != 'u') ||
                                    ((fileName[^1] | 32) != 'a'))
                                    continue;

                                // Parse ID: các ký tự từ index 1 đến trước ".lua"
                                ulong id = 0;
                                int i = 1;
                                while (i < fileName.Length - 4)
                                {
                                    char c = fileName[i];
                                    if (c < '0' || c > '9') break; // dừng khi gặp ký tự không phải số
                                    id = id * 10 + (ulong)(c - '0');
                                    i++;
                                }

                                // Nếu không có số nào sau 'c', bỏ qua file
                                if (i == 1) continue;

                                // Thêm vào dictionary
                                if (!localDict.TryGetValue(id, out var list))
                                {
                                    list = new List<string>(1);
                                    localDict[id] = list;
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

                            foreach (var file in Directory.EnumerateFiles(dir, "*.lua"))
                                channel.Writer.TryWrite(file);
                        }
                        catch { } // skip directories/files with access issues
                    }
                    channel.Writer.Complete();
                });

                await Task.WhenAll(workerTasks);

                var final = new Dictionary<ulong, IReadOnlyList<string>>(32768);
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


                //////////////////////////////////////// Original version for reference
                
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

                                    if (name.Length < 6) continue;
                                    if (name[0] != 'c') continue;
                                    if (name[^4] != '.' ||
                                       ((name[^3] | 32) != 'l') ||
                                       ((name[^2] | 32) != 'u') ||
                                       ((name[^1] | 32) != 'a'))
                                        continue;

                                    ulong id = 0;
                                    int i = 1;

                                    while (i < name.Length)
                                    {
                                        uint d = (uint)(name[i] - '0');
                                        if (d > 9) break;
                                        id = id * 10 + d;
                                        i++;
                                    }

                                    if (i == 1) continue;
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
                    stack.Push(root);
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
        public async Task<(Dictionary<ulong, IReadOnlyList<string>>?, string)> LoadCardScript1(string root)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))  return (null, CMess.dataSourceMiss.ToText());

            try
            {
                // BoundedChannel để tránh memory spike
                var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(4096)
                {
                    SingleWriter = false,   // nhiều producer song song
                    SingleReader = false,
                    FullMode = BoundedChannelFullMode.Wait
                });

                int workerCount = Math.Max(2, System.Environment.ProcessorCount);

                // Workers parse tên file
                var workerTasks = Enumerable.Range(0, workerCount).Select(_ => Task.Run(async () =>
                {
                    var local = new Dictionary<ulong, List<string>>(4096);
                    await foreach (var path in channel.Reader.ReadAllAsync())
                    {
                        if (!TryParseCardPath(path, out ulong id)) continue;

                        if (!local.TryGetValue(id, out var list)) local[id] = list = new List<string>(1);
                        list.Add(path);
                    }
                    return local;
                })).ToArray();

                // Producer: song song hóa việc duyệt thư mục
                await ProduceParallelAsync(root, channel.Writer);

                await Task.WhenAll(workerTasks);

                // Merge
                var final = new Dictionary<ulong, IReadOnlyList<string>>(32768);
                foreach (var task in workerTasks)
                    foreach (var kv in task.Result)
                    {
                        if (!final.TryGetValue(kv.Key, out var existing)) final[kv.Key] = kv.Value;
                        else ((List<string>)existing).AddRange(kv.Value); // cast vì ta kiểm soát type
                    }

                return (final, string.Empty);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
        private static async Task ProduceParallelAsync(string root, ChannelWriter<string> writer)
        {
            try
            {
                // Lấy tất cả subdirectories trước
                var dirs = new List<string> { root };
                try { dirs.AddRange(Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories)); }
                catch { }

                // Song song hóa EnumerateFiles trên nhiều folder
                await Parallel.ForEachAsync(dirs,
                    new ParallelOptions { MaxDegreeOfParallelism = Math.Max(2, System.Environment.ProcessorCount) },
                    async (dir, ct) =>
                    {
                        IEnumerable<string> files;
                        try { files = Directory.EnumerateFiles(dir, "*.lua"); }
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryParseCardPath(string path, out ulong id)
        {
            id = 0;
            var fileName = Path.GetFileNameWithoutExtension(path.AsSpan()); // tránh alloc

            if (fileName.Length < 2) return false;
            if (fileName[0] != 'c' && fileName[0] != 'C') return false;

            for (int i = 1; i < fileName.Length; i++)
            {
                uint d = (uint)(fileName[i] - '0');
                if (d > 9) return false;
                id = id * 10 + d;
            }
            return id > 0;
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

        public async Task SearchFileContent(string query, IProgress<List<ScriptItem>> progress, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query)) return;

            // Local variable thay vì instance field
            int? maxItems = _filterConfig.MaxItems;
            var regex = BuildRegex(query);

            // Dùng Channel để tách producer/consumer
            var channel = Channel.CreateUnbounded<ScriptItem>(
                new UnboundedChannelOptions { SingleReader = true });

            var producer = Task.Run(async () =>
            {
                try
                {
                    int totalFound = 0;

                    await Parallel.ForEachAsync(
                        _scriptStore.AllPaths,
                        new ParallelOptions { CancellationToken = ct, MaxDegreeOfParallelism = System. Environment.ProcessorCount },
                        async (filePath, innerCt) =>
                        {
                            // Check trước khi đọc file
                            if (maxItems.HasValue && Interlocked.Read(ref Unsafe.As<int, long>(ref totalFound)) >= maxItems.Value)
                                return;

                            try
                            {
                                var matchedLines = new List<int>();
                                var lineNumber = 0;

                                foreach (var line in File.ReadLines(filePath))
                                {
                                    innerCt.ThrowIfCancellationRequested();
                                    lineNumber++;

                                    if (regex.IsMatch(line)) matchedLines.Add(lineNumber);


                                    //if (!regex.IsMatch(line)) continue;

                                    //var current = Interlocked.Increment(ref totalFound);
                                    //if (maxItems.HasValue && current > maxItems.Value) return;

                                    //await channel.Writer.WriteAsync(
                                    //    new ScriptItem { FullPath = filePath, LineNumber = lineNumber }, innerCt);

                                    //break; // Bỏ nếu muốn lấy tất cả dòng match
                                }
                                if (matchedLines.Count > 0)
                                {
                                    await channel.Writer.WriteAsync(new ScriptItem { FullPath = filePath, LineNumbers = matchedLines }, innerCt);
                                }
                            }
                            catch (OperationCanceledException) { }
                            catch { }
                        });
                }
                finally
                {
                    channel.Writer.Complete();
                }
            }, ct);

            // Consumer: gom batch và report
            var batch = new List<ScriptItem>(BatchSize);
            await foreach (var item in channel.Reader.ReadAllAsync(ct))
            {
                batch.Add(item);
                if (batch.Count >= BatchSize)
                {
                    progress.Report(new List<ScriptItem>(batch));
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
                progress.Report(new List<ScriptItem>(batch));

            await producer; // Propagate exceptions
        }
        #endregion

        #region Get Data
        public IReadOnlyList<string>? GetListScript(ulong cardId)
        {
            if (_scriptStore.CardScripts.TryGetValue(cardId, out var scripts))
            {
                return scripts;
            }
            return null;
        }
        #endregion

        public void Dispose()
        {
            //_scriptStore.Dispose();
        }
    }
}
