using System.Reflection;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;
using ScriptSupport.Models;
using ScriptSupport.States;
using ScriptSupport.Stores;
using ScriptSupport.Helpers;
using ScriptSupport.Commands;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;
using ScriptSupport.Localization;
using CMess = ScriptSupport.Localization.Language;

namespace ScriptSupport.ViewModels
{
    public class AboutViewModel : BaseViewModel, IDisposable
    {
        #region Fields
        public UIConfigState UIConfig { get; }
        private readonly AppEnvironment _appEnvironment;
        private readonly AppRuntimeConfig _runtimeConfig;
        private readonly ConfigStore _configStore;
        private readonly IDialogInterface _dialogService;
        private readonly IStringInterface _stringService;
        private readonly ILauncherInterface _launcherService;
        private readonly IApplicationInterface _applicationInterface;
        #endregion

        #region Properties
        private string _appName = "ScriptSupport";
        public string AppName
        {
            get => _appName;
            set => SetProperty(ref _appName, value);
        }
        private string _version = "1.0.0";
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }
        private string _author = "Trí Dũng Song Toàn";
        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }
        private string _license = "Non-commercial License";
        public string License
        {
            get => _license;
            set => SetProperty(ref _license, value);
        }
        private string _copyright = "Copyright © 2026 Trí Dũng Song Toàn. All rights reserved.";
        public string Copyright
        {
            get => _copyright;
            set => SetProperty(ref _copyright, value);
        }
        private string _netVersionBuilded = string.Empty;
        public string NetVersionBuild
        {
            get => _netVersionBuilded;
            set => SetProperty(ref _netVersionBuilded, value);
        }
        private string _netRunTime = string.Empty;
        public string NetRunTime
        {
            get => _netRunTime;
            set => SetProperty(ref _netRunTime, value);
        }

        private string _operatingSystem = string.Empty;
        public string OperatingSystem
        {
            get => _operatingSystem;
            set => SetProperty(ref _operatingSystem, value);
        }
        private DateTime _releaseDate = new DateTime();
        public DateTime ReleaseDate
        {
            get => _releaseDate;
            set => SetProperty(ref _releaseDate, value);
        }
        #endregion

        #region Commands
        public RelayCommand? CreatorWebCommand { get; set; }
        public RelayCommand? GithubCommand { get; set; }
        public RelayCommand? CopyInfoCommand { get; set; }
        public RelayCommand? OKCommand { get; set; }
        #endregion

        #region Constructor
        public AboutViewModel(UIConfigState uIConfig, AppEnvironment appEnvironment, AppRuntimeConfig runtimeConfig, ConfigStore configStore,
            IDialogInterface dialogInterface, ILauncherInterface launcherService, IStringInterface stringService,
            IApplicationInterface applicationInterface)
        {
            UIConfig = uIConfig;
            _appEnvironment = appEnvironment;
            _runtimeConfig = runtimeConfig;
            _configStore = configStore;
            _dialogService = dialogInterface;
            _launcherService = launcherService;
            _stringService = stringService;
            _applicationInterface = applicationInterface;

            InitializeCommand();
            LoadAbout();
        }
        private void InitializeCommand()
        {
            CreatorWebCommand = new RelayCommand(_ => CreatorWeb());
            GithubCommand = new RelayCommand(_ => Github());
            CopyInfoCommand = new RelayCommand(async _ => await CopyInfo());
            OKCommand = new RelayCommand(_ => OK());
        }
        private void LoadAbout()
        {
            AppName = "Script Support";
            Version = "1.0.0";
            Author = "Trí Dũng Song Toàn";
            License = "Non-commercial License";
            Copyright = "Copyright © 2026 Trí Dũng Song Toàn. All rights reserved.";
            var framework = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
            NetVersionBuild = framework?.Replace(".NETCoreApp,Version=v", ".NET ").Replace(".NETFramework,Version=v", ".NET Framework ") ?? "Unknown";
            NetRunTime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            OperatingSystem = RuntimeInformation.OSDescription;

            ReleaseDate = BuildInfoHelper.ReleaseDateUtc;
        }
        #endregion

        #region Command Methods
        private void CreatorWeb()
        {
            string URL = _runtimeConfig.CreatorURL;
            var (Success, ErrorMessage) = _launcherService.OpenWeb(URL);
            if (!Success)
            {
                var request = new MessageBoxRequest
                {
                    Title = CMess.error.ToText(),
                    IconType = MessageBoxIconType.Error,
                    Message = ErrorMessage,
                    Buttons = new[] { CMess.ok.ToText() },
                    DefaultButtonIndex = 0,
                    ResponseSource = null
                };
                _dialogService.ShowMessage(request);
            }
        }
        private void Github()
        {
            string URL = _runtimeConfig.ScriptSupportURL;
            var (Success, ErrorMessage) = _launcherService.OpenWeb(URL);
            if (!Success)
            {
                var request = new MessageBoxRequest
                {
                    Title = CMess.error.ToText(),
                    IconType = MessageBoxIconType.Error,
                    Message = ErrorMessage,
                    Buttons = new[] { CMess.ok.ToText() },
                    DefaultButtonIndex = 0,
                    ResponseSource = null
                };
                _dialogService.ShowMessage(request);
            }
        }
        private async Task CopyInfo()
        {
            string text = $"{AppName}\n" +
                $"Version: {Version}\n" +
                $"License: {License}\n" +
                $"Build Version: {NetVersionBuild}\n" +
                $"Release Date: {ReleaseDate:yyyy-MM-dd} (yyyy-MM-dd)\n" +
                $"Runtime Version: {NetRunTime}\n" +
                $"OS: {OperatingSystem}";

            var (Success, ClipboardText) = _stringService.SetClipboard(text);
            if (!Success)
            {
                var request = new MessageBoxRequest
                {
                    Title = CMess.error.ToText(),
                    IconType = MessageBoxIconType.Error,
                    Message = ClipboardText,
                    Buttons = new[] { CMess.ok.ToText() },
                    DefaultButtonIndex = 0,
                    ResponseSource = null
                };
                await _dialogService.ShowMessage(request);
            }
        }
        private void OK()
        {
            _applicationInterface.CloseWindow(this);
        }
        #endregion

        public void Dispose()
        {
            ///
        }
    }
}
