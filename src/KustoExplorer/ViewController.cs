using System;

using AppKit;
using Foundation;
using KustoExplorer.Core;
using ObjCRuntime;

namespace KustoExplorer
{
	public partial class ViewController : NSViewController
	{
		public ViewController (NativeHandle handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.

			var x = new Class1();
			x.GetTest("print now()");
		}

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}
