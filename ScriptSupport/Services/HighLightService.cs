using System.IO;
using System.Xml;
using ScriptSupport.States;
using ScriptSupport.Interfaces;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace ScriptSupport.Services
{
    public class HighLightService : IHighlightInterface
    {
        private readonly HighlightState _state;
        private string? _currentPath;
        private IHighlightingDefinition? _currentDef;

        public HighLightService(HighlightState state)
        {
            _state = state;
        }

        public async Task LoadAsync(string path)
        {
            _currentPath = path;
            await Task.Run(() =>
            {
                using var stream = File.OpenRead(path);
                using var reader = new XmlTextReader(stream);
                var def = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                _currentDef = def;
                App.Current.Dispatcher.Invoke(() => _state.Current = def);
            });
        }

        public Task ReloadAsync()
        {
            return _currentPath != null ? LoadAsync(_currentPath) : Task.CompletedTask;
        }

        public void Dispose()
        {
            _currentDef = null;
            _state.Current = null;
        }

    }
}
