namespace Scrapiyard.Core.Models;

/// <summary>
/// Operation Result of convert/load.
/// </summary>
public sealed class OperationResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public int CountSuccesses { get; init; }
    public int CountSkipped { get; init; }
    public int CountErrors { get; init; }
    public IReadOnlyList<string> FailedFiles { get; init; } = Array.Empty<string>();

    public static OperationResult Ok(int successes, int skipped, int errors, IReadOnlyList<string>? failedFiles = null) => new()
    {
        Success = true,
        CountSuccesses = successes,
        CountSkipped = skipped,
        CountErrors = errors,
        FailedFiles = failedFiles ?? Array.Empty<string>()
    };

    public static OperationResult Fail(string error) => new() { Success = false, Error = error };

    public override string ToString() => Success
        ? $"OK — Successes: {CountSuccesses}, Skipped: {CountSkipped}, Errors: {CountErrors}"
        : $"FAILED — {Error}";
}

/// <summary>
/// Kết quả của thao tác Load JSON → Object.
/// </summary>
public sealed class LoadResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<CompletionSymbol> Symbols { get; init; } = Array.Empty<CompletionSymbol>();
    public static LoadResult Ok(IReadOnlyList<CompletionSymbol> symbols) => new() { Success = true, Symbols = symbols };
    public static LoadResult Fail(string error) => new() { Success = false, Error = error, Symbols = Array.Empty<CompletionSymbol>() };
}
