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
    class TextViewController : NSViewController
    {
        protected NSTextView textView;

        public TextViewController()
        {
            var scrollView = new NSScrollView()
            {
                //TranslatesAutoresizingMaskIntoConstraints = false,
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                DrawsBackground = false,
                BorderType = NSBorderType.GrooveBorder
            };
            View = scrollView;
            // todo: we want this to be an editor
            this.textView = new NSTextView() { Frame = scrollView.Bounds };
            scrollView.DocumentView = textView;

            textView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
            textView.TextContainer.Size = new CoreGraphics.CGSize(scrollView.ContentSize.Width, float.MaxValue);
            textView.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);

        }

        public void SetText(string text)
        {
            textView.TextStorage.SetString(new NSAttributedString(text));
        }
    }

    class ResultsViewViewController : TextViewController
    {
        public ResultsViewViewController()
        {
        }
    }

    class QueryViewController : TextViewController
    {
        public QueryViewController()
        {
        }

        internal string GetQueryScript()
        {
            return this.textView.TextStorage.Value;
        }
    }

    static class NSViewExtensions
    {
        public static void AddArrangedSubviewAndAttachHorizontally(this NSStackView parent, NSView view, int margins = 0)
        {
            parent.AddArrangedSubview(view);
            AttachViewHorizontally(parent, view, margins);
        }

        public static void AttachViewHorizontally(this NSView parent, NSView view, int margins = 0)
        {
            view.LeadingAnchor.ConstraintLessThanOrEqualTo(parent.LeadingAnchor, margins).Active = true;
            view.TrailingAnchor.ConstraintEqualTo(parent.TrailingAnchor, -margins).Active = true;
        }

        public static void CreateFlexibleSpace(this NSStackView view)
        {
            view.AddArrangedSubview(new NSView() { TranslatesAutoresizingMaskIntoConstraints = false });
        }
    }

    class QueryAndResultsViewController : NSSplitViewController
    {
        private QueryViewController queryViewController;
        private ResultsViewViewController resultsViewController;

        public QueryAndResultsViewController()
        {
            this.View.TranslatesAutoresizingMaskIntoConstraints = false;
            this.queryViewController = new QueryViewController();
            this.resultsViewController = new ResultsViewViewController();

            var item = new NSSplitViewItem() { MinimumThickness = 50 };
            item.ViewController = this.queryViewController;

            this.AddSplitViewItem(item);

            item = new NSSplitViewItem() {  MinimumThickness = 50};
            item.ViewController = this.resultsViewController;
            this.AddSplitViewItem(item);

            this.queryViewController.SetText("Q");
            this.resultsViewController.SetText("R");
        }

        internal string GetQueryScript()
        {
            return this.queryViewController.GetQueryScript();
        }

        internal void SetQueryText(string text)
        {
            this.queryViewController.SetText(text);
        }

        internal void SetResultsText(string text)
        {
            this.resultsViewController.SetText(text);
        }
    }

    class KustoViewController : NSViewController
    {
        private QueryAndResultsViewController queryAndResultsViewController;
        private NSStackView toolbar;
        private NSButton runButton;

        NSStackView CreateHorizontalStackView(int spacing = 10)
        {
            return new NSStackView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
                Distribution = NSStackViewDistribution.Fill,
                Alignment = NSLayoutAttribute.CenterY,
                Spacing = spacing,
            };
        }

        public KustoViewController()
        {
            var stackView = new NSStackView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Alignment = NSLayoutAttribute.Leading,
                Distribution = NSStackViewDistribution.Fill
            };
            View = stackView;
            stackView.EdgeInsets = new NSEdgeInsets(10, 10, 0, 10);

            //toolbar
            this.toolbar = CreateHorizontalStackView();
            //toolbar.WantsLayer = true;
            //toolbar.Layer.BackgroundColor = NSColor.Blue.CGColor;
            stackView.AddArrangedSubviewAndAttachHorizontally(toolbar, 10);

            this.runButton = new NSButton() { TranslatesAutoresizingMaskIntoConstraints = false, BezelStyle = NSBezelStyle.RoundRect, Title = "Query" };
            this.toolbar.AddArrangedSubview(runButton);
            toolbar.CreateFlexibleSpace(); //creates an flexible area to don't force other views to grow

            this.queryAndResultsViewController = new QueryAndResultsViewController();
            stackView.AddArrangedSubviewAndAttachHorizontally(queryAndResultsViewController.View);
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            // todo: selector
            this.runButton.Activated += RunButton_Activated;

            this.View.NeedsLayout = true;
            this.View.NeedsDisplay = true;
        }

        public override void ViewWillDisappear()
        {
            base.ViewWillDisappear();
            this.runButton.Activated -= RunButton_Activated;
        }

        private void RunButton_Activated(object sender, EventArgs e)
        {
            // run the script
            var x = new KustoTest();
            var y = x.GetTest(this.queryAndResultsViewController.GetQueryScript());
            this.SetResultText(y);
        }

        public void SetQueryText(string text)
        {
            this.queryAndResultsViewController.SetQueryText(text);
        }

        public void SetResultText(string text)
        {
            this.queryAndResultsViewController.SetResultsText(text);
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
        private KustoViewController contentViewController;
        private FilePath filePath;
        public KustoQueryFileDocumentController()
        {
            contentViewController = new KustoViewController();
        }

        protected override Task OnSave()
        {
            return Task.FromResult(true);
        }

        protected override async Task OnInitialize(ModelDescriptor modelDescriptor, Properties status)
        {
            if (!(modelDescriptor is FileDescriptor fileDescriptor))
                throw new InvalidOperationException();

            IdeApp.Workbench.DocumentOpened += OnDocumentOpened;

            await base.OnInitialize(modelDescriptor, status);
        }

        CancellationTokenSource cancellationTokenSource;

        private void OnDocumentOpened(object sender, DocumentEventArgs e)
        {
             this.contentViewController.SetQueryText(File.ReadAllText(this.FilePath));
        }

        protected override Control OnGetViewControl(DocumentViewContent view)
        {
            return contentViewController.View;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}