using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Specialized;
using ICSharpCode.AvalonEdit.Rendering;
using ScriptSupport.Models.Settings;
using ScriptSupport.Theming;
using ScriptSupport.Editor.Hover;
using ScriptSupport.Editor.Completion;
using ScriptSupport.ViewModels;
using ScriptSupport.Collections;
using ScriptSupport.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ScriptSupport.UserControls
{
    /// <summary>
    /// Interaction logic for CodeEditor.xaml
    /// </summary>
    public partial class CodeEditor : UserControl, IDisposable
    {
        private HoverService? _hoverService;
        private CompletionService? _completionService;
        private LineHighlightRenderer? _renderer;
        private readonly ICodeEditConfigInterface _configService;
        public CodeEditor()
        {
            _configService = ((App)Application.Current).Services.GetRequiredService<ICodeEditConfigInterface>();
            InitializeComponent();
            DataContextChanged += CodeEditor_DataContextChanged;
            SetConfig(_configService.Current);
            _configService.SettingChanged += OnEditorSettingChanged;
        }

        private void CodeEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is DocumentViewModel oldVm)
            {
                oldVm.HighlightLines.CollectionChanged -= HighlightLines_CollectionChanged;
            }

            if (_renderer != null)
            {
                Editor.TextArea.TextView.BackgroundRenderers.Remove(_renderer);
                _renderer = null;
            }

            if (e.NewValue is DocumentViewModel vm)
            {
                var brush = vm.UIConfig.ThemeColor as SolidColorBrush;
                if (brush != null)
                {
                    Color color = brush.Color;
                    color.A = 150;

                    Editor.TextArea.SelectionBrush = new SolidColorBrush(color);
                    Editor.TextArea.Caret.CaretBrush = vm.UIConfig.ThemeColor;
                }

                _renderer = new LineHighlightRenderer(Editor.TextArea.TextView, vm.UIConfig);
                Editor.TextArea.TextView.BackgroundRenderers.Add(_renderer);
                vm.AttachRenderer(_renderer);

                // Subscribe CollectionChanged
                vm.HighlightLines.CollectionChanged += HighlightLines_CollectionChanged;

                // Sync trạng thái ban đầu
                if (vm.HighlightLines?.Count > 0) _renderer.SetLines(vm.HighlightLines);
            }
        }
        private void HighlightLines_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_renderer == null) return;

            // sender chính là HighlightLines collection
            if (sender is BulkObservableCollection<int> lines) _renderer.SetLines(lines);

            // Trigger redraw
            Editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not DocumentViewModel vm) return;

            _completionService = vm.EditorServiceFactory.CreateCompletion(Editor);
            _hoverService = vm.EditorServiceFactory.CreateHover(Editor);

            Editor.TextArea.KeyDown += TextArea_KeyDown;
            Editor.TextArea.TextEntered += TextArea_TextEntered;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Editor.TextArea.KeyDown -= TextArea_KeyDown;
            Editor.TextArea.TextEntered -= TextArea_TextEntered;

            _completionService?.Dispose();
            _hoverService?.Dispose();
            _completionService = null;
            _hoverService = null;
        }

        private void TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _completionService?.ShowCompletion();
                e.Handled = true;
                return;
            }
            else if (e.Key == Key.Back)
            {
                _completionService?.OnKeyDown(e);
                _completionService?.ShowCompletion();
                //e.Handled = true;
                return;
            }
            _completionService?.OnKeyDown(e);
        }
        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length == 1)
                _completionService?.OnTextEntered(e.Text[0]);
        }
        private void OnEditorSettingChanged(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() => SetConfig(_configService.Current));
        }
        private void SetConfig(CodeEditSetting? setting)
        {
            if (setting == null) return;

            Editor.WordWrap = setting.WordWrap;

            Editor.ShowLineNumbers = setting.ShowLineNumber; // Hiển thị số dòng
            Editor.Options.ShowSpaces = setting.ShowSpace; //Hiển thị ký hiệu · cho các ký tự khoảng trắng (space).
            Editor.Options.ShowTabs = setting.ShowTab; //Hiển thị ký hiệu » cho các ký tự tab.
            Editor.Options.ShowEndOfLine = setting.ShowEndLine; //Hiển thị ký hiệu ¶ ở cuối mỗi dòng.
            Editor.Options.ShowBoxForControlCharacters = setting.ShowControlChar; //Hiển thị hộp chứa mã hex cho các ký tự điều khiển không in được.
            Editor.Options.HighlightCurrentLine = setting.HighLightLine; //Tô sáng dòng hiện tại chứa caret (con trỏ văn bản).
            Editor.Options.HideCursorWhileTyping = setting.HiddenCursor; //Ẩn con trỏ chuột khi người dùng đang gõ phím.

            Editor.Options.ShowColumnRuler = setting.ShowColumnRuler; //Hiển thị thanh thước dọc tại vị trí cột xác định.
            Editor.Options.ColumnRulerPosition = setting.ColumnRulerPosition; //Vị trí (số cột) của thanh thước dọc khi ShowColumnRuler bật.


            Editor.Options.EnableTextDragDrop = setting.TextDragDrop; //Cho phép kéo thả đoạn văn bản trong vùng soạn thảo.
            Editor.Options.AllowToggleOverstrikeMode = setting.Overstrikemode; //Cho phép chuyển đổi chế độ ghi đè (overstrike mode).
            Editor.Options.CutCopyWholeLine = setting.HandleWholeLine; //Khi không có vùng chọn, thao tác cắt/sao chép sẽ áp dụng cho toàn bộ dòng hiện tại.
            Editor.Options.EnableVirtualSpace = setting.VirtualSpace; //Cho phép đặt caret (con trỏ) vượt ra ngoài cuối dòng (virtual space).
            Editor.Options.EnableRectangularSelection = setting.RectangularSelection; //Cho phép chọn vùng hình chữ nhật (Alt + kéo chuột).
            Editor.Options.AllowScrollBelowDocument = setting.ScrollBelowDocument; //Cho phép cuộn vượt quá cuối tài liệu(thêm không gian trống bên dưới).

            Editor.Options.EnableImeSupport = setting.IMESupport; //Bật/tắt hỗ trợ IME (Input Method Editor) cho các ngôn ngữ như Trung, Nhật, Hàn.
            Editor.Options.EnableHyperlinks = setting.HyperLink; //Cho phép nhận diện và click vào các liên kết(URL) trong văn bản.
            Editor.Options.EnableEmailHyperlinks = setting.MailHyperLink; //Cho phép nhận diện và click vào các liên kết email trong văn bản.
            Editor.Options.RequireControlModifierForHyperlinkClick = setting.RequireControlHyperLink; //Yêu cầu nhấn phím Ctrl khi click vào liên kết để mở liên kết đó.

            Editor.Options.ConvertTabsToSpaces = setting.TabsToSpace; //Chuyển tab thành khoảng trắng khi thụt lề.
            Editor.Options.IndentationSize = setting.IndentationSize; //Độ rộng của một đơn vị thụt lề (số ký tự).
            Editor.Options.WordWrapIndentation = setting.WordWrapIndentation; //Độ thụt lề cho các dòng bị ngắt(word wrap), trừ dòng đầu tiên.
            Editor.Options.InheritWordWrapIndentation = setting.InheritWordWrapIndentation; //Các dòng ngắt dòng có kế thừa thụt lề của dòng đầu tiên hay không.
            // textEditorPane.Options.GetIndentationString(); //Phương thức trả về chuỗi thụt lề phù hợp (tab hoặc số lượng space) tùy theo cấu hình.
        }

        public void Dispose()
        {
            _configService.SettingChanged -= OnEditorSettingChanged;
            _completionService?.Dispose();
            _hoverService?.Dispose();

            DataContextChanged -= CodeEditor_DataContextChanged;
            if (DataContext is DocumentViewModel vm)
                vm.HighlightLines.CollectionChanged -= HighlightLines_CollectionChanged;
        }
    }
}
