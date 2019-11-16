using System;
using Sanet.SmartSkating.Tools.GpxComposer.Models;

namespace Sanet.SmartSkating.Tools.GpxComposer
{
    class Program
    {
        static void Main(string[] args)
        {
            var coordinateReader = new CoordinatesReader();
            coordinateReader.ReadFromBackup();
            Console.WriteLine("Done! Press any key to exit");
            Console.ReadKey();
        }
    }
}