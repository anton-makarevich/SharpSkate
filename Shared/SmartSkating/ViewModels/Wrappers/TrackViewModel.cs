using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.ViewModels.Base;

namespace Sanet.SmartSkating.ViewModels.Wrappers
{
    public class TrackViewModel:BindableBase
    {
        private readonly TrackDto _model;
        private bool _isSelected;

        public TrackViewModel(TrackDto model)
        {
            _model = model;
        }

        public string Name => _model.Name;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}