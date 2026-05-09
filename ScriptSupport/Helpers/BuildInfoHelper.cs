using System.Reflection;

namespace ScriptSupport.Helpers
{
    public static class BuildInfoHelper
    {
        public static DateTime ReleaseDateUtc
        {
            get
            {
                var raw = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyMetadataAttribute>()?
                    .Value;

                if (!DateTime.TryParse(raw, out var dt))
                {
                    dt = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                }

                return dt.ToUniversalTime();
            }
        }
    }
}
