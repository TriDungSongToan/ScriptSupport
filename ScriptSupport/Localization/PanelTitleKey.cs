namespace ScriptSupport.Localization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PanelTitleKeyAttribute : Attribute
    {
        public Language Key { get; }

        public PanelTitleKeyAttribute(Language key)
        {
            Key = key;
        }
    }
}
