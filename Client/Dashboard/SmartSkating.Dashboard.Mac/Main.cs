﻿// SmartSkating. Speed Skating activity tracker app
// Main.cs
// Copyrigh 2020 amakarevich anton.makarevich@hotmail.com

using AppKit;

namespace Sanet.SmartSkating.Dashboard.Mac
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.Delegate = new AppDelegate();
            NSApplication.Main(args);
        }
    }
}
