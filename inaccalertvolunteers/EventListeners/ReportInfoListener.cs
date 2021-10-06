using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using inaccalertvolunteers.DataModel;
using inaccalertvolunteers.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.EventListeners
{
    class ReportInfoListener : Java.Lang.Object, IValueEventListener
    {

        List<HistoryDataModel> datalist = new List<HistoryDataModel>();

        public class HistoryEventArgs : EventArgs
        {
            public List<HistoryDataModel> data { get; set; }
        }

        public event EventHandler<HistoryEventArgs> HistorydataRetrieve;

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                datalist.Clear();
                foreach (DataSnapshot reportdata in child)
                {
                    if (reportdata.Child("accident_history") != null)
                    {
                        HistoryDataModel datamodel = new HistoryDataModel();
                        datamodel.uID = reportdata.Key;
                        datamodel.UsersName = reportdata.Child("Name_user").Value.ToString();
                        datamodel.VolunteerName = reportdata.Child("Name_volunteer").Value.ToString();
                        datamodel.AccidentAddress = reportdata.Child("Accident_address").Value.ToString();
                        datamodel.AccidentCateg = reportdata.Child("Accident_Category").Value.ToString();
                        datamodel.AccidentDescription = reportdata.Child("Accident_Description").Value.ToString();
                        datamodel.Accidentdate = reportdata.Child("Date_Submitted").Value.ToString();
                        datalist.Add(datamodel);
                    }
                }
                HistorydataRetrieve.Invoke(this, new HistoryEventArgs { data = datalist });
            }
        }

        public void Create()
        {
            string currentuser = AppDataHelper.Getcurrentuser().Uid;
            DatabaseReference database = AppDataHelper.Getdatabase().GetReference("volunteers/" + currentuser + "/accident_history");
            database.AddValueEventListener(this);
        }
    }
}