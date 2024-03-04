using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace XmlDocCollapser
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class TextViewCreationListener : IWpfTextViewCreationListener
    {
        [Import]
        internal IOutliningManagerService OutliningManagerService { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            var manager = OutliningManagerService?.GetOutliningManager(textView);
            if (manager != null)
                manager.RegionsChanged += RegionChangedHandler;
        }

        void RegionChangedHandler(object sender, RegionsChangedEventArgs e)
        {
            var manager = sender as IOutliningManager;
            manager.CollapseAll(e.AffectedSpan, c => Selector(c));
            manager.RegionsChanged -= RegionChangedHandler;
        }

        static bool Selector(ICollapsible c)
        {
            if (c is null || c.IsCollapsed || !c.IsCollapsible)
                return false;

            var text = c.CollapsedForm.ToString();
            if (text.Length < 4)
                return false;

            var hasPrefix = text.StartsWith("///") || text.StartsWith("/**");
            if (!hasPrefix)
                return false;

            var hasDelimiter = char.IsWhiteSpace(text[3]);
            return hasDelimiter;
        }
    }
}