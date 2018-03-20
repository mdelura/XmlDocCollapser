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
            IOutliningManager outliningManager = OutliningManagerService?.GetOutliningManager(textView);

            if (outliningManager != null)
                outliningManager.RegionsChanged += OutliningManager_RegionsChanged;
        }

        private void OutliningManager_RegionsChanged(object sender, RegionsChangedEventArgs e)
        {
            const string xmlDocCollapsedFormStart = "/// <summary>";

            IOutliningManager outliningManager = sender as IOutliningManager;

            outliningManager.CollapseAll(e.AffectedSpan, c => c?.CollapsedForm?.ToString()?.StartsWith(xmlDocCollapsedFormStart) ?? false && !c.IsCollapsed && c.IsCollapsible);

            //Unsubscribe from RegionsChanged after initial change
            outliningManager.RegionsChanged -= OutliningManager_RegionsChanged;
        }
    }
}

/*

    System.NullReferenceException: 
    Object reference not set to an instance of an object. at 
    XmlDocCollapser.TextViewCreationListener.TextViewCreated(IWpfTextView textView) 
    in 
    E:\Users\micha\VisualStudio\Projects\VSIX\XmlDocCollapser\XmlDocCollapser\TextViewCreationListener.cs:line 21 
        at Microsoft.VisualStudio.Text.Editor.Implementation.WpfTextView.<>c__DisplayClass244_0.<BindContentTypeSpecificAssets>b__1() 
        at Microsoft.VisualStudio.Text.Utilities.GuardedOperations.CallExtensionPoint(Object errorSource, Action call)
        --- End of stack trace from previous location where exception was thrown --- 
        at Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.WatsonReport.GetClrWatsonExceptionInfo(Exception exceptionObject)
     
     */
