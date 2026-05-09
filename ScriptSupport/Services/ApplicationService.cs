using System.Windows;
using ScriptSupport.Interfaces;

namespace ScriptSupport.Services
{
    public class ApplicationService : IApplicationInterface
    {
        public void CloseWindow(object viewModel)
        {
            var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == viewModel);
            window?.Close();
        }
        public void Shutdown()
        {
            Application.Current.Shutdown();
        }

    }
}
