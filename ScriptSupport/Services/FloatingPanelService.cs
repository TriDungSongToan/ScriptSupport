using ScriptSupport.Interfaces;

namespace ScriptSupport.Services
{
    public class FloatingPanelService : IFloatingPanelInterface
    {
        public event Action<Type>? ShowRequested;

        public void Show<TViewModel>()
        {
            ShowRequested?.Invoke(typeof(TViewModel));
        }
    }
}
