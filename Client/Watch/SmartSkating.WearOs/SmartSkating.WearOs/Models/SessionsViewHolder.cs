using Android.Views;
using Android.Widget;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.ViewModels.Wrappers;

namespace Sanet.SmartSkating.WearOs.Models
{
    public class SessionsViewHolder: ListViewHolder<SessionViewModel>
    {
        private TextView? Name { get; set; }

        public SessionsViewHolder (ViewGroup parent) : base (LayoutInflater.From (parent.Context).
            Inflate (Resource.Layout.cell_track, parent, false))
        {
            Name = ItemView.FindViewById<TextView> (Resource.Id.textView);
        }

        public override void BindViewModel(SessionViewModel viewModel)
        {
            if (Name != null) Name.Text = $"{viewModel.RinkName} {viewModel.StartTime}";
        }
    }
}