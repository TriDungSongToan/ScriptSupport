namespace ScriptSupport.Interfaces
{
    public interface ILauncherInterface
    {
        (bool, string) OpenWeb(string url);
        (bool, string) OpenFileOrFolder(string path);
        (bool, string) OpenLink(string input);
    }
}
