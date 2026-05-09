using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ScriptSupport.Behaviors
{
    public static class RichTextBoxBehavior
    {
        public static readonly DependencyProperty DocumentProperty =
        DependencyProperty.RegisterAttached(
            "Document",
            typeof(FlowDocument),
            typeof(RichTextBoxBehavior),
            new PropertyMetadata(null, OnDocumentChanged));

        public static FlowDocument GetDocument(DependencyObject obj)
            => (FlowDocument)obj.GetValue(DocumentProperty);

        public static void SetDocument(DependencyObject obj, FlowDocument value)
            => obj.SetValue(DocumentProperty, value);

        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBox rtb) return;
            if (e.NewValue is FlowDocument doc)
                rtb.Document = doc;
            else
                rtb.Document = new FlowDocument();
        }
    }
}
