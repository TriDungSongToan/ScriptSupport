namespace ScriptSupport.Interfaces
{
    public interface IFloatingPanelInterface
    {
        event Action<Type>? ShowRequested;
        void Show<TViewModel>();
    }
}
