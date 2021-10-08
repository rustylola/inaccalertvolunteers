using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using inaccalertvolunteers.Adapter;
using inaccalertvolunteers.DataModel;
using inaccalertvolunteers.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Fragments
{
    public class historyFragment : Android.Support.V4.App.Fragment
    {

        RecyclerView myrecyleview;
        ReportInfoListener reportinfo;
        List<HistoryDataModel> datamodellist;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.history, container, false);

            //initialize layouts
            myrecyleview = (RecyclerView)view.FindViewById(Resource.Id.recyclerView);

            //Getting all data from accident history firebase
            RetrievedData();
            
            return view;
        }

        private void SetupReCylceview()
        {
            myrecyleview.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(myrecyleview.Context));
            HistoryRecycleAdapter adapter = new HistoryRecycleAdapter(datamodellist);
            myrecyleview.SetAdapter(adapter);
        }

        public void RetrievedData()
        {
            reportinfo = new ReportInfoListener();
            reportinfo.Create();
            reportinfo.HistorydataRetrieve += Reportinfo_HistorydataRetrieve;
        }

        private void Reportinfo_HistorydataRetrieve(object sender, ReportInfoListener.HistoryEventArgs e)
        {
            datamodellist = e.data;
            SetupReCylceview();
        }
    }
}