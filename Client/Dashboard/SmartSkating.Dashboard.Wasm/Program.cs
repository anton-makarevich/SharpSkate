namespace Sanet.SmartSkating.Dashboard.Wasm
{
	public class Program
	{
		static int Main(string[] args)
		{
            Windows.UI.Xaml.Application.Start(_ => new Sanet.SmartSkating.Dashboard.UWP.App());

			return 0;
		}
    }
}
