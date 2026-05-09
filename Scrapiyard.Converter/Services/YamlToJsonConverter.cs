using System.Text.Json;
using Scrapiyard.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Scrapiyard.Converter.Services;

/// <summary>
/// Đọc thư mục chứa file *.yml → Deserialize → Serialize ra file JSON.
/// </summary>
public static class YamlToJsonConverter
{
    private static readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    // ==========================================
    // PUBLIC API
    // ==========================================

    /// <summary>
    /// Convert tất cả file Constant YAML trong folder → JSON.
    /// </summary>
    /// <param name="sourceFolder">Thư mục chứa file *.yml.</param>
    /// <param name="outputFile">Đường dẫn file JSON đầu ra.</param>
    public static OperationResult ConvertConstants(string sourceFolder, string outputFile)
        => Convert<ConstantYaml>(
            sourceFolder, outputFile,
            tag: "---!constant",
            isValid: y => !string.IsNullOrEmpty(y.name),
            map: SymbolMapper.FromConstant);

    /// <summary>
    /// Convert tất cả file Enum YAML trong folder → JSON.
    /// </summary>
    public static OperationResult ConvertEnums(string sourceFolder, string outputFile)
        => Convert<EnumYaml>(
            sourceFolder, outputFile,
            tag: "---!enum",
            isValid: y => !string.IsNullOrEmpty(y.name),
            map: SymbolMapper.FromEnum);

    /// <summary>
    /// Convert tất cả file Function YAML trong folder → JSON.
    /// </summary>
    public static OperationResult ConvertFunctions(string sourceFolder, string outputFile)
        => Convert<FunctionYaml>(
            sourceFolder, outputFile,
            tag: "---!function",
            isValid: y => !string.IsNullOrEmpty(y.name),
            map: SymbolMapper.FromFunction);

    /// <summary>
    /// Convert tất cả file Namespace YAML trong folder → JSON.
    /// </summary>
    public static OperationResult ConvertNameSpaces(string sourceFolder, string outputFile)
        => Convert<NameSpaceYaml>(
            sourceFolder, outputFile,
            tag: "---!namespace",
            isValid: y => !string.IsNullOrEmpty(y.name),
            map: SymbolMapper.FromNameSpace);

    /// <summary>
    /// Convert tất cả file Tag YAML trong folder → JSON.
    /// </summary>
    public static OperationResult ConvertTags(string sourceFolder, string outputFile)
        => Convert<TagYaml>(
            sourceFolder, outputFile,
            tag: "---!tag",
            isValid: y => !string.IsNullOrEmpty(y.name),
            map: SymbolMapper.FromTag);

    /// <summary>
    /// Convert tất cả file Type YAML trong folder → JSON.
    /// </summary>
    public static OperationResult ConvertTypes(string sourceFolder, string outputFile)
        => Convert<TypeYaml>(
            sourceFolder, outputFile,
            tag: "---!type",
            isValid: y => !string.IsNullOrWhiteSpace(y.name),
            map: SymbolMapper.FromType);

    // ==========================================
    // PRIVATE CORE
    // ==========================================

    private static OperationResult Convert<TYaml>(
        string sourceFolder,
        string outputFile,
        string tag,
        Func<TYaml, bool> isValid,
        Func<TYaml, CompletionSymbol> map)
    {
        if (string.IsNullOrWhiteSpace(sourceFolder))
            return OperationResult.Fail("sourceFolder is null or empty.");

        if (!Directory.Exists(sourceFolder))
            return OperationResult.Fail($"Folder not found: {sourceFolder}");

        if (string.IsNullOrWhiteSpace(outputFile))
            return OperationResult.Fail("outputFile is null or empty.");

        int countSuccesses = 0, countSkipped = 0, countErrors = 0;
        var symbols = new List<CompletionSymbol>();
        var failedFiles = new List<string>();

        try
        {
            foreach (var file in Directory.GetFiles(sourceFolder, "*.yml", SearchOption.AllDirectories))
            {
                string text = File.ReadAllText(file);

                // Kiểm tra tag đầu file (ví dụ: "---!constant")
                if (!text.StartsWith(tag, StringComparison.OrdinalIgnoreCase))
                {
                    countSkipped++;
                    continue;
                }

                // Bỏ dòng tag đầu tiên
                int newlineIdx = text.IndexOf('\n');
                if (newlineIdx > 0) text = text[(newlineIdx + 1)..];

                TYaml yaml;
                try
                {
                    yaml = _deserializer.Deserialize<TYaml>(text);
                }
                catch (Exception ex)
                {
                    countErrors++;
                    failedFiles.Add($"{file} | {ex.Message}");
                    continue;
                }

                if (yaml == null || !isValid(yaml))
                {
                    countSkipped++;
                    continue;
                }

                symbols.Add(map(yaml));
                countSuccesses++;
            }

            // Đảm bảo thư mục output tồn tại
            var outputDir = Path.GetDirectoryName(outputFile);
            if (!string.IsNullOrEmpty(outputDir))
                Directory.CreateDirectory(outputDir);

            var json = JsonSerializer.Serialize(symbols, _jsonOptions);
            File.WriteAllText(outputFile, json);

            return OperationResult.Ok(countSuccesses, countSkipped, countErrors, failedFiles);
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}
