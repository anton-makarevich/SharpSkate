using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Sanet.SmartSkating.Models;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.ViewModels.Base;
using Sanet.SmartSkating.ViewModels.Wrappers;

namespace Sanet.SmartSkating.ViewModels
{
    public class TracksViewModel:BaseViewModel
    {
        private readonly ITrackService _trackService;

        public TracksViewModel(ITrackService trackService)
        {
            _trackService = trackService;
        }

        public ObservableCollection<TrackViewModel> Tracks { get; } = new ObservableCollection<TrackViewModel>();

        public bool HasSelectedTrack => Tracks.Any(t => t.IsSelected);
        public ICommand ConfirmSelectionCommand => new SimpleCommand(async () =>
        {
            if (!HasSelectedTrack || _trackService.SelectedRink == null) return;
            await NavigationService.NavigateToViewModelAsync<SessionsViewModel>();
        });

        public async Task LoadTracksAsync()
        {
            await _trackService.LoadTracksAsync();
            foreach (var track in _trackService.Tracks)
            {
                if (Tracks.All(t => t.Name != track.Name))
                    Tracks.Add(new TrackViewModel(track));
            }

            if (Tracks.Count == 1)
            {
                SelectTrack(Tracks.First());
                ConfirmSelectionCommand.Execute(null);
            }
        }

        public override void AttachHandlers()
        {
            base.AttachHandlers();
#pragma warning disable 4014
            LoadTracksAsync();
#pragma warning restore 4014
        }

        public void SelectTrack(TrackViewModel track)
        {
            track.IsSelected = false;
            foreach (var trackViewModel in Tracks)
            {
                trackViewModel.IsSelected = track == trackViewModel;
            }

            if (!track.IsSelected) return;
            _trackService.SelectRinkByName(Tracks.Single(t => t.IsSelected).Name);
            NotifyPropertyChanged(nameof(HasSelectedTrack));
        }
    }
}
