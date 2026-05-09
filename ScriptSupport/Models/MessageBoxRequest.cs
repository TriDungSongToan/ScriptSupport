namespace ScriptSupport.Models
{
    public class MessageBoxRequest
    {
        public string Title { get; set; } = string.Empty;
        public MessageBoxIconType IconType { get; set; }
        public string Message { get; set; } = string.Empty;
        public string[] Buttons { get; set; } = Array.Empty<string>();
        public int DefaultButtonIndex { get; set; } = 0;

        public TaskCompletionSource<int>? ResponseSource { get; set; }
        public Task<int> ResponseTask => ResponseSource?.Task ?? Task.FromResult(-1);
    }
}
