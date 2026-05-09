using Scrapiyard.Core.Models;
using System.Reflection;
using System.Text;

namespace Scrapiyard.Core.Services;

/// <summary>
/// Build chuỗi mô tả cho một CompletionSymbol.
/// Trả về plain string — platform tự quyết định hiển thị.
/// </summary>
public static class SymbolDescriptionBuilder
{
    public static string Build(CompletionSymbol symbol)
    {
        if (symbol == null) return string.Empty;

        return symbol.Kind switch
        {
            SymbolKind.Function => BuildFunction(symbol),
            SymbolKind.Constant => BuildConstant(symbol),
            SymbolKind.Enum => BuildEnum(symbol),
            SymbolKind.NameSpace => BuildNameSpace(symbol),
            SymbolKind.Tag => BuildTag(symbol),
            SymbolKind.Type => BuildType(symbol),
            _ => BuildConstant(symbol),
        };
    }

    // ==========================================
    // PER-KIND BUILDERS
    // ==========================================

    private static string BuildFunction(CompletionSymbol s)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{s.Kind} {s.Namespace}.{s.Name}");
        sb.AppendLine();

        var Signatures = BuildSignatures(s);
        foreach (var signature in Signatures)
        {
            sb.AppendLine(signature);
        }

        if (!string.IsNullOrWhiteSpace(s.Description)) sb.AppendLine($"Description: {s.Description}");
        if (!string.IsNullOrWhiteSpace(s.Summary)) sb.AppendLine($"Summary: {s.Summary}");
        if (s.Status != null && s.Status.Count > 0)
        {
            sb.AppendLine("Status:");
            foreach (var statu in s.Status)
            {
                sb.AppendLine($"{statu.Key}: {statu.Value}");
            }
        }
        return sb.ToString();
    }

    private static string BuildConstant(CompletionSymbol s)
    {
        if (s == null) return string.Empty;

        var sb = new StringBuilder();

        sb.AppendLine($"{s.Name}");
        sb.AppendLine($"Enum: {s.OwnerEnum}");
        sb.AppendLine($"Value: {s.Value}");
        sb.AppendLine($"{s.Description}");
        sb.AppendLine($"Summary: {s.Summary}");

        if (s.Status != null && s.Status.Count > 0)
        {
            sb.AppendLine("Status:");
            foreach (var statu in s.Status)
            {
                sb.AppendLine($"{statu.Key}: {statu.Value}");
            }
        }
        return sb.ToString();
    }

    private static string BuildEnum(CompletionSymbol s)
    {
        if (s == null) return string.Empty;

        var sb = new StringBuilder();

        sb.AppendLine($"{s.Name}");
        sb.AppendLine($"{s.Description}");
        sb.AppendLine($"Summary: {s.Summary}");
        if (s.Tags != null && s.Tags.Count > 0)
        {
            sb.AppendLine($"Tags: {string.Join(", ", s.Tags)}");
        }
        if (s.Status != null && s.Status.Count > 0)
        {
            sb.AppendLine("Status:");
            foreach (var statu in s.Status)
            {
                sb.AppendLine($"{statu.Key}: {statu.Value}");
            }
        }

        return sb.ToString();
    }

    private static string BuildNameSpace(CompletionSymbol s)
    {
        if (s == null) return string.Empty;

        var sb = new StringBuilder();

        sb.AppendLine($"{s.Name}");
        sb.AppendLine($"{s.Description}");
        sb.AppendLine($"Summary: {s.Summary}");

        if (s.Status != null && s.Status.Count > 0)
        {
            sb.AppendLine("Status:");
            foreach (var statu in s.Status)
            {
                sb.AppendLine($"{statu.Key}: {statu.Value}");
            }
        }
        if (s.Tags != null && s.Tags.Count > 0)
        {
            sb.AppendLine($"Tags: {string.Join(", ", s.Tags)}");
        }

        return sb.ToString();
    }

    private static string BuildTag(CompletionSymbol s)
    {
        if (s == null) return string.Empty;

        var sb = new StringBuilder();

        sb.AppendLine(s.Name);
        sb.AppendLine(s.Description);
        sb.AppendLine($"Summary: {s.Summary}");

        if (s.Links != null && s.Links.Count > 0)
        {
            sb.AppendLine("Suggested Links:");
            foreach (var link in s.Links)
                sb.AppendLine($"{link.Name}: {link.Link}");
        }

        return sb.ToString();
    }

    private static string BuildType(CompletionSymbol s)
    {
        if (s == null) return string.Empty;

        var sb = new StringBuilder();

        sb.AppendLine($"{s.Name}");
        sb.AppendLine(BuildTypeSignature(s));
        sb.AppendLine();
        if (!string.IsNullOrWhiteSpace(s.Description)) sb.AppendLine(s.Description);
        if (!string.IsNullOrWhiteSpace(s.Summary)) sb.AppendLine(s.Summary);
        if (!string.IsNullOrWhiteSpace(s.Supertype)) sb.AppendLine($"Super Type: {s.Supertype}");

        if (s.Status != null && s.Status.Count > 0)
        {
            sb.AppendLine();
            foreach (var kv in s.Status)
            {
                sb.AppendLine($"{kv.Key}: {kv.Value}");
            }
        }
        if (s.Tags != null && s.Tags.Count > 0)
        {
            sb.AppendLine($"Tags: {string.Join(", ", s.Tags)}");
        }
        return sb.ToString();
    }

    // ==========================================
    // SHARED HELPERS
    // ==========================================

    private static void AppendStatus(StringBuilder sb, CompletionSymbol s)
    {
        if (s.Status == null || s.Status.Count == 0) return;
        sb.AppendLine("Status:");
        foreach (var kv in s.Status)
            sb.AppendLine($"  {kv.Key}: {kv.Value}");
    }

    private static IReadOnlyList<string> BuildSignatures(CompletionSymbol s)
    {
        if (s.Kind != SymbolKind.Function || s.Overloads == null) return Array.Empty<string>();
        return s.Overloads.Select(o => FormatSignature($"{s.Namespace}.{s.Name}", o)).ToList();
    }

    private static string FormatSignature(string fullName, SymbolOverload o)
    {
        var returnType = FormatReturnType(o);
        var parameters = o.Parameters == null || o.Parameters.Count == 0
            ? string.Empty
            : string.Join(", ", o.Parameters.Select(p => p.IsOptional ? $"[{p.Name}]" : p.Name));
        return $"{returnType} {fullName}({parameters});";
    }

    private static string FormatReturnType(SymbolOverload o)
    {
        if (o.Returns == null || o.Returns.Count == 0) return "void";
        var allTypes = o.Returns.SelectMany(r => r.Types).ToList();
        if (allTypes.Count == 0) return "void";
        if (allTypes.Count == 1) return allTypes[0];
        return $"({string.Join(", ", allTypes)})";
    }

    private static string BuildTypeSignature(CompletionSymbol s)
    {
        var name = s.Name;
        var parameters = s.DeclaredType?.parameters;

        if (parameters != null && parameters.Count > 0)
        {
            var paramText = string.Join(", ", parameters.Select(FormatTypeParameter));
            name += $"({paramText})";
        }

        if (!string.IsNullOrWhiteSpace(s.Supertype)) name += $" : {s.Supertype}";

        return name;
    }

    private static string FormatTypeParameter(TypeParameter p)
    {
        var type = p.type == null || p.type.Count == 0 ? "any" : string.Join(" | ", p.type);
        return $"{p.name}: {type}";
    }
}
