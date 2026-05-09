using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using ScriptSupport.ViewModels;
using ScriptSupport.UserControls;

namespace ScriptSupport
{
    /// <summary>
    /// Interaction logic for MainWindowEmbedded.xaml
    /// </summary>
    public partial class MainWindowEmbedded : Window
    {
        public MainWindowEmbedded(MainViewModel vm, MainUserControl mainUserControl)
        {
            InitializeComponent();
            DataContext = vm;
            MainContent.Content = mainUserControl;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var handle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(handle)?.AddHook(WindowProc);
        }
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_GETMINMAXINFO = 0x0024;

            if (msg == WM_GETMINMAXINFO)
            {
                WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
            }

            return IntPtr.Zero;
        }
        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            var screen = Screen.FromHandle(hwnd);
            var workArea = screen.WorkingArea;
            var monitorArea = screen.Bounds;

            mmi.ptMaxPosition.x = Math.Abs(workArea.Left - monitorArea.Left);
            mmi.ptMaxPosition.y = Math.Abs(workArea.Top - monitorArea.Top);

            mmi.ptMaxSize.x = workArea.Width;
            mmi.ptMaxSize.y = workArea.Height;

            Marshal.StructureToPtr(mmi, lParam, true);
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Tab)
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.SwitchDocument();
                }

                e.Handled = true;
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximize();
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }
        private void ToggleMaximize()
        {
            this.WindowState = this.WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}