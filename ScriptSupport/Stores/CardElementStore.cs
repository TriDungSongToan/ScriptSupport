using ScriptSupport.Models;
using ScriptSupport.Collections;

namespace ScriptSupport.Stores
{
    public class CardElementStore : IDisposable
    {
        public BulkObservableCollection<RuleItem> RuleItems { get; set; }
        public BulkObservableCollection<TypeItem> TypeItems { get; set; }
        public BulkObservableCollection<RaceItem> RaceItems { get; set; }
        public BulkObservableCollection<CharItem> CharItems { get; set; }
        public BulkObservableCollection<AttributeItem> AttributeItems { get; set; }
        public BulkObservableCollection<SetCodeItem> SetCodeItems { get; set; }
        public BulkObservableCollection<CategoryItem> CategoryItems { get; set; }
        public BulkObservableCollection<FlagItem> FlagItems { get; set; }
        public BulkObservableCollection<LevelItem> LevelItems { get; set; }
        public BulkObservableCollection<LinkArrowItem> LinkArrowItems { get; set; }
        public BulkObservableCollection<CharacterItem> SpecialCharacters { get; set; }

        public List<(ulong bit, string name)> listrule { get; set; }
        public List<(ulong bit, string name)> listtype { get; set; }
        public List<(ulong bit, string name)> listtypedeck { get; set; }
        public List<(ulong bit, string name)> listrace { get; set; }
        public List<(ulong bit, string name)> listchar { get; set; }
        public List<(ulong bit, string name)> listattr { get; set; }
        public List<(ulong bit, string name)> listsetcode { get; set; }
        public List<(ulong bit, string name)> listlinkarrow { get; set; }

        public CardElementStore()
        {
            RuleItems = new BulkObservableCollection<RuleItem>();
            TypeItems = new BulkObservableCollection<TypeItem>();
            RaceItems = new BulkObservableCollection<RaceItem>();
            CharItems = new BulkObservableCollection<CharItem>();
            AttributeItems = new BulkObservableCollection<AttributeItem>();
            SetCodeItems = new BulkObservableCollection<SetCodeItem>();
            CategoryItems = new BulkObservableCollection<CategoryItem>();
            FlagItems = new BulkObservableCollection<FlagItem>();
            LevelItems = new BulkObservableCollection<LevelItem>();
            LinkArrowItems = new BulkObservableCollection<LinkArrowItem>();
            SpecialCharacters = new BulkObservableCollection<CharacterItem>();

            listrule = new List<(ulong bit, string name)>();
            listtype = new List<(ulong bit, string name)>();
            listtypedeck = new List<(ulong bit, string name)>();
            listrace = new List<(ulong bit, string name)>();
            listchar = new List<(ulong bit, string name)>();
            listattr = new List<(ulong bit, string name)>();
            listsetcode = new List<(ulong bit, string name)>();
            listlinkarrow = new List<(ulong bit, string name)>();
        }

        public void SetRuleItems(IEnumerable<RuleItem> items, IList<(ulong, string)> listItem)
        {
            RuleItems.ReplaceAll(items);
            listrule.Clear();
            listrule.AddRange(listItem);
        }
        public void SetTypeItems(IEnumerable<TypeItem> items, IList<(ulong, string)> listItem)
        {
            TypeItems.ReplaceAll(items);
            listtype.Clear();
            listtype.AddRange(listItem);
        }
        public void SetRaceItems(IEnumerable<RaceItem> items, IList<(ulong, string)> listItem)
        {
            RaceItems.ReplaceAll(items);
            listrace.Clear();
            listrace.AddRange(listItem);
        }
        public void SetCharItems(IEnumerable<CharItem> items, IList<(ulong, string)> listItem)
        {
            CharItems.ReplaceAll(items);
            listchar.Clear();
            listchar.AddRange(listItem);
        }
        public void SetAttributeItems(IEnumerable<AttributeItem> items, IList<(ulong, string)> listItem)
        {
            AttributeItems.ReplaceAll(items);
            listattr.Clear();
            listattr.AddRange(listItem);
        }
        public void SetSetCodeItems(IEnumerable<SetCodeItem> items, IList<(ulong, string)> listItem)
        {
            SetCodeItems.ReplaceAll(items);
            listsetcode.Clear();
            listsetcode.AddRange(listItem);
        }
        public void SetCategoryItems(IEnumerable<CategoryItem> items)
        {
            CategoryItems.ReplaceAll(items);
        }
        public void SetFlagItems(IEnumerable<FlagItem> items)
        {
            FlagItems.ReplaceAll(items);
        }
        public void SetLevelItems(IEnumerable<LevelItem> items)
        {
            LevelItems.ReplaceAll(items);
        }
        public void SetLinkArrowItems(IEnumerable<LinkArrowItem> items, IList<(ulong, string)> listItem)
        {
            LinkArrowItems.ReplaceAll(items);
            listlinkarrow.Clear();
            listlinkarrow.AddRange(listItem);
        }
        public void SetSpecialCharacters(IEnumerable<CharacterItem> items)
        {
            SpecialCharacters.ReplaceAll(items);
        }

        public void Dispose()
        {
            RuleItems = new();
            TypeItems = new();
            RaceItems = new();
            CharItems = new();
            AttributeItems = new();
            SetCodeItems = new();
            CategoryItems = new();
            FlagItems = new();
            LevelItems = new();
            LinkArrowItems = new();
            SpecialCharacters = new();
            listrule = new();
            listtype = new();
            listtypedeck = new();
            listrace = new();
            listchar = new();
            listattr = new();
            listsetcode = new();
            listlinkarrow = new();
        }
    }
}
