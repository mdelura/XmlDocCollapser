using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Utilities;

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
            IOutliningManager outliningManager = OutliningManagerService.GetOutliningManager(textView);

            outliningManager.RegionsChanged += OutliningManager_RegionsChanged;
        }

        private void OutliningManager_RegionsChanged(object sender, RegionsChangedEventArgs e)
        {
            const string xmlDocCollapsedFormStart = "/// <summary>";

            IOutliningManager outliningManager = sender as IOutliningManager;

            outliningManager.CollapseAll(e.AffectedSpan, c => c.CollapsedForm.ToString().StartsWith(xmlDocCollapsedFormStart) && !c.IsCollapsed && c.IsCollapsible);

            //Unsubscribe from RegionsChanged after initial change
            outliningManager.RegionsChanged -= OutliningManager_RegionsChanged;
        }
    }
}
