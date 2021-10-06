using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace inaccalertvolunteers.Adapter
{
    class HistoryRecycleAdapter : RecyclerView.Adapter
    {
        public event EventHandler<HistoryRecycleAdapterClickEventArgs> ItemClick;
        public event EventHandler<HistoryRecycleAdapterClickEventArgs> ItemLongClick;
        string[] items;

        public HistoryRecycleAdapter(string[] data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);

            var vh = new HistoryRecycleAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as HistoryRecycleAdapterViewHolder;
            //holder.TextView.Text = items[position];
        }

        public override int ItemCount => items.Length;

        void OnClick(HistoryRecycleAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(HistoryRecycleAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class HistoryRecycleAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }


        public HistoryRecycleAdapterViewHolder(View itemView, Action<HistoryRecycleAdapterClickEventArgs> clickListener,
                            Action<HistoryRecycleAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            itemView.Click += (sender, e) => clickListener(new HistoryRecycleAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new HistoryRecycleAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class HistoryRecycleAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}