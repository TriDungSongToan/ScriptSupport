using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ScriptSupport.States;

namespace ScriptSupport.Views
{
    /// <summary>
    /// Interaction logic for CMSG.xaml
    /// </summary>
    public class ButtonInfo : INotifyPropertyChanged
    {
        public string Text { get; set; } = "";
        private bool _isDefault;
        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                _isDefault = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDefault)));
            }
        }
        public int Index { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public partial class CMSG : Window, INotifyPropertyChanged
    {
        public static UIConfigState? UIConfig { get; private set; }

        private Visibility _iconVisibility = Visibility.Collapsed;
        public Visibility IconVisibility
        {
            get => _iconVisibility;
            set
            {
                _iconVisibility = value;
                OnPropertyChanged(nameof(IconVisibility));
            }
        }
        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        private ObservableCollection<ButtonInfo> _buttons = new();
        public ObservableCollection<ButtonInfo> Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value;
                OnPropertyChanged(nameof(Buttons));
            }
        }
        public int ResultIndex { get; private set; } = -1;

        public CMSG()
        {
            InitializeComponent();
            DataContext = this;
        }
        public static void Initialize(UIConfigState uiConfig)
        {
            UIConfig = uiConfig;
        }
        public static int Show(string title, ImageSource? icon, string message, string[] buttons, int defaultButtonIndex = 0)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                return Application.Current.Dispatcher.Invoke(() =>
                    Show(title, icon, message, buttons, defaultButtonIndex));

            if (defaultButtonIndex < 0 || defaultButtonIndex >= buttons.Length)
                defaultButtonIndex = 0;

            var msgBox = new CMSG
            {
                Title = title,
                Message = message,
                Buttons = new ObservableCollection<ButtonInfo>(
                    buttons.Select((text, index) => new ButtonInfo
                    {
                        Text = text,
                        IsDefault = index == defaultButtonIndex,
                        Index = index
                    }))
            };

            if (icon != null)
            {
                msgBox.MessageIcon.Source = icon;
                msgBox.MessageIconHeader.Source = icon;
                msgBox.IconVisibility = Visibility.Visible;
            }
            else msgBox.IconVisibility = Visibility.Collapsed;

            msgBox.ShowDialog();
            return msgBox.ResultIndex;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var btn = Buttons.FirstOrDefault(b => b.IsDefault);
                if (btn != null)
                {
                    ResultIndex = btn.Index;
                    DialogResult = true;
                    Close();
                }
            }
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (Buttons.Count == 0) return;
                int currentIndex = Buttons.ToList().FindIndex(b => b.IsDefault);
                if (currentIndex < 0) currentIndex = 0;

                Buttons[currentIndex].IsDefault = false;

                int nextIndex = (currentIndex + 1) % Buttons.Count;

                if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                    nextIndex = (currentIndex - 1 + Buttons.Count) % Buttons.Count;

                Buttons[nextIndex].IsDefault = true;
                e.Handled = true;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ButtonInfo info)
            {
                ResultIndex = info.Index;
                DialogResult = true;
                Close();
            }
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
