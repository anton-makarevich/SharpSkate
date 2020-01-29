using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Sanet.SmartSkating.ViewModels.Wrappers;

namespace Sanet.SmartSkating.WearOs.Models
{
    public class TracksAdapter: RecyclerView.Adapter
    {
        public event EventHandler<int>? ItemClick;
        
        private readonly List<TrackViewModel> _tracks;

        public TracksAdapter (List<TrackViewModel> tracks)
        {
            _tracks = tracks;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is TracksViewHolder viewHolder) viewHolder.Name.Text = _tracks[position].Name;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From (parent.Context).
                Inflate (Resource.Layout.cell_track, parent, false);

            var viewHolder = new TracksViewHolder (itemView, OnClick);
            return viewHolder;
        }

        public override int ItemCount => _tracks.Count;
        
        void OnClick (int position)
        {
            ItemClick?.Invoke (this, position);
        }
    }
}