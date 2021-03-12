using System;
using System.Collections.Generic;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Sanet.SmartSkating.WearOs.Models
{
    public class ListAdapter<TViewModel,TViewHolder>: RecyclerView.Adapter where TViewHolder:ListViewHolder<TViewModel>
    {
        public event EventHandler<int>? ItemClick;
        
        private readonly List<TViewModel> _items;

        public ListAdapter(List<TViewModel> items)
        {
            _items = items;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is TViewHolder viewHolder) viewHolder.BindViewModel(_items[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var viewHolder =Activator.CreateInstance(typeof(TViewHolder), parent) as TViewHolder;
            viewHolder.ItemView.Click += (sender, args) => OnClick(viewHolder.LayoutPosition);
            return viewHolder;
        }

        public override int ItemCount => _items.Count;

        private void OnClick (int position)
        {
            ItemClick?.Invoke (this, position);
        }
    }
}