using Avalonia;
using Avalonia.Markup.Xaml;
using Sanet.SmartSkating.Dashboard.Avalonia.Views.Base;
using Sanet.SmartSkating.ViewModels;

namespace Sanet.SmartSkating.Dashboard.Avalonia.Views;

public partial class SessionsView : BaseView<SessionsViewModel>
{
    public SessionsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (ViewModel != null) ViewModel.OnlyActiveSessions = false;
        base.OnAttachedToVisualTree(e);
    }
}
