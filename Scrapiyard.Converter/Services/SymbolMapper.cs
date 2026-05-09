using Scrapiyard.Core.Models;

namespace Scrapiyard.Converter.Services;

/// <summary>
/// Chuyển đổi các Yaml model → CompletionSymbol.
/// </summary>
internal static class SymbolMapper
{
    public static CompletionSymbol FromConstant(ConstantYaml yaml) => new()
    {
        Kind = SymbolKind.Constant,
        Name = yaml.name ?? string.Empty,
        Namespace = string.Empty,
        OwnerEnum = yaml.@enum ?? string.Empty,
        Value = yaml.value ?? string.Empty,
        Summary = yaml.summary ?? string.Empty,
        Description = yaml.description ?? string.Empty,
        Status = yaml.status
    };

    public static CompletionSymbol FromEnum(EnumYaml yaml) => new()
    {
        Kind = SymbolKind.Enum,
        Name = yaml.name ?? string.Empty,
        Namespace = string.Empty,
        Summary = yaml.summary ?? string.Empty,
        Description = yaml.description ?? string.Empty,
        IsBitmask = yaml.bitmaskInt,
        Tags = yaml.tags ?? new List<string>()
    };

    public static CompletionSymbol FromFunction(FunctionYaml yaml)
    {
        var overloads = new List<SymbolOverload> { BuildRootOverload(yaml) };

        if (yaml.overloads != null)
            overloads.AddRange(yaml.overloads.Select(ConvertOverload));

        return new CompletionSymbol
        {
            Kind = SymbolKind.Function,
            Name = yaml.name ?? string.Empty,
            Namespace = yaml.@namespace ?? string.Empty,
            Summary = yaml.summary ?? string.Empty,
            Description = yaml.description ?? string.Empty,
            Overloads = overloads,
            Status = yaml.status
        };
    }

    public static CompletionSymbol FromNameSpace(NameSpaceYaml yaml) => new()
    {
        Kind = SymbolKind.NameSpace,
        Name = yaml.name ?? string.Empty,
        Namespace = yaml.name ?? string.Empty,
        Summary = yaml.summary ?? string.Empty,
        Description = yaml.description ?? string.Empty,
        Status = yaml.status
    };

    public static CompletionSymbol FromTag(TagYaml yaml) => new()
    {
        Kind = SymbolKind.Tag,
        Name = yaml.name ?? string.Empty,
        Namespace = string.Empty,
        Summary = yaml.summary ?? string.Empty,
        Description = yaml.description ?? string.Empty,
        Links = yaml.suggestedLinks?.Select(l => new SuggestedLink
        {
            Name = l.Name,
            Link = l.Link
        }).ToList() ?? new List<SuggestedLink>()
    };

    public static CompletionSymbol FromType(TypeYaml yaml) => new()
    {
        Kind = SymbolKind.Type,
        Name = yaml.name,
        Namespace = string.Empty,
        Summary = yaml.summary,
        Description = yaml.description,
        Tags = yaml.tags ?? new List<string>(),
        Status = yaml.status,
        Supertype = yaml.supertype,
        Links = yaml.suggestedLinks?.Select(l => new SuggestedLink
        {
            Name = l.Name,
            Link = l.Link
        }).ToList() ?? new List<SuggestedLink>(),
        DeclaredType = yaml
    };

    // ==========================================
    // PRIVATE HELPERS
    // ==========================================

    private static SymbolOverload BuildRootOverload(FunctionYaml yaml) => new()
    {
        Description = yaml.description,
        Parameters = yaml.parameters?.Select(ConvertParameter).ToList()
                     ?? new List<SymbolParameter>(),
        Returns = ConvertReturns(yaml.returns)
    };

    private static SymbolOverload ConvertOverload(YamlOverload ov) => new()
    {
        Description = ov.description,
        Parameters = ov.parameters?.Select(ConvertParameter).ToList()
                     ?? new List<SymbolParameter>(),
        Returns = Array.Empty<SymbolReturn>()
    };

    private static SymbolParameter ConvertParameter(YamlParameter p) => new()
    {
        Name = p.name ?? string.Empty,
        Types = p.type ?? new List<string>(),
        IsOptional = p.required == false || p.defaultValue != null,
        IsVariadic = p.name == "...",
        Description = p.description
    };

    private static IReadOnlyList<SymbolReturn> ConvertReturns(List<YamlReturn>? yamlReturns)
    {
        if (yamlReturns == null || yamlReturns.Count == 0)
            return Array.Empty<SymbolReturn>();

        return yamlReturns.Select(r => new SymbolReturn
        {
            Types = r.type ?? new List<string>(),
            Description = r.description
        }).ToList();
    }
}
