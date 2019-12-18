using Sanet.SmartSkating.Xf;
using SimpleInjector;

namespace Sanet.SmartSkating.Tizen
{
    class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
    {
        private readonly Container _container = new Container();
        protected override void OnCreate()
        {
            base.OnCreate();
            _container.RegisterModules();
            LoadApplication(new App(_container));
        }

        static void Main(string[] args)
        {
            var app = new Program();
            Xamarin.Forms.Forms.Init(app);
            global::Tizen.Wearable.CircularUI.Forms.Renderer.FormsCircularUI.Init();
            app.Run(args);
        }
    }
}
