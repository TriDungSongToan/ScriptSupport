using ScriptSupport.Models;

namespace ScriptSupport.Helpers
{
    public static class LineEndingExtensions
    {
        public static string ToLineString(this LineEnding ending) => ending switch
        {
            LineEnding.CRLF => "\r\n",
            LineEnding.LF => "\n",
            LineEnding.CR => "\r",
            _ => "\r\n"
        };

        // Detect từ nội dung file khi load
        public static LineEnding Detect(string text)
        {
            if (text.Contains("\r\n")) return LineEnding.CRLF;
            if (text.Contains("\n")) return LineEnding.LF;
            if (text.Contains("\r")) return LineEnding.CR;
            return LineEnding.CRLF; // default
        }
    }
}
