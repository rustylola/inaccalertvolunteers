using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using inaccalertvolunteers.Helper;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.EventListeners
{
    public class SendAccidentReportListener : Java.Lang.Object, IValueEventListener
    {
        string usernamereport;
        string nameofvolunteer;
        string categoryofaccident;
        string accidentDescription;
        string addressofAccident;
        double lataccident;
        double lngaccident;

        FirebaseDatabase database;
        DatabaseReference AccidentReportRef;
        //ISharedPreferences preferences = Application.Context.GetSharedPreferences("volunteerHistory", FileCreationMode.Private);
        //ISharedPreferencesEditor editor;

        public SendAccidentReportListener(string nameofuser, string namevolunteer, string categoryaccident, string accidentdesc, string addressaccident,double lat,double lng)
        {
            usernamereport = nameofuser;
            nameofvolunteer = namevolunteer;
            categoryofaccident = categoryaccident;
            accidentDescription = accidentdesc;
            addressofAccident = addressaccident;
            lataccident = lat;
            lngaccident = lng;
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            
        }

        public void CreateReport()
        {
            database = AppDataHelper.Getdatabase();
            string uid = AppDataHelper.Getcurrentuser().Uid;
            DateTime dateTime = DateTime.Now;
            string dd = dateTime.ToString("dd");
            string mm = dateTime.ToString("MM");
            string yy = dateTime.ToString("yyyy");
            string hh = dateTime.Hour.ToString();
            string min = dateTime.Minute.ToString();

            AccidentReportRef = database.GetReference("Accident_Report").Push();

            HashMap reports = new HashMap();
            reports.Put("Name_user", usernamereport);
            reports.Put("Name_volunteer", nameofvolunteer);
            reports.Put("Accident_address", addressofAccident);
            reports.Put("Accident_Category", categoryofaccident);
            reports.Put("Accident_Description", accidentDescription);
            reports.Put("Date_Submitted", yy + "-" + mm + "-" + dd);
            reports.Put("Date_time", hh + ":" + min);

            reports.Put("latitude", lataccident);
            reports.Put("longitude", lngaccident);

            AccidentReportRef.SetValue(reports); // upload report in firebase
            CreateOwnCopy();
        }

        public void CreateOwnCopy()
        {
            database = AppDataHelper.Getdatabase();
            string uid = AppDataHelper.Getcurrentuser().Uid;
            DateTime dateTime = DateTime.Now;
            string dd = dateTime.ToString("dd");
            string mm = dateTime.ToString("MM");
            string yy = dateTime.ToString("yyyy");
            string hh = dateTime.Hour.ToString();
            string min = dateTime.Minute.ToString();

            AccidentReportRef = database.GetReference("volunteers/" + uid + "/accident_history").Push();

            HashMap reportsHistory = new HashMap();
            reportsHistory.Put("Name_user", usernamereport);
            reportsHistory.Put("Name_volunteer", nameofvolunteer);
            reportsHistory.Put("Accident_address", addressofAccident);
            reportsHistory.Put("Accident_Category", categoryofaccident);
            reportsHistory.Put("Accident_Description", accidentDescription);
            reportsHistory.Put("Date_Submitted", yy + "-" + mm + "-" + dd);
            reportsHistory.Put("Date_time", hh + ":" + min);

            AccidentReportRef.SetValue(reportsHistory); // upload history in firebase
        }
    }
}