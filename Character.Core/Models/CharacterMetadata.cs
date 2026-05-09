namespace Character.Core.Models
{
    public class CharacterMetadata
    {
        public CharacterGroup Group { get; set; }
        public string SubCategory { get; set; } = string.Empty;
        public List<TagItem> Tags { get; set; } = new List<TagItem>();
    }
}
