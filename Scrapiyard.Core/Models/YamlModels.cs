namespace Scrapiyard.Core.Models;

// YAML RAW MODELS  
// Deserialize từ file *.yml
// Naming convention: snake_case theo YAML spec
public sealed class FunctionYaml
{
    public string? name { get; set; }
    public string? @namespace { get; set; }
    public string? description { get; set; }
    public string? summary { get; set; }
    public List<YamlParameter>? parameters { get; set; }
    public List<YamlOverload>? overloads { get; set; }
    public List<YamlReturn>? returns { get; set; }
    public Dictionary<string, string>? status { get; set; }
}

public sealed class TypeYaml
{
    public string name { get; set; } = string.Empty;
    public string summary { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public List<string> tags { get; set; } = new();
    public Dictionary<string, string>? status { get; set; }
    public string supertype { get; set; } = string.Empty;
    public List<TypeParameter>? parameters { get; set; }
    public List<TypeValue>? values { get; set; }
    public List<SuggestedLink>? suggestedLinks { get; set; }
    public string guide { get; set; } = string.Empty;
    public List<TypeReturn>? returns { get; set; }
}

public sealed class TypeParameter
{
    public string? name { get; set; }
    public List<string>? type { get; set; }
    public string? description { get; set; }
    public List<string> constraints { get; set; } = new();
}

public sealed class TypeValue
{
    public List<string> types { get; set; } = new();
    public string description { get; set; } = string.Empty;
}

public sealed class TypeReturn
{
    public List<string>? type { get; set; }
    public string? description { get; set; }
}

public sealed class YamlParameter
{
    public string? name { get; set; }
    public List<string>? type { get; set; }
    public string? description { get; set; }
    public bool? required { get; set; }
    public string? defaultValue { get; set; }
}

public sealed class YamlReturn
{
    public string? name { get; set; }
    public List<string>? type { get; set; }
    public string? description { get; set; }
}

public sealed class YamlOverload
{
    public string? description { get; set; }
    public List<YamlParameter>? parameters { get; set; }
}

public sealed class ConstantYaml
{
    public string? name { get; set; }
    public string? @enum { get; set; }
    public string? value { get; set; }
    public string? description { get; set; }
    public string? summary { get; set; }
    public Dictionary<string, string>? status { get; set; }
}

public sealed class EnumYaml
{
    public string? name { get; set; }
    public string? description { get; set; }
    public string? summary { get; set; }
    public bool bitmaskInt { get; set; }
    public List<string>? tags { get; set; }
}

public sealed class NameSpaceYaml
{
    public string? name { get; set; }
    public string? description { get; set; }
    public string? summary { get; set; }
    public Dictionary<string, string>? status { get; set; }
}

public sealed class TagYaml
{
    public string? name { get; set; }
    public string? description { get; set; }
    public string? summary { get; set; }
    public List<SuggestedLink>? suggestedLinks { get; set; }
}
