using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using inaccalertvolunteers.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.EventListeners
{
    public class NewAccidentRequestListener : Java.Lang.Object, IValueEventListener
    {
        string mAccidentid;
        Android.Locations.Location mLastlocation;
        FirebaseDatabase database;
        DatabaseReference Accidentref;
        bool isAccepted;

        public NewAccidentRequestListener(string accident_id, Android.Locations.Location lastlocation)
        {
            mAccidentid = accident_id;
            mLastlocation = lastlocation;
            database = AppDataHelper.Getdatabase();

        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                if (!isAccepted)
                {
                    isAccepted = true;
                    Accept();
                }
            }
        }
        public void Create()
        {
            Accidentref = database.GetReference("accidentRequest/" + mAccidentid);
            Accidentref.AddValueEventListener(this); //addvalueeventlistener
        }
        void Accept()
        {
            Accidentref.Child("status").SetValue("accept");
            Accidentref.Child("volunteer_name").SetValue(AppDataHelper.Getname());
            Accidentref.Child("volunteer_phone").SetValue(AppDataHelper.Getphone());
            Accidentref.Child("volunteerlocation").Child("latitude").SetValue(mLastlocation.Latitude);
            Accidentref.Child("volunteerlocation").Child("longitude").SetValue(mLastlocation.Longitude);
            Accidentref.Child("volunteerID").SetValue(AppDataHelper.Getcurrentuser().Uid);
        }

        public void UpdateLocation(Android.Locations.Location lastlocation)
        {
            mLastlocation = lastlocation;
            Accidentref.Child("volunteerlocation").Child("latitude").SetValue(mLastlocation.Latitude);
            Accidentref.Child("volunteerlocation").Child("longitude").SetValue(mLastlocation.Longitude);
        }
    }
}