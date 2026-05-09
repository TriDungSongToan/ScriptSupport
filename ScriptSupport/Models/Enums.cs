using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.Models
{
    #region Image
    public enum AppImage
    {
        Error,
        Information,
        Notification,
        Question,
        Warning,

        Logo,
        Blank,
        LevelStar,
        RankStar,
        LevelRankStar,
    }
    #endregion

    #region FlowDirection
    public enum FlowDirection
    {
        LeftToRight = 1,
        RightToLeft = 2
    }
    public static class FlowDirectionExtensions
    {
        public static string ToFriendlyString(this FlowDirection direction)
        {
            return direction switch
            {
                FlowDirection.LeftToRight => CMess.LeftToRight.ToText(),
                FlowDirection.RightToLeft => CMess.RightToLeft.ToText(),
                _ => CMess.LeftToRight.ToText()
            };
        }
    }
    #endregion

    #region TextAlignment
    public enum TextAlignment
    {
        Left = 1,
        Right = 2,
        Justify = 3,
        Center = 4,
    }
    public static class TextAlignmentExtensions
    {
        public static string ToFriendlyString(this TextAlignment alignment)
        {
            return alignment switch
            {
                TextAlignment.Left => CMess.AligLeft.ToText(),
                TextAlignment.Right => CMess.AligRight.ToText(),
                TextAlignment.Justify => CMess.AligJustify.ToText(),
                TextAlignment.Center => CMess.AligCenter.ToText(),

                _ => CMess.AligLeft.ToText()
            };
        }
    }

    #endregion

    #region StampPosition
    public enum StampPosition
    {
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 3,
        BottomRight = 4,
        Center = 5,
        Unknown = 6
    }
    public static class StampPositionExtensions
    {
        public static string ToFriendlyString(this StampPosition position)
        {
            return position switch
            {
                StampPosition.TopLeft => CMess.topLeft.ToText(),
                StampPosition.TopRight => CMess.topRight.ToText(),
                StampPosition.BottomLeft => CMess.bottomLeft.ToText(),
                StampPosition.BottomRight => CMess.bottomRight.ToText(),
                StampPosition.Center => CMess.center.ToText(),
                _ => CMess.unknown.ToText(),
            };
        }
    }
    #endregion

    #region Sort
    public enum SortType
    {
        ID = 0,
        NAME = 1,
        RULE = 2,
        ALIAS = 3,
        SETCODE = 4,
        TYPE = 5,
        ATK = 6,
        DEF = 7,
        LEVEL = 8,
        RACE = 9,
        ATTRIBUTE = 10,
        CATEGORY = 11,
        RARE = 12,
        GPOINT = 13
    }
    public static class SortExtensions
    {
        public static string ToFriendlyString(this SortType arrange)
        {
            return arrange switch
            {
                SortType.ID => CMess.cardID.ToText(),
                SortType.NAME => CMess.cardName.ToText(),
                SortType.RULE => CMess.cardLabelScope.ToText(),
                SortType.ALIAS => CMess.cardAlias.ToText(),
                SortType.SETCODE => CMess.cardlabelSetCode.ToText(),
                SortType.TYPE => CMess.cardLabelType.ToText(),
                SortType.ATK => CMess.cardatk.ToText(),
                SortType.DEF => CMess.carddef.ToText(),
                SortType.LEVEL => CMess.Level.ToText(),
                SortType.RACE => CMess.cardLabelRace.ToText(),
                SortType.ATTRIBUTE => CMess.cardLabelAttri.ToText(),
                SortType.CATEGORY => CMess.cardLabelCategory.ToText(),
                SortType.RARE => CMess.cardrare.ToText(),
                SortType.GPOINT => CMess.genesysPoint.ToText(),
                _ => CMess.unknown.ToText(),
            };
        }
    }
    #endregion

    #region Card Field
    [Flags]
    public enum CardDataField : ulong
    {
        None = 0,

        Ot = 1UL << 0,
        Alias = 1UL << 1,
        Setcode = 1UL << 2,
        Type = 1UL << 3,
        Atk = 1UL << 4,
        Def = 1UL << 5,
        Level = 1UL << 6,
        Race = 1UL << 7,
        Attribute = 1UL << 8,
        Category = 1UL << 9,
        Flag = 1UL << 10,
    }
    public enum CardTextField : ulong
    {
        None = 0,

        Name = 1UL << 0,
        Desc = 1UL << 1,
        Str1 = 1UL << 2,
        Str2 = 1UL << 3,
        Str3 = 1UL << 4,
        Str4 = 1UL << 5,
        Str5 = 1UL << 6,
        Str6 = 1UL << 7,
        Str7 = 1UL << 8,
        Str8 = 1UL << 9,
        Str9 = 1UL << 10,
        Str10 = 1UL << 11,
        Str11 = 1UL << 12,
        Str12 = 1UL << 13,
        Str13 = 1UL << 14,
        Str14 = 1UL << 15,
        Str15 = 1UL << 16,
        Str16 = 1UL << 17,
    }
    public enum CardFieldBanList : ulong
    {
        None = 0,
        Name = 1UL << 0,
    }

    public static class CardFieldUpdate
    {
        public static readonly Dictionary<CardDataField, Action<ScriptSupport.Models.CardData, ScriptSupport.Models.CardData>> DataFieldUpdaters = new()
        {
            {CardDataField.Ot, (t, s) => t.ot = s.ot },
            {CardDataField.Alias, (t, s) => t.alias = s.alias },
            {CardDataField.Setcode, (t, s) => t.setcode = s.setcode },
            {CardDataField.Type, (t, s) => t.type = s.type },
            {CardDataField.Atk, (t, s) => t.atk = s.atk },
            {CardDataField.Def, (t, s) => t.def = s.def },
            {CardDataField.Level, (t, s) => t.level = s.level },
            {CardDataField.Race, (t, s) => t.race = s.race },
            {CardDataField.Attribute, (t, s) => t.attribute = s.attribute },
            {CardDataField.Category, (t, s) => t.category = s.category },
            {CardDataField.Flag, (t, s) => t.flag = s.flag },
        };
        public static readonly Dictionary<CardTextField, Action<ScriptSupport.Models.CardText, ScriptSupport.Models.CardText>> TextFieldUpdaters = new()
        {
            {CardTextField.Name, (t, s) => t.name = s.name },
            {CardTextField.Desc, (t, s) => t.desc = s.desc },
            {CardTextField.Str1, (t, s) => t.str1 = s.str1 },
            {CardTextField.Str2, (t, s) => t.str2 = s.str2 },
            {CardTextField.Str3, (t, s) => t.str3 = s.str3 },
            {CardTextField.Str4, (t, s) => t.str4 = s.str4 },
            {CardTextField.Str5, (t, s) => t.str5 = s.str5 },
            {CardTextField.Str6, (t, s) => t.str6 = s.str6 },
            {CardTextField.Str7, (t, s) => t.str7 = s.str7 },
            {CardTextField.Str8, (t, s) => t.str8 = s.str8 },
            {CardTextField.Str9, (t, s) => t.str9 = s.str9 },
            {CardTextField.Str10, (t, s) => t.str10 = s.str10 },
            {CardTextField.Str11, (t, s) => t.str11 = s.str11 },
            {CardTextField.Str12, (t, s) => t.str12 = s.str12 },
            {CardTextField.Str13, (t, s) => t.str13 = s.str13 },
            {CardTextField.Str14, (t, s) => t.str14 = s.str14 },
            {CardTextField.Str15, (t, s) => t.str15 = s.str15 },
            {CardTextField.Str16, (t, s) => t.str16 = s.str16 },
        };
    }
    #endregion

    #region MessageBox
    public enum MessageBoxIconType
    {
        Error,
        Warning,
        Notification,
        Information,
        Question
    }
    #endregion

    #region Filter
    public enum ItemsEdit
    {
        Setting = 0,
        ReplaceDesc = 1,
        ReplaceField = 2,
        ImportData = 3,
    }
    [Flags]
    public enum FilterOption
    {
        None = 0,
        Advanced = 1 << 0,
        MatchCase = 1 << 1,
        Wildcards = 1 << 2,
        Prefix = 1 << 3,
        Suffix = 1 << 4,
        MatchWhole = 1 << 5,
        IgnPunct = 1 << 6,
        IgnSpace = 1 << 7
    }
    #endregion

    #region LineEnding
    public enum LineEnding
    {
        CRLF,   // Windows  \r\n
        LF,     // Unix     \n
        CR      // Mac cũ   \r
    }
    #endregion

    #region IndentOptions
    public enum IndentOption
    {
        Spaces,
        Tabs
    }
    #endregion

    #region Link
    public enum LinkType
    {
        External,
        Internal,
        Api,
        Unknown
    }
    #endregion
}
