using AppKit;
using Sanet.SmartSkating.Dashboard;

namespace Sanet.SmartSkating.Dashboard.macOS
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.Delegate = new App();
            NSApplication.Main(args);
        }
    }
}
