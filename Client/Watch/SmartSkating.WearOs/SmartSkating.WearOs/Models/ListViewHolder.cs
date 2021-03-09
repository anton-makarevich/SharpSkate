using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Sanet.SmartSkating.WearOs.Models
{
    public abstract class ListViewHolder<TViewModel>: RecyclerView.ViewHolder
    {
        protected ListViewHolder(View itemView) : base (itemView)
        {
        }

        public abstract void BindViewModel(TViewModel viewModel);
    }
}