using System.IO;
using System.Text.RegularExpressions;
using ScriptSupport.Models;

namespace ScriptSupport.Helpers
{
    public static class StringHelper
    {
        public static ulong? GetCardIDFromScript(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath)) return null;
            string fileName = Path.GetFileName(fullPath);

            var match = Regex.Match(fileName, @"^c(\d+).*\.lua$", RegexOptions.IgnoreCase);
            if (!match.Success) return null;
            if (!ulong.TryParse(match.Groups[1].Value, out ulong id)) return null;

            return id;
        }
        public static LinkType DetectURL(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return LinkType.Unknown;

            input = input.Trim();

            // 1. External
            if (Uri.TryCreate(input, UriKind.Absolute, out var uri))
            {
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                    return LinkType.External;
            }

            // 2. API
            if (input.StartsWith("/api/", StringComparison.OrdinalIgnoreCase)) return LinkType.Api;

            // 3. Internal
            if (input.StartsWith("/")) return LinkType.Internal;

            // 4. Fallback
            return LinkType.Unknown;
        }
    }
}
