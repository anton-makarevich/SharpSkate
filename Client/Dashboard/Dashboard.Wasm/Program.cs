using Windows.UI.Xaml;
using SimpleInjector;

namespace Sanet.SmartSkating.Dashboard.Wasm
{
    public static class Program
    {
        private static readonly Container Container = new Container();
        static int Main(string[] args)
        {
            Container.RegisterUnoMainModule();
            Application.Start(_ => new App(Container));

            return 0;
        }
    }
}
