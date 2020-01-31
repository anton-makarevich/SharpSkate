using Windows.UI.Xaml;

namespace Dashboard.Wasm
{
    public static class Program
    {
        static int Main(string[] args)
        {
            Application.Start(_ => new App());

            return 0;
        }
    }
}
