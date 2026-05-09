using System.IO;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ScriptSupport.Models;
using ScriptSupport.Factorys;
using ScriptSupport.ViewModels;
using ScriptSupport.Interfaces;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.Manager
{
    public class DocumentManager : INotifyPropertyChanged
    {
        private readonly IDialogInterface _dialogService;
        public ObservableCollection<DocumentViewModel> Documents { get; } = new();
        private readonly DocumentFactory _factory;
        private readonly List<DocumentViewModel> _mru = new();
        private readonly Dictionary<DocumentViewModel, FileSystemWatcher> _watchers = new();
        private DocumentViewModel? _activeDocument;
        public DocumentViewModel? ActiveDocument
        {
            get => _activeDocument;
            set
            {
                if (_activeDocument == value) return;

                _activeDocument = value;
                if (value != null)
                {
                    _mru.Remove(value);
                    _mru.Insert(0, value);
                }
                OnPropertyChanged();
            }
        }

        public DocumentManager(IDialogInterface dialogInterface, DocumentFactory factory)
        {
            _dialogService = dialogInterface;
            _factory = factory;
        }

        public async Task OpenDocument(string path, IReadOnlyList<int>? lines = null )
        {
            lines ??= new List<int>();
            var existing = FindPermanent(path);

            if (existing != null)
            {
                ActiveDocument = existing;
                existing.HighlightLines.ReplaceAll(lines);
                return;
            }
            await RemoveOldPreview();

            var doc = CreateDocument(path, false);
            doc.HighlightLines.ReplaceAll(lines);
            Documents.Add(doc);
            ActiveDocument = doc;
        }
        public async Task OpenPreview(string path, IReadOnlyList<int>? lines = null)
        {
            lines ??= new List<int>();
            var existing = FindPermanent(path);

            if (existing != null)
            {
                ActiveDocument = existing;
                existing.HighlightLines.ReplaceAll(lines);
                return;
            }

            await RemoveOldPreview();
            var doc = CreateDocument(path, true);
            doc.HighlightLines.ReplaceAll(lines);
            Documents.Add(doc);
            ActiveDocument = doc;
        }
        public async Task OpenDocumentEmpty(string title = "")
        {
            await RemoveOldPreview();
            var doc = CreateEmptyDocument(false, title);
            doc.HighlightLines.Clear();
            Documents.Add(doc);
            ActiveDocument = doc;
        }
        public async Task OpenPreviewEmpty(string title = "")
        {
            await RemoveOldPreview();
            var doc = CreateEmptyDocument(true, title);
            doc.HighlightLines.Clear();
            Documents.Add(doc);
            ActiveDocument = doc;
        }

        private async Task RemoveOldPreview()
        {
            var previews = Documents.Where(d => d.IsPreview).ToList();
            foreach (var doc in previews)
            {
                await CloseDocument(doc);
            }
            //var old = Documents.FirstOrDefault(d => d.IsPreview);
            //if (old != null) CloseDocument(old);
        }
        public void PromotePreview(DocumentViewModel doc)
        {
            if (!doc.IsPreview) return;
            doc.IsPreview = false;
        }

        private DocumentViewModel CreateDocument(string path, bool preview)
        {
            try
            {
                var doc = _factory.CreateDocument();
                doc.IsPreview = preview;
                doc.OpenFile(path);
                WatchFile(doc);
                return doc;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CreateDocument ERROR] {ex}");
                throw;
            }
        }
        private DocumentViewModel CreateEmptyDocument(bool preview, string title)
        {
            try
            {
                var doc = _factory.CreateDocument();
                doc.IsPreview = preview;
                doc.OpenEmpty(title);
                WatchFile(doc);
                return doc;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CreateDocument ERROR] {ex}");
                throw;
            }
        }
        private void WatchFile(DocumentViewModel doc)
        {
            if (doc.FilePath == null) return;

            var dir = Path.GetDirectoryName(doc.FilePath)!;
            var name = Path.GetFileName(doc.FilePath);

            var watcher = new FileSystemWatcher(dir, name)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            DateTime lastRead = DateTime.MinValue;

            WeakEventManager<FileSystemWatcher, FileSystemEventArgs>
                .AddHandler(watcher, nameof(watcher.Changed), (s, e) =>
                {
                    var now = DateTime.Now;
                    if ((now - lastRead).TotalMilliseconds < 200) return;
                    lastRead = now;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (doc.FilePath == null || doc.IsDirty) return;

                        try
                        {
                            var text = File.ReadAllText(doc.FilePath);
                            if (doc.Document.Text != text)
                            {
                                doc.LoadText(text);
                            }
                        }
                        catch (IOException)
                        {
                            ///
                        }
                    });
                });

            _watchers[doc] = watcher;
        }

        public async Task<bool> CloseDocument(DocumentViewModel doc)
        {
            if (doc.IsDirty)
            {
                var resetQuest = new MessageBoxRequest
                {
                    Title = CMess.questi.ToText(),
                    IconType = MessageBoxIconType.Question,
                    Message = $"{CMess.QuestSaveChange.ToText()}",
                    Buttons = new[] { CMess.yes.ToText(), CMess.no.ToText() },
                    DefaultButtonIndex = 0,
                    ResponseSource = new TaskCompletionSource<int>()
                };
                int resultQuest = await _dialogService.ShowMessage(resetQuest);
                if (resultQuest == 0) SaveDocument(doc);
            }

            if (_watchers.TryGetValue(doc, out var watcher))
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                _watchers.Remove(doc);
            }

            doc.Dispose();
            Documents.Remove(doc);
            _mru.Remove(doc);
            ActiveDocument = _mru.FirstOrDefault(d => Documents.Contains(d));
            return true;
        }
        public void SaveDocument(DocumentViewModel doc)
        {
            try
            {
                if (doc.FilePath == null) return;
                File.WriteAllText(doc.FilePath, doc.Document.Text);
                doc.IsDirty = false;
            }
            catch (Exception)
            {
                ///
            }
        }
        public void SaveAsDocument(DocumentViewModel doc, string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath)) return;
                File.WriteAllText(filePath, doc.Document.Text);
            }
            catch (Exception)
            {
                ///
            }
        }
        public void SaveActiveDocument()
        {
            var doc = ActiveDocument;
            if (doc == null || !doc.IsDirty) return;

            SaveDocument(doc);
        }

        public void SwitchNext()
        {
            if (Documents.Count <= 1) return;
            var index = Documents.IndexOf(ActiveDocument!);
            index++;

            if (index >= Documents.Count) index = 0;
            ActiveDocument = Documents[index];
        }
        public void InsertAtCaret(string text)
        {
            ActiveDocument?.InsertText(text);
        }

        private DocumentViewModel? FindPermanent(string path)
        {
            return Documents.FirstOrDefault(d => d.FilePath == path && !d.IsPreview);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
