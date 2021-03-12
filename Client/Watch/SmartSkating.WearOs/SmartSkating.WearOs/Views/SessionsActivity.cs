using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.Wear.Widget;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.WearOs.Models;
using Sanet.SmartSkating.WearOs.Services;

namespace Sanet.SmartSkating.WearOs.Views
{
    [Activity]
    public class SessionsActivity: BaseActivity<SessionsViewModel>
    {
        private WearableRecyclerView? _recyclerView;
        private Button? _confirmButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_sessions);
            
            _recyclerView = FindViewById<WearableRecyclerView> (Resource.Id.recyclerViewSessions);
            _recyclerView.EdgeItemsCenteringEnabled = true;
            _confirmButton = FindViewById<Button>(Resource.Id.confirmButtonSessions);
            
            _confirmButton.Click += ConfirmButtonOnClick;
            
            SetViewModel();
            
            var layoutManager = new WearableLinearLayoutManager(this);
            _recyclerView.SetLayoutManager (layoutManager);
        }

        private void ConfirmButtonOnClick(object sender, EventArgs e)
        {
            ViewModel?.StartCommand?.Execute(null);
        }

        private void SetViewModel()
        {
            ViewModel = AndroidNavigationService.SharedInstance?.Container?.GetInstance<SessionsViewModel>();
            if (ViewModel == null) return;
            if (AndroidNavigationService.SharedInstance != null)
                ViewModel.SetNavigationService(AndroidNavigationService.SharedInstance);
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
            ViewModel.Sessions.CollectionChanged += TracksOnCollectionChanged;
            UpdateButtonsState();
        }

        private void TracksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_recyclerView?.GetAdapter() is ListAdapter<SessionDto,SessionsViewHolder> tracksAdapter)
                tracksAdapter.ItemClick-= AdapterOnItemClick;
            if (ViewModel == null) return;
            var adapter = new ListAdapter<SessionDto,SessionsViewHolder>(ViewModel.Sessions.ToList());
            adapter.ItemClick+= AdapterOnItemClick;
            _recyclerView?.SetAdapter(adapter);
        }

        private void AdapterOnItemClick(object sender, int e)
        {
            ViewModel?.SelectSession(ViewModel.Sessions[e]);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.CanStart))
            {
                UpdateButtonsState();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (ViewModel == null) return;
            ViewModel.Sessions.CollectionChanged -= TracksOnCollectionChanged;
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }
        
        private void UpdateButtonsState()
        {
            if (_confirmButton == null || ViewModel == null) return;
            _confirmButton.Enabled = ViewModel.CanStart;
        }
    }
}