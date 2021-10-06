using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using inaccalertvolunteers.DataModel;
using System;
using System.Collections.Generic;

namespace inaccalertvolunteers.Adapter
{
    class HistoryRecycleAdapter : RecyclerView.Adapter
    {
        public event EventHandler<HistoryRecycleAdapterClickEventArgs> ItemClick;
        public event EventHandler<HistoryRecycleAdapterClickEventArgs> ItemLongClick;
        List<HistoryDataModel> items;

        public HistoryRecycleAdapter(List<HistoryDataModel> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.listViewDataTemplate, parent, false);
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);

            var vh = new HistoryRecycleAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
           
            var holder = viewHolder as HistoryRecycleAdapterViewHolder;
            //holder.TextView.Text = items[position];
            holder.UsersNametext.Text = items[position].UsersName;
            holder.VolunteerNametext.Text = items[position].VolunteerName;
            holder.AccidentAddresstext.Text = items[position].AccidentAddress;
            holder.AccidentCategtext.Text = items[position].AccidentCateg;
            holder.AccidentDescriptiontext.Text = items[position].AccidentDescription;
            holder.dates.Text = items[position].Accidentdate;
        }

        public override int ItemCount => items.Count;

        void OnClick(HistoryRecycleAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(HistoryRecycleAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class HistoryRecycleAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView UsersNametext { get; set; }
        public TextView VolunteerNametext { get; set; }
        public TextView AccidentAddresstext { get; set; }
        public TextView AccidentCategtext { get; set; }
        public TextView AccidentDescriptiontext { get; set; }
        public TextView dates { get; set; }

        public HistoryRecycleAdapterViewHolder(View itemView, Action<HistoryRecycleAdapterClickEventArgs> clickListener,
                            Action<HistoryRecycleAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            UsersNametext = (TextView)itemView.FindViewById(Resource.Id.username);
            VolunteerNametext = (TextView)itemView.FindViewById(Resource.Id.volunteername);
            AccidentAddresstext = (TextView)itemView.FindViewById(Resource.Id.addressname);
            AccidentCategtext = (TextView)itemView.FindViewById(Resource.Id.categoryname);
            AccidentDescriptiontext = (TextView)itemView.FindViewById(Resource.Id.descriptionname);
            dates = (TextView)itemView.FindViewById(Resource.Id.datetitle);

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