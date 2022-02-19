using Avalonia.ReactiveUI;
using Avalonia.Web.Blazor;

namespace DashboardAvalonia.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        WebAppBuilder.Configure<DashboardAvalonia.App>()
            .UseReactiveUI()
            .SetupWithSingleViewLifetime();
    }
}