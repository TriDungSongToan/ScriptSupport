using System.Text.Json.Serialization;

namespace Character.Core.Models
{
    public class TagItem
    {
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public bool IsEnabled { get; set; } = true;
        public override string ToString()
        {
            return Name;
        }
    }
}
