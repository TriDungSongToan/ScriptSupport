using ScriptSupport.Helpers;
using ScriptSupport.Services;
using ScriptSupport.Stores;
using ScriptSupport.ViewModels;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.Models
{
    [Serializable]
    public class CardData
    {
        public ulong id { get; set; }
        public ulong ot { get; set; } = 0;
        public ulong alias { get; set; } = 0;
        public ulong setcode { get; set; } = 0;
        public ulong type { get; set; } = 0;
        public long atk { get; set; } = 0;
        public long def { get; set; } = 0;
        public ulong level { get; set; } = 0;
        public ulong race { get; set; } = 0;
        public ulong attribute { get; set; } = 0;
        public ulong category { get; set; } = 0;
        public ulong flag { get; set; } = 0;
        public ulong rarity { get; set; } = 0;

        public void UpdateFrom(CardData src, params Action<CardData, CardData>[] updaters)
        {
            if (src == null || updaters == null) return;

            foreach (var u in updaters)
                u(this, src);
        }
        public void UpdateFrom(CardData src, IEnumerable<Action<CardData, CardData>> updaters)
        {
            if (src == null || updaters == null) return;
            foreach (var u in updaters)
                u(this, src);
        }
    }
    public class CardDataFilter : BaseViewModel
    {
        private ulong? _id = null;
        public ulong? Id
        {
            get => _id;
            set
            {
                if (SetProperty(ref _id, value))
                    OnPropertyChanged(nameof(IdText));
            }
        }
        public string IdText
        {
            get => _id switch
            {
                null => string.Empty,
                0 => "0",
                var v => v.ToString()!
            };
            set => Id = GetInfoHelper.ParseProperty<ulong>(value);
        }
        private ulong _ot = 0;
        public ulong ot
        {
            get => _ot;
            set => SetProperty(ref _ot, value);
        }
        private ulong? _alias = null;
        public ulong? Alias
        {
            get => _alias;
            set
            {
                if (SetProperty(ref _alias, value))
                    OnPropertyChanged(nameof(AliasText));
            }
        }
        public string AliasText
        {
            get => _alias switch
            {
                null => string.Empty,
                0 => "0",
                var v => v.ToString()!
            };
            set => Alias = GetInfoHelper.ParseProperty<ulong>(value);
        }
        private ulong _setCode = 0;
        public ulong SetCode
        {
            get => _setCode;
            set => SetProperty(ref _setCode, value);
        }
        private ulong _type = 0;
        public ulong Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        private long? _atk = null;
        public long? Atk
        {
            get => _atk;
            set
            {
                if (SetProperty(ref _atk, value))
                    OnPropertyChanged(nameof(AtkText));
            }
        }
        public string AtkText
        {
            get => _atk switch
            {
                null => string.Empty,
                < 0 => "?",
                var v => v.ToString()!
            };
            set => Atk = GetInfoHelper.ParsePowerProperty(value);
        }
        private long? _def = null;
        public long? Def
        {
            get => _def;
            set
            {
                if (SetProperty(ref _def, value))
                    OnPropertyChanged(nameof(DefText));
            }
        }
        public string DefText
        {
            get => _def switch
            {
                null => string.Empty,
                < 0 => "?",
                var v => v.ToString()!
            };
            set => Def = GetInfoHelper.ParsePowerProperty(value);
        }
        private long _linkMaker = 0;
        public long LinkMaker
        {
            get => _linkMaker;
            set => SetProperty(ref _linkMaker, value);
        }
        private int? _level = null;
        public int? Level
        {
            get => _level;
            set
            {
                if (SetProperty(ref _level, value))
                    OnPropertyChanged(nameof(LevelText));
            }
        }
        public string LevelText
        {
            get => _level switch
            {
                null => string.Empty,
                0 => "0",
                var v => v.ToString()!
            };
            set => Level = GetInfoHelper.ParseProperty<int>(value);
        }
        private int? _linkRating = null;
        public int? LinkRating
        {
            get => _linkRating;
            set
            {
                if (SetProperty(ref _linkRating, value))
                    OnPropertyChanged(nameof(LinkRatingText));
            }
        }
        public string LinkRatingText
        {
            get => _linkRating switch
            {
                null => string.Empty,
                0 => "0",
                var v => v.ToString()!
            };
            set => LinkRating = GetInfoHelper.ParseProperty<int>(value);
        }
        private int? _leftScale = null;
        public int? LeftScale
        {
            get => _leftScale;
            set
            {
                if (SetProperty(ref _leftScale, value))
                    OnPropertyChanged(nameof(LeftScaleText));
            }
        }
        public string LeftScaleText
        {
            get => _leftScale switch
            {
                null => string.Empty,
                0 => "0",
                var v => v.ToString()!
            };
            set => LeftScale = GetInfoHelper.ParseProperty<int>(value);
        }
        private int? _rightScale = null;
        public int? RightScale
        {
            get => _rightScale;
            set
            {
                if (SetProperty(ref _rightScale, value))
                    OnPropertyChanged(nameof(RightScaleText));
            }
        }
        public string RightScaleText
        {
            get => _rightScale switch
            {
                null => string.Empty,
                0 => "0",
                var v => v.ToString()!
            };
            set => RightScale = GetInfoHelper.ParseProperty<int>(value);
        }
        private ulong _race = 0;
        public ulong Race
        {
            get => _race;
            set => SetProperty(ref _race, value);
        }
        private ulong _attribute = 0;
        public ulong Attribute
        {
            get => _attribute;
            set => SetProperty(ref _attribute, value);
        }
        private ulong _category = 0;
        public ulong Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }
        private ulong _flag = 0;
        public ulong Flag
        {
            get => _flag;
            set => SetProperty(ref _flag, value);
        }
        private ulong _rarity = 0;
        public ulong Rarity
        {
            get => _rarity;
            set => SetProperty(ref _rarity, value);
        }

        private int? _gPoint = null;
        public int? GPoint
        {
            get => _gPoint;
            set
            {
                if (SetProperty(ref _gPoint, value))
                    OnPropertyChanged(nameof(GPointText));
            }
        }
        public string GPointText
        {
            get => _gPoint switch
            {
                null => string.Empty,
                0 => "0",
                var v => v.ToString()!
            };
            set => GPoint = GetInfoHelper.ParseProperty<int>(value);
        }

        public bool IsDefault()
        {
            var d = new CardDataFilter();
            return Id == d.Id &&
                ot == d.ot &&
                Alias == d.Alias &&
                SetCode == d.SetCode &&
                Type == d.Type &&
                Atk == d.Atk &&
                Def == d.Def &&
                LinkMaker == d.LinkMaker &&
                Level == d.Level &&
                LinkRating == d.LinkRating &&
                LeftScale == d.LeftScale &&
                RightScale == d.RightScale &&
                Race == d.Race &&
                Attribute == d.Attribute &&
                Category == d.Category &&
                Flag == d.Flag &&
                Rarity == d.Rarity &&
                GPoint == d.GPoint;
        }
    }
    [Serializable]
    public class CardText
    {
        public ulong id { get; set; }
        public string name { get; set; } = string.Empty;
        public string desc { get; set; } = string.Empty;
        public string str1 { get; set; } = string.Empty;
        public string str2 { get; set; } = string.Empty;
        public string str3 { get; set; } = string.Empty;
        public string str4 { get; set; } = string.Empty;
        public string str5 { get; set; } = string.Empty;
        public string str6 { get; set; } = string.Empty;
        public string str7 { get; set; } = string.Empty;
        public string str8 { get; set; } = string.Empty;
        public string str9 { get; set; } = string.Empty;
        public string str10 { get; set; } = string.Empty;
        public string str11 { get; set; } = string.Empty;
        public string str12 { get; set; } = string.Empty;
        public string str13 { get; set; } = string.Empty;
        public string str14 { get; set; } = string.Empty;
        public string str15 { get; set; } = string.Empty;
        public string str16 { get; set; } = string.Empty;
        public string DBPath { get; set; } = string.Empty;

        public void UpdateFrom(CardText src, params Action<CardText, CardText>[] updaters)
        {
            if (src == null || updaters == null) return;

            foreach (var u in updaters)
                u(this, src);
        }
        public void UpdateFrom(CardText src, IEnumerable<Action<CardText, CardText>> updaters)
        {
            if (src == null || updaters == null) return;
            foreach (var u in updaters)
                u(this, src);
        }
    }
    public class CardTextFilter : BaseViewModel
    {
        private ulong? _id = null;
        public ulong? Id
        {
            get => _id;
            set
            {
                if (SetProperty(ref _id, value))
                    OnPropertyChanged(nameof(IdText));
            }
        }
        public string IdText
        {
            get => _id switch
            {
                null => string.Empty,
                0 => "0",
                var v => v.ToString()!
            };
            set => Id = GetInfoHelper.ParseProperty<ulong>(value);
        }
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _desc = string.Empty;
        public string Desc
        {
            get => _desc;
            set => SetProperty(ref _desc, value);
        }
        private string _str = string.Empty;
        public string Str
        {
            get => _str;
            set => SetProperty(ref _str, value);
        }
    }
    public class CardInformation : BaseViewModel
    {
        private CardElementStore _store; 

        private string _cardName = string.Empty;
        public string CardName
        {
            get => _cardName;
            set => SetProperty(ref _cardName, value);
        }
        private string _cardDesc = string.Empty;
        public string CardDesc
        {
            get => _cardDesc;
            set => SetProperty(ref _cardDesc, value);
        }
        private string _cardType = string.Empty;
        public string CardType
        {
            get => _cardType;
            set => SetProperty(ref _cardType, value);
        }
        private string _setCode = string.Empty;
        public string SetCode
        {
            get => _setCode;
            set => SetProperty(ref _setCode, value);
        }
        private string _attribute = string.Empty;
        public string Attribute
        {
            get => _attribute;
            set => SetProperty(ref _attribute, value);
        }
        private string _raceLabel = string.Empty;
        public string RaceLabel
        {
            get => _raceLabel;
            set => SetProperty(ref _raceLabel, value);
        }
        private string _race = string.Empty;
        public string Race
        {
            get => _race;
            set => SetProperty(ref _race, value);
        }
        private string _rarity = string.Empty;
        public string Rarity
        {
            get => _rarity;
            set => SetProperty(ref _rarity, value);
        }

        private string _levelLabel = string.Empty;
        public string LevelLabel
        {
            get => _levelLabel;
            set => SetProperty(ref _levelLabel, value);
        }
        private string _level = string.Empty;
        public string Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }
        private string _linkRating = string.Empty;
        public string LinkRating
        {
            get => _linkRating;
            set => SetProperty(ref _linkRating, value);
        }
        private string _leftScale = string.Empty;
        public string LeftScale
        {
            get => _leftScale;
            set => SetProperty(ref _leftScale, value);
        }
        private string _rightScale = string.Empty;
        public string RightScale
        {
            get => _rightScale;
            set => SetProperty(ref _rightScale, value);
        }
        private string _atk = string.Empty;
        public string Atk
        {
            get => _atk;
            set => SetProperty(ref _atk, value);
        }
        private string _def = string.Empty;
        public string Def
        {
            get => _def;
            set => SetProperty(ref _def, value);
        }
        private bool _hasAtk = true;
        public bool HasAtk
        {
            get => _hasAtk;
            set => SetProperty(ref _hasAtk, value);
        }
        private bool _hasDef = true;
        public bool HasDef
        {
            get => _hasDef;
            set => SetProperty(ref _hasDef, value);
        }
        private string _linkMarker = string.Empty;
        public string LinkMarker
        {
            get => _linkMarker;
            set => SetProperty(ref _linkMarker, value);
        }
        private string _cardId = string.Empty;
        public string CardId
        {
            get => _cardId;
            set => SetProperty(ref _cardId, value);
        }
        private string _cardAlias = string.Empty;
        public string CardAlias
        {
            get => _cardAlias;
            set => SetProperty(ref _cardAlias, value);
        }
        private string _cardRule = string.Empty;
        public string CardRule
        {
            get => _cardRule;
            set => SetProperty(ref _cardRule, value);
        }

        public CardInformation(CardElementStore store)
        {
            _store = store;
        }

        public void SetViewText(CardText? text)
        {
            if (text == null)
            {
                CardName = string.Empty;
                CardDesc = string.Empty;
                return;
            }

            CardName = text.name;
            CardDesc = text.desc;
        }
        public void SetViewData(CardData? data)
        {
            BeginUpdate();

            try
            {
                if (data == null)
                {
                    SetCode = string.Empty;
                    Attribute = string.Empty;
                    RaceLabel = "Race: ";
                    Race = string.Empty;
                    Rarity = string.Empty;
                    LevelLabel = "Level: ";
                    Level = string.Empty;
                    LinkRating = string.Empty;
                    LeftScale = string.Empty;
                    RightScale = string.Empty;
                    Atk = string.Empty;
                    Def = string.Empty;
                    HasAtk = false;
                    HasDef = false;
                    LinkMarker = string.Empty;
                    CardId = string.Empty;
                    CardAlias = string.Empty;
                    CardRule = string.Empty;
                    return;
                }

                CardItemInfo cardItemInfo = new CardItemInfo(data.type);

                CardType = _store.listtype.Any() ? FindInfoService.FindCardInfo(_store.listtype, data.type, separation: true) : string.Empty;
                SetCode = _store.listsetcode.Any() ? FindInfoService.FindSetcode(_store.listsetcode, data.setcode) : string.Empty;
                Attribute = _store.listattr.Any() ? FindInfoService.FindCardInfo(_store.listattr, data.attribute, separation: true) : string.Empty;
                RaceLabel = cardItemInfo.IsSkill ? CMess.cardLabelChar.ToText() : CMess.cardLabelRace.ToText();
                Race = cardItemInfo.IsSkill
                    ? (_store.listchar.Any() ? FindInfoService.FindCardInfo(_store.listchar, data.race, separation: true) : string.Empty)
                    : (_store.listrace.Any() ? FindInfoService.FindCardInfo(_store.listrace, data.race, separation: true) : string.Empty);
                Rarity = ""; // Hiện tại chưa có thông tin rarity trong DB, cần cập nhật sau khi có dữ liệu

                if (cardItemInfo.IsXyz && cardItemInfo.IsNonXyz) LevelLabel = $"{CMess.Level.ToText()}/{CMess.Rank.ToText()}";
                else if (cardItemInfo.IsXyz && !cardItemInfo.IsNonXyz) LevelLabel = CMess.Rank.ToText();
                else LevelLabel = CMess.Level.ToText();

                var (level, link, rightScale, leftScale) = FindInfoService.FindLevelPenScale(data.level, cardItemInfo.IsLink);
                Level = level >= 0 ? level.ToString() : "0";
                LinkRating = link >= 0 ? link.ToString() : "0";
                LeftScale = leftScale >= 0 ? leftScale.ToString() : "0";
                RightScale = rightScale >= 0 ? rightScale.ToString() : "0";

                if (cardItemInfo.IsLink)
                {
                    var cardPower = GetInfoHelper.DecodeDef(data.def);
                    HasAtk = (cardItemInfo.IsMonster && !cardPower.notHasATK);
                    HasDef = (cardItemInfo.IsMonster && cardPower.deffromtext.HasValue);
                    Atk = HasAtk ? (data.atk >= 0 ? data.atk.ToString() : "?") : string.Empty;
                    Def = HasDef ? (cardPower.deffromtext!.Value >= 0 ? cardPower.deffromtext!.Value.ToString() : "?") : string.Empty;
                    LinkMarker = _store.listlinkarrow.Any() ? FindInfoService.FindCardInfo(_store.listlinkarrow, (ulong)cardPower.linkarrow, separation: false) : string.Empty;
                }
                else
                {
                    HasAtk = cardItemInfo.IsMonster;
                    HasDef = cardItemInfo.IsMonster;
                    Atk = data.atk >= 0 ? data.atk.ToString() : "?";
                    Def = data.def >= 0 ? data.def.ToString() : "?";
                    LinkMarker = string.Empty;
                }

                CardId = data.id != 0 ? data.id.ToString() : string.Empty;
                CardAlias = data.alias != 0 ? data.alias.ToString() : string.Empty;

                CardRule = _store.listrule.Any() ? FindInfoService.FindCardInfo(_store.listrule, data.ot, separation: true) : string.Empty;
            }
            catch { }
            finally
            {
                EndUpdate();
            }
        }
    }
    public readonly struct CardItemInfo
    {
        public bool IsMonster { get; }
        public bool IsNormal { get; }
        public bool IsEffect { get; }
        public bool IsXyz { get; }
        public bool IsNonXyz { get; }
        public bool IsPendulum { get; }
        public bool IsLink { get; }
        public bool IsSkill { get; }
        public bool IsToken { get; }

        public CardItemInfo(ulong type)
        {
            IsMonster = FindInfoService.CheckCardInfo(type, CardType.Monster);
            IsNormal = FindInfoService.CheckCardInfo(type, CardType.Normal);
            IsEffect = FindInfoService.CheckCardInfo(type, CardType.Effect);
            IsXyz = FindInfoService.CheckCardInfo(type, CardType.eXceed);
            IsNonXyz = FindInfoService.CheckCardInfo(type, CardType.Fusion, CardType.Ritual, CardType.Synchro);
            IsPendulum = FindInfoService.CheckCardInfo(type, CardType.Pendulum);
            IsLink = FindInfoService.CheckCardInfo(type, CardType.Link);
            IsSkill = FindInfoService.CheckCardInfo(type, CardType.Skill);
            IsToken = FindInfoService.CheckCardInfo(type, CardType.Token);
        }
    }
}