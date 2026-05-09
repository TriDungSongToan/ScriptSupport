using System.Windows.Input;
using ScriptSupport.States;

namespace ScriptSupport.Interfaces
{
    public interface IEditorContextMenuInterface
    {
        UIConfigState UIConfig { get; }
        ICommand CutCommand { get; }
        ICommand CopyCommand { get; }
        ICommand PasteCommand { get; }
        ICommand ToFullWidthCommand { get; }
        ICommand ToHalfWidthCommand { get; }
        ICommand ToSuperScriptCommand { get; }
        ICommand FromSuperScriptCommand { get; }
        ICommand ToSubScriptCommand { get; }
        ICommand FromSubScriptCommand { get; }
        ICommand SpecialCharactersCommand { get; }
    }
}
