// SmartSkating. Speed Skating activity tracker app
// ViewController.cs
// Copyrigh 2020 amakarevich anton.makarevich@hotmail.com
using System;

using AppKit;
using Foundation;

namespace SmartSkating.Dashboard.Mac
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
