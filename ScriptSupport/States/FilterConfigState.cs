using ScriptSupport.ViewModels;
using ScriptSupport.Models.Settings;

namespace ScriptSupport.States
{
    public class FilterConfigState : BaseViewModel
    {
        private int? _maxItems = 500;
        public int? MaxItems
        {
            get => _maxItems;
            set => SetProperty(ref _maxItems, value);
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

        public void SetValue(FilterSetting setting)
        {
            MaxItems = setting.MaxItems;
            Advanced = setting.Advanced;
            MatchCase = setting.MatchCase;
            Wildcards = setting.Wildcards;
            Prefix = setting.Prefix;
            Suffix = setting.Suffix;
            MatchWhole = setting.MatchWhole;
            Ignpunct = setting.Ignpunct;
            Ignpspace = setting.Ignpspace;
        }
    }
}
