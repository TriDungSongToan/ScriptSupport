using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Character.Core.Models
{
    public class CharacterItem
    {
        public string Character { get; set; } = string.Empty;
        public CharacterMetadata Metadata { get; set; } = new CharacterMetadata();
        public string Description { get; set; } = string.Empty;
    }
    public class CharacterDataFile
    {
        public int Version { get; set; } = 1;
        public List<CharacterItem> Items { get; set; } = new();
    }
    public class CharacterFilter
    {
        public string? SearchText { get; set; } // match Description
        public CharacterGroup? Group { get; set; }
        public string? SubCategory { get; set; }
        public List<string> Tags { get; set; } = new(); // filter tag name
    }
}
