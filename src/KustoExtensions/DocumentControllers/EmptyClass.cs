using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AppKit;
using CoreGraphics;
using Foundation;
using KustoExtensions.Kusto;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Documents;
using ObjCRuntime;

namespace KustoExtensions
{
    class QueryView : NSView
    {
        private NSTextView textView;

        public QueryView()
        {
            // todo: we want this to be an editor
            this.textView = new NSTextView() { TranslatesAutoresizingMaskIntoConstraints = false };
            this.AddSubview(this.textView);

            textView.TextStorage.SetString(new NSAttributedString("Hellow orls"));
        }

        public override void SetFrameSize(CGSize newSize)
        {
            base.SetFrameSize(newSize);
            this.textView.Frame = this.Bounds;
        }

        public void SetText(string text)
        {
            textView.TextStorage.SetString(new NSAttributedString(text));
        }

        internal string GetQueryScript()
        {
            return this.textView.TextStorage.Value;
        }
    }

    class ResultsView : NSView
    {
        private NSTextView textView;

        public ResultsView()
        {
            this.TranslatesAutoresizingMaskIntoConstraints = false;

            this.textView = new NSTextView() { TranslatesAutoresizingMaskIntoConstraints = false };


            //this.WantsLayer = true;
            //this.Layer.BackgroundColor = NSColor.Red.CGColor;

            this.AddSubview(this.textView);
            this.textView.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor).Active = true;
            this.textView.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor).Active = true;
            this.textView.TopAnchor.ConstraintEqualTo(this.TopAnchor).Active = true;
            this.textView.BottomAnchor.ConstraintEqualTo(this.BottomAnchor).Active = true;
        }

        public void SetText(string text)
        {
            textView.TextStorage.SetString(new NSAttributedString(text));
        }
    }

    class QueryAndResultsView : NSSplitView
    {
        private QueryView queryView;
        private ResultsView resultsView;
        private NSSplitView splitView;
        private NSView toolbar;
        private NSButton runButton;

        public QueryAndResultsView()
        {
            this.TranslatesAutoresizingMaskIntoConstraints = false;

            this.queryView = new QueryView();
            this.resultsView = new ResultsView();
            this.splitView = new NSSplitView() { TranslatesAutoresizingMaskIntoConstraints = false };
            this.toolbar = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
            this.runButton = new NSButton() { TranslatesAutoresizingMaskIntoConstraints = false, BezelStyle = NSBezelStyle.Circular };

            // todo: selector
            this.runButton.Activated += RunButton_Activated;

            // todo: arrange these better by default
            this.splitView.AddArrangedSubview(this.queryView);
            this.splitView.AddArrangedSubview(this.resultsView);

            this.AddSubview(this.toolbar);
            this.toolbar.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor).Active = true;
            this.toolbar.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor).Active = true;
            this.toolbar.TopAnchor.ConstraintEqualTo(this.TopAnchor).Active = true;
            this.toolbar.HeightAnchor.ConstraintEqualTo(30).Active = true;

            this.toolbar.AddSubview(this.runButton);
            this.runButton.LeadingAnchor.ConstraintEqualTo(this.toolbar.LeadingAnchor, 4).Active = true;
            this.runButton.WidthAnchor.ConstraintEqualTo(26).Active = true;
            this.runButton.TopAnchor.ConstraintEqualTo(this.toolbar.TopAnchor, 2).Active = true;
            this.runButton.HeightAnchor.ConstraintEqualTo(26).Active = true;

            this.AddSubview(this.splitView);
            this.splitView.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor).Active = true;
            this.splitView.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor).Active = true;
            this.splitView.TopAnchor.ConstraintEqualTo(this.toolbar.BottomAnchor).Active = true;
            this.splitView.BottomAnchor.ConstraintEqualTo(this.BottomAnchor).Active = true;
        }

        private void RunButton_Activated(object sender, EventArgs e)
        {
            // run the script
            var x = new KustoTest();
            var y = x.GetTest(this.queryView.GetQueryScript());
            this.SetResultText(y);

        }

        public void SetQueryText(string text)
        {
            this.queryView.SetText(text);
        }

        public void SetResultText(string text)
        {
            this.resultsView.SetText(text);
        }

    }



    [ExportFileDocumentController(
        Id = "KustoQuery",
        Name = "Kusto Query",
        FileExtension = ".kql",
        CanUseAsDefault = true,
        InsertBefore = "DefaultDisplayBinding")]
    class KustoQueryFileDocumentController : FileDocumentController//, IOutlinedDocument, IPropertyPadProvider
    {
        private QueryAndResultsView contentView;
        private FilePath filePath;


        public KustoQueryFileDocumentController()
        {
            contentView = new QueryAndResultsView();
            contentView.TranslatesAutoresizingMaskIntoConstraints = false;
        }

        async void PropertyPad_Changed(object sender, EventArgs e)
        {
        }

        protected override Task OnSave()
        {
            return Task.FromResult(true);
        }

        protected override async Task OnInitialize(ModelDescriptor modelDescriptor, Properties status)
        {
            if (!(modelDescriptor is FileDescriptor fileDescriptor))
                throw new InvalidOperationException();

            //if (session == null)
            //{
            //    Owner = fileDescriptor.Owner;
            //    filePath = fileDescriptor.FilePath;
            //    DocumentTitle = fileDescriptor.FilePath.FileName;

            //    figmaDelegate = new FigmaDesignerDelegate();

            //    var localPath = Path.Combine(filePath.ParentDirectory.FullPath, FigmaBundle.ResourcesDirectoryName);

            //    fileProvider = new ControlFileNodeProvider(localPath) { File = filePath.FullPath };
            //    rendererService = new ControlViewRenderingService(fileProvider);

            //    //we generate a new file provider for embeded windows
            //    var tmpRemoteProvider = new FileNodeProvider(localPath) { File = filePath.FullPath };
            //    rendererService.CustomConverters.Add(new EmbededWindowConverter(tmpRemoteProvider) { LiveButtonAlwaysVisible = false });
            //    rendererService.CustomConverters.Add(new EmbededSheetDialogConverter(tmpRemoteProvider));

            //    layoutManager = new StoryboardLayoutManager();
            //    session = new FigmaDesignerSession(fileProvider, rendererService, layoutManager);
            //    //session.ModifiedChanged += HandleModifiedChanged;
            //    session.ReloadFinished += Session_ReloadFinished;

            //    surface = new FigmaDesignerSurface(figmaDelegate, session)
            //    {
            //        Session = session
            //    };

            //    surface.FocusedViewChanged += Surface_FocusedViewChanged;

            //    var window = NSApplication.SharedApplication.MainWindow;
            //    surface.SetWindow(new WindowInternalWrapper(window));
            //    surface.StartHoverSelection();

            //    //IdeApp.Workbench.ActiveDocumentChanged += OnActiveDocumentChanged;
                IdeApp.Workbench.DocumentOpened += OnDocumentOpened;
            //}
            //await RefreshAll();
            await base.OnInitialize(modelDescriptor, status);
        }

        //void Surface_FocusedViewChanged(object sender, IView e)
        //{
        //    var model = session.GetModel(e);
        //    if (model == null)
        //    {
        //        return;
        //    }

        //    if (outlinePad != null)
        //    {
        //        outlinePad.Focus(model);
        //    }
        //    //var currentWrapper = GetWrapper (model);

        //    DesignerSupport.DesignerSupport.Service.PropertyPad?.SetCurrentObject(model, new object[] { model });
        //    //PropertyPad.Instance.Control.CurrentObject = GetWrapper(model);
        //}

        //void Session_ReloadFinished(object sender, EventArgs e)
        //{
        //}

        private void OnDocumentOpened(object sender, DocumentEventArgs e)
        {
            //            UpdateLayout();
            this.contentView.NeedsLayout = true;
            this.contentView.NeedsDisplay = true;
        }

        //protected override Control OnGetViewControl(DocumentViewContent view)
        //{
        //}

        protected async override Task<Control> OnGetViewControlAsync(CancellationToken token, DocumentViewContent view)
        {
            // todo: I would have thought the document controller did this?
            this.contentView.SetQueryText(await File.ReadAllTextAsync(this.FilePath, token));

            return contentView;
        }

        //private void OnActiveDocumentChanged(object sender, EventArgs e)
        //{
        //    UpdateLayout();
        //}

        //string lastLayout;
        //private void UpdateLayout()
        //{
        //    var current = IdeApp.Workbench.ActiveDocument?.GetContent<string>();
        //    if (current == null)
        //    {
        //        if (lastLayout != null && IdeApp.Workbench.CurrentLayout == "Visual Design")
        //            IdeApp.Workbench.CurrentLayout = lastLayout;
        //        lastLayout = null;
        //    }
        //    else
        //    {
        //        if (IdeApp.Workbench.CurrentLayout != "Visual Design")
        //        {
        //            if (lastLayout == null)
        //            {
        //                lastLayout = IdeApp.Workbench.CurrentLayout;
        //                IdeApp.Workbench.CurrentLayout = "Visual Design";
        //            }
        //        }
        //        //current.widget.SetFocus();
        //    }
        //}

        //#region IOutlinedDocument

        //public Widget GetOutlineWidget()
        //{
        //    outlinePad = FigmaDesignerOutlinePad.Instance;
        //    outlinePad.GenerateTree(session.Response.document, figmaDelegate);

        //    outlinePad.RaiseFirstResponder += OutlinePad_RaiseFirstResponder;
        //    outlinePad.RaiseDeleteItem += OutlinePad_RaiseDeleteItem;
        //    return outlinePad;
        //}

        //async void OutlinePad_RaiseDeleteItem(object sender, FigmaNode e)
        //{
        //    HasUnsavedChanges = true;
        //    session.DeleteView(e);
        //    await RefreshAll();
        //}

        //ViewRenderServiceOptions fileOptions = new ViewRenderServiceOptions();

        //async Task RefreshAll()
        //{
        //    await session.ReloadAsync(scrollview.ContentView, filePath, fileOptions);
        //    if (outlinePad != null)
        //    {
        //        outlinePad.GenerateTree(session.Response.document, figmaDelegate);
        //        outlinePad.Focus(GetCurrentSelectedNode());
        //    }
        //}

        //FigmaNode GetCurrentSelectedNode()
        //{
        //    var selectedView = surface.SelectedView;
        //    var selectedModel = session.GetModel(selectedView);
        //    return selectedModel;
        //}

        //void OutlinePad_RaiseFirstResponder(object sender, FigmaNode e)
        //{
        //    var view = session.GetViewWrapper(e);
        //    surface.ChangeFocusedView(view);
        //}

        //public IEnumerable<Widget> GetToolbarWidgets()
        //{
        //    yield break;
        //}

        //public void ReleaseOutlineWidget()
        //{
        //    // throw new NotImplementedException();
        //}

        //#endregion

        ////#region ICustomPropertyPadProvider

        ////PropertyContentPad propertyPad;

        ////public Widget GetCustomPropertyWidget()
        ////{
        ////    PropertyPad.Initialize(session);
        ////    propertyPad = PropertyPad.Instance;
        ////    return propertyPad.Control;
        ////}

        ////public void DisposeCustomPropertyWidget()
        ////{
        ////    //throw new NotImplementedException();
        ////}

        ////#endregion

        protected override void OnDispose()
        {
//            surface.StopHover();
            base.OnDispose();
        }

        //public object GetActiveComponent()
        //{
        //    return GetCurrentSelectedNode();
        //}

        //public object GetProvider()
        //{
        //    return GetCurrentSelectedNode();
        //}

        //public void OnEndEditing(object obj)
        //{

        //}

        //public void OnChanged(object obj)
        //{

        //}
    }
}