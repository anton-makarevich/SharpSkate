using Sanet.SmartSkating.Dashboard.Wasm;
using SimpleInjector;

namespace Sanet.SmartSkating.Dashboard.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            var container = new Container();
            container.RegisterUnoMainModule();

            LoadApplication(new Sanet.SmartSkating.Dashboard.App(container));
        }
    }
}
