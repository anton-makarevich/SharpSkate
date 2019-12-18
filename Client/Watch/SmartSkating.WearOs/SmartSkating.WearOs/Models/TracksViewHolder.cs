using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Sanet.SmartSkating.WearOs.Models
{
    public class TracksViewHolder: RecyclerView.ViewHolder
    {
        public TextView Name { get; private set; }

        public TracksViewHolder (View itemView, Action<int> listener) : base (itemView)
        {
            // Locate and cache view references:
            Name = itemView.FindViewById<TextView> (Resource.Id.textView);
            itemView.Click += (sender, args) => listener(LayoutPosition);
        }
    }
}