using System.IO;
using System.Text;
using System.Windows;
using ScriptSupport.Models;
using ScriptSupport.Stores;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.Services
{
    public class CardElelemtService : ICardElelemtInterface
    {
        private readonly AppEnvironment _env;
        private readonly ConfigStore _config;
        private readonly CardElementStore _cardElementStore;
        private static readonly object _lockObject = new object();

        public CardElelemtService(AppEnvironment env, ConfigStore config, CardElementStore cardElementStore)
        {
            _env = env;
            _config = config;
            _cardElementStore = cardElementStore;
        }

        public async Task<(bool Success, string Message)> LoadAllCardElement()
        {
            string LanguageCode = _config.UserSetting.Language;
            string Game = _config.UserSetting.Game;

            var tasks = new List<Task<(bool Success, string Message)>>
            {
                LoadRule(LanguageCode, Game),
                LoadType(LanguageCode),
                LoadTypeImage(LanguageCode),
                LoadRace(LanguageCode),
                LoadChar(LanguageCode),
                LoadAttri(LanguageCode),
                LoadSetCode(LanguageCode, Game),
                LoadCategory(LanguageCode, Game),
                LoadLinkArrow(LanguageCode),
                LoadFlag(LanguageCode),
                LoadSpecialCharacters(LanguageCode)
            };
            var results = await Task.WhenAll(tasks);
            bool success = results.All(r => r.Success);

            string message = string.Join(System.Environment.NewLine, results.Where(r => !string.IsNullOrWhiteSpace(r.Message))
                .Select(r => r.Message));
            return (success, message);
        }

        private async Task<(bool Success, string Message)> LoadRule(string LanguageCode, string Game)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\rule{Game}.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: rule{Game}.txt");

            try
            {
                var tempListItem = new List<RuleItem>();
                var tempList = new List<(ulong bit, string name)>();

                var existedRuleCodes = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string ruleName = parts[1].Trim();
                    if (existedRuleCodes.Add(bitValue))
                    {
                        tempListItem.Add(new RuleItem { RuleCode = bitValue, RuleName = ruleName });
                        tempList.Add((bitValue, ruleName));
                    }
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lock (_lockObject)
                    {
                        _cardElementStore.SetRuleItems(tempListItem, tempList);
                    }
                });
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadType(string LanguageCode)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\type.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: type.txt");

            try
            {
                var tempListItem = new List<TypeItem>();
                var tempList = new List<(ulong bit, string name)>();

                var existedTypeCodes = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string typeName = parts[1].Trim();
                    if (existedTypeCodes.Add(bitValue))
                    {
                        tempListItem.Add(new TypeItem { TypeCode = bitValue, TypeName = typeName });
                        tempList.Add((bitValue, typeName));
                    }
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lock (_lockObject)
                    {
                        _cardElementStore.SetTypeItems(tempListItem, tempList);
                    }
                });
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadTypeImage(string LanguageCode)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\typeImage.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: typeImage.txt");

            try
            {
                var tempList = new List<(ulong bit, string name)>();
                var existedTypeCodes = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string name = parts[1].Trim();
                    if (existedTypeCodes.Add(bitValue))
                    {
                        tempList.Add((bitValue, name));
                    }
                }
                //listtype = tempList.AsEnumerable().Reverse().ToList();
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadRace(string LanguageCode)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\race.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: race.txt");

            try
            {
                var tempListItem = new List<RaceItem>();
                var tempList = new List<(ulong bit, string name)>();

                var existedRaceCodes = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string raceName = parts[1].Trim();
                    if (existedRaceCodes.Add(bitValue))
                    {
                        tempListItem.Add(new RaceItem { RaceCode = bitValue, RaceName = raceName });
                        tempList.Add((bitValue, raceName));
                    }
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lock (_lockObject)
                    {
                        _cardElementStore.SetRaceItems(tempListItem, tempList);
                    }
                });
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadChar(string LanguageCode)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\character.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: character.txt");
            try
            {
                var tempListItem = new List<CharItem>();
                var tempList = new List<(ulong bit, string name)>();

                var existedCharCodes = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string charName = parts[1].Trim();
                    if (existedCharCodes.Add(bitValue))
                    {
                        tempListItem.Add(new CharItem { CharCode = bitValue, CharName = charName });
                        tempList.Add((bitValue, charName));
                    }
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lock (_lockObject)
                    {
                        _cardElementStore.SetCharItems(tempListItem, tempList);
                    }
                });
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadAttri(string LanguageCode)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\attribute.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: attribute.txt");

            try
            {
                var tempListItem = new List<AttributeItem>();
                var tempList = new List<(ulong bit, string name)>();

                var existedAttriCodes = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string attriName = parts[1].Trim();
                    if (existedAttriCodes.Add(bitValue))
                    {
                        tempListItem.Add(new AttributeItem { AttributeCode = bitValue, AttributeName = attriName });
                        tempList.Add((bitValue, attriName));
                    }
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lock (_lockObject)
                    {
                        _cardElementStore.SetAttributeItems(tempListItem, tempList);
                    }
                });
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadSetCode(string LanguageCode, string Game)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\setname{Game}.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: setname{Game}.txt");

            try
            {
                var tempListItem = new List<SetCodeItem>();
                var tempList = new List<(ulong bit, string name)>();

                var existedSetCodeCodes = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string setCodeName = parts[1].Trim();
                    if (existedSetCodeCodes.Add(bitValue))
                    {
                        tempListItem.Add(new SetCodeItem { SetCode = bitValue, SetName = setCodeName });
                        tempList.Add((bitValue, setCodeName));
                    }
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lock (_lockObject)
                    {
                        _cardElementStore.SetSetCodeItems(tempListItem, tempList);
                    }
                });
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadCategory(string LanguageCode, string Game)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\category{Game}.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: category{Game}.txt");

            try
            {
                var tempListItem = new List<CategoryItem>();
                var existedCategoryCodes = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string categoryName = parts[1].Trim();
                    if (existedCategoryCodes.Add(bitValue))
                    {
                        tempListItem.Add(new CategoryItem { CategoryCode = bitValue, CategoryName = categoryName });
                    }
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lock (_lockObject)
                    {
                        _cardElementStore.SetCategoryItems(tempListItem);
                    }
                });
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadLinkArrow(string LanguageCode)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\linkmarker.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: linkmarker.txt");

            try
            {
                var tempListItem = new List<LinkArrowItem>();
                var tempList = new List<(ulong bit, string name)>();

                var existedLinkarrow = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string linkArrowName = parts[1].Trim();
                    if (existedLinkarrow.Add(bitValue))
                    {
                        tempListItem.Add(new LinkArrowItem { LinkArrowCode = bitValue, LinkArrowName = linkArrowName });
                        tempList.Add((bitValue, linkArrowName));
                    }
                }
                lock (_lockObject)
                {
                    _cardElementStore.SetLinkArrowItems(tempListItem, tempList);
                }
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadFlag(string LanguageCode)
        {
            string dataPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\cardinfo\flag.txt");
            if (!File.Exists(dataPath)) return (false, $"{CMess.fileNotExit.ToText()}: flag.txt");

            try
            {
                var tempListItem = new List<FlagItem>();
                var existedFlagCodes = new HashSet<ulong>();
                const int bufferSize = 4096;

                using var fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
                using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: bufferSize, leaveOpen: false);

                while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) continue;
                    var parts = line.Split('\t');
                    if (parts.Length < 2) continue;

                    string hexPart = parts[0].Substring(2).Trim();
                    if (!ulong.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out ulong bitValue))
                        continue;

                    string flagName = parts[1].Trim();
                    if (existedFlagCodes.Add(bitValue))
                    {
                        tempListItem.Add(new FlagItem { FlagCode = bitValue, FlagName = flagName });
                    }
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lock (_lockObject)
                    {
                        _cardElementStore.SetFlagItems(tempListItem);
                    }
                });
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private async Task<(bool Success, string Message)> LoadSpecialCharacters(string LanguageCode)
        {
            string speCharPath = Path.Combine(_env.DataFolderPath, $@"CardData\Language\{LanguageCode}\SpecialCharacters.csv");
            if (!File.Exists(speCharPath)) return (false, $"{CMess.fileNotExit.ToText()}: SpecialCharacters.txt");

            try
            {
                var tempList = new List<CharacterItem>();
                using (var stream = new FileStream(speCharPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    bool isFirstLine = true;

                    while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
                    {
                        line = line.Trim();
                        if (string.IsNullOrEmpty(line)) continue;
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }
                        var parts = line.Split(',');
                        if (parts.Length >= 3)
                        {
                            tempList.Add(new CharacterItem
                            {
                                Character = parts[0].Trim(),
                                Category = parts[1].Trim(),
                                Description = parts[2].Trim()
                            });
                        }
                    }
                }
                lock (_lockObject)
                {
                    _cardElementStore.SetSpecialCharacters(tempList);
                }
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

    }
}
