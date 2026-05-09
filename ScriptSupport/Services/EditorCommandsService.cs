using System.Windows.Input;
using ScriptSupport.Helpers;
using ScriptSupport.Commands;
using ScriptSupport.Converter;
using ScriptSupport.Interfaces;
using ScriptSupport.ViewModels;

namespace ScriptSupport.Services
{
    public class EditorCommandsService
    {
        private readonly IFloatingPanelInterface _panelService;
        public ICommand CutCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand ToFullWidthCommand { get; }
        public ICommand ToHalfWidthCommand { get; }
        public ICommand ToSuperScriptCommand { get; }
        public ICommand FromSuperScriptCommand { get; }
        public ICommand ToSubScriptCommand { get; }
        public ICommand FromSubScriptCommand { get; }
        public ICommand SpecialCharactersCommand { get; }

        public EditorCommandsService(IFloatingPanelInterface panelService)
        {
            _panelService = panelService;
            CutCommand = new RelayCommand<object>(p => EditorHelper.Cut(p));
            CopyCommand = new RelayCommand<object>(p => EditorHelper.Copy(p));
            PasteCommand = new RelayCommand<object>(p => EditorHelper.Paste(p));
            ToFullWidthCommand = new RelayCommand<object>(ExecuteToFullWidth);
            ToHalfWidthCommand = new RelayCommand<object>(ExecuteToHalfWidth);
            ToSuperScriptCommand = new RelayCommand<object>(ExecuteToSuperScript);
            FromSuperScriptCommand = new RelayCommand<object>(ExecuteFromSuperScript);
            ToSubScriptCommand = new RelayCommand<object>(ExecuteToSubScript);
            FromSubScriptCommand = new RelayCommand<object>(ExecuteFromSubScript);
            SpecialCharactersCommand = new RelayCommand(ExecuteSpecialCharacters);
        }

        private void ExecuteToFullWidth(object? parameter)
        {
            var text = EditorHelper.GetSelectedText(parameter);
            if (string.IsNullOrEmpty(text)) return;
            EditorHelper.SetSelectedText(parameter, StringConvert.ConvertToFullWidth(text));
        }
        private void ExecuteToHalfWidth(object? parameter)
        {
            var text = EditorHelper.GetSelectedText(parameter);
            if (string.IsNullOrEmpty(text)) return;
            EditorHelper.SetSelectedText(parameter, StringConvert.ConvertToHalfWidth(text));
        }
        private void ExecuteToSuperScript(object? parameter)
        {
            var text = EditorHelper.GetSelectedText(parameter);
            if (string.IsNullOrEmpty(text)) return;
            EditorHelper.SetSelectedText(parameter, StringConvert.ConvertToSuperscript(text));
        }
        private void ExecuteFromSuperScript(object? parameter)
        {
            var text = EditorHelper.GetSelectedText(parameter);
            if (string.IsNullOrEmpty(text)) return;
            EditorHelper.SetSelectedText(parameter, StringConvert.ConvertFromSuperscript(text));
        }
        private void ExecuteToSubScript(object? parameter)
        {
            var text = EditorHelper.GetSelectedText(parameter);
            if (string.IsNullOrEmpty(text)) return;
            EditorHelper.SetSelectedText(parameter, StringConvert.ConvertToSubscript(text));
        }
        private void ExecuteFromSubScript(object? parameter)
        {
            var text = EditorHelper.GetSelectedText(parameter);
            if (string.IsNullOrEmpty(text)) return;
            EditorHelper.SetSelectedText(parameter, StringConvert.ConvertFromSubscript(text));
        }
        private void ExecuteSpecialCharacters()
        {
            _panelService.Show<SpecialCharViewModel>();
        }
    }
}
