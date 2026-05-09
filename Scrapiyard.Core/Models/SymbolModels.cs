using System.Text.Json.Serialization;
namespace Scrapiyard.Core.Models;

// CORE SYMBOL
public sealed class CompletionSymbol
{
    // Immutable data (từ JSON)
    public SymbolKind Kind { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Namespace { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Supertype { get; init; } = string.Empty;
    public List<SuggestedLink> Links { get; init; } = new();
    public string OwnerEnum { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public TypeYaml? DeclaredType { get; init; }
    public List<SymbolOverload> Overloads { get; init; } = new();
    public List<string> Tags { get; init; } = new();
    public bool IsBitmask { get; init; } = false;
    public Dictionary<string, string>? Status { get; init; }

    // Semantic graph (mutable, không serialize)
    [JsonIgnore]
    public CompletionSymbol? Owner { get; internal set; }

    [JsonIgnore]
    public IReadOnlyList<CompletionSymbol> Members => _members;

    private readonly List<CompletionSymbol> _members = new();

    internal void AddMember(CompletionSymbol member) => _members.Add(member);
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SymbolKind
{
    Function,
    Constant,
    Enum,
    NameSpace,
    Tag,
    EnumMember,
    Keyword,
    Type,
    Variable
}

// OVERLOAD / PARAMETER / RETURN
public sealed class SymbolOverload
{
    public string? Description { get; init; }
    public IReadOnlyList<SymbolParameter> Parameters { get; init; } = Array.Empty<SymbolParameter>();
    public IReadOnlyList<SymbolReturn> Returns { get; init; } = Array.Empty<SymbolReturn>();
}

public sealed class SymbolParameter
{
    public string Name { get; init; } = string.Empty;
    public List<string> Types { get; init; } = new();
    public bool IsOptional { get; init; }
    public bool IsVariadic { get; init; }
    public string? Description { get; init; }
}

public sealed class SymbolReturn
{
    public string? Name { get; init; }
    public List<string> Types { get; init; } = new();
    public string? Description { get; init; }
}

// SHARED

public sealed class SuggestedLink
{
    public string Name { get; init; } = string.Empty;
    public string Link { get; init; } = string.Empty;
}

