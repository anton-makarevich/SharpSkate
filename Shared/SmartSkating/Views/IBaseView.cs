using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.Views
{
    public interface IBaseView
    {
        object? ViewModel { get; set; }
    }

    public interface IBaseView<out TViewModel> : IBaseView where TViewModel : BaseViewModel
    {
        new TViewModel? ViewModel { get; }
    }
}
