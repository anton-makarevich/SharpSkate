using Avalonia.Markup.Xaml;
using Sanet.SmartSkating.Dashboard.Avalonia.Views.Base;
using Sanet.SmartSkating.ViewModels;

namespace Sanet.SmartSkating.Dashboard.Avalonia.Views;

public partial class SessionDetailsView : BaseView<SessionDetailsViewModel>
{
    public SessionDetailsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
