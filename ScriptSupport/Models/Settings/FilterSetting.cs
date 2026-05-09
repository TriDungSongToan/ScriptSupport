using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ScriptSupport.Models.Settings
{
    public class FilterSetting : INotifyPropertyChanged
    {
        private int? _maxItems = 500;
        public int? MaxItems
        {
            get => _maxItems;
            set
            {
                if (_maxItems != value)
                {
                    _maxItems = value;
                    OnPropertyChanged(nameof(MaxItems));
                    OnPropertyChanged(nameof(MaxItemsText));
                }
            }
        }
        [JsonIgnore]
        public string MaxItemsText
        {
            get => _maxItems switch
            {
                null => string.Empty,
                _ => _maxItems.ToString()!
            };
            set
            {
                if (string.IsNullOrWhiteSpace(value)) MaxItems = null;
                else if (int.TryParse(value, out int result)) MaxItems = result;
                else MaxItems = 500;
            }
        }
        private bool _advanced = true;
        public bool Advanced
        {
            get => _advanced;
            set
            {
                if (_advanced != value)
                {
                    _advanced = value;
                    OnPropertyChanged(nameof(Advanced));
                }
            }
        }
        private bool _matchCase = false;
        public bool MatchCase
        {
            get => _matchCase;
            set
            {
                if (_matchCase != value)
                {
                    _matchCase = value;
                    OnPropertyChanged(nameof(MatchCase));
                }
            }
        }
        private bool _wildcards = true;
        public bool Wildcards
        {
            get => _wildcards;
            set
            {
                if (_wildcards != value)
                {
                    _wildcards = value;
                    OnPropertyChanged(nameof(Wildcards));
                }
            }
        }
        private bool _prefix = true;
        public bool Prefix
        {
            get => _prefix;
            set
            {
                if (_prefix != value)
                {
                    _prefix = value;
                    OnPropertyChanged(nameof(Prefix));
                }
            }
        }
        private bool _suffix = false;
        public bool Suffix
        {
            get => _suffix;
            set
            {
                if (_suffix != value)
                {
                    _suffix = value;
                    OnPropertyChanged(nameof(Suffix));
                }
            }
        }
        private bool _matchWhole = false;
        public bool MatchWhole
        {
            get => _matchWhole;
            set
            {
                if (_matchWhole != value)
                {
                    _matchWhole = value;
                    OnPropertyChanged(nameof(MatchWhole));
                }
            }
        }
        private bool _ignpunct = false;
        public bool Ignpunct
        {
            get => _ignpunct;
            set
            {
                if (_ignpunct != value)
                {
                    _ignpunct = value;
                    OnPropertyChanged(nameof(Ignpunct));
                }
            }
        }
        private bool _ignspace = false;
        public bool Ignpspace
        {
            get => _ignspace;
            set
            {
                if (_ignspace != value)
                {
                    _ignspace = value;
                    OnPropertyChanged(nameof(Ignpspace));
                }
            }
        }

        public FilterSetting Clone()
        {
            var  json = JsonSerializer.Serialize(this);
            return JsonSerializer.Deserialize<FilterSetting>(json)!;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}