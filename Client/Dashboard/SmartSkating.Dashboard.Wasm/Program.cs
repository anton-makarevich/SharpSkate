using System;
using Windows.UI.Xaml;
using Microsoft.Extensions.Logging;

namespace SmartSkating.Wasm
{
	public class Program
	{
		static int Main(string[] args)
		{
            Windows.UI.Xaml.Application.Start(_ => new SmartSkating.UWP.App());

			return 0;
		}
    }
}
