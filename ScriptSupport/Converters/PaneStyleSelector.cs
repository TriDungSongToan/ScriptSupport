using System.Windows;
using System.Windows.Controls;
using ScriptSupport.ViewModels;

namespace ScriptSupport.Converters
{
    public class PaneStyleSelector : StyleSelector
    {
        public Style? DocumentStyle { get; set; }
        public Style? AnchorableStyle { get; set; }

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            if (item is DocumentViewModel) return DocumentStyle;
            //if (item is ToolViewModel) return AnchorableStyle;
            return base.SelectStyle(item, container);
        }
    }
}
