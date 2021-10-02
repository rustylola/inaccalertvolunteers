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
    class AccidentDetailsListener : Java.Lang.Object, IValueEventListener
    {
        public class AccidentDetailsEventArgs : EventArgs
        {
            public AccidentDetails Accidentdatail { get; set; }
        }
        public event EventHandler<AccidentDetailsEventArgs> AccidentDetailFound;
        public event EventHandler AccidentDetailNotFound;
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                AccidentDetails accidentdatails = new AccidentDetails();
                accidentdatails.accidentAddress = snapshot.Child("userlocation_address").Value.ToString();
                accidentdatails.accidentLat = double.Parse(snapshot.Child("userlocation").Child("latitude").Value.ToString());
                accidentdatails.accidentLng = double.Parse(snapshot.Child("userlocation").Child("longitude").Value.ToString());

                accidentdatails.accidentID = snapshot.Key;
                accidentdatails.userName = snapshot.Child("user_name").Value.ToString();
                accidentdatails.userPhone = snapshot.Child("user_phone").Value.ToString();

                AccidentDetailFound?.Invoke(this, new AccidentDetailsEventArgs { Accidentdatail = accidentdatails });
            }
            else
            {
                AccidentDetailNotFound.Invoke(this, new EventArgs());
            }
        }

        public void Create(string accident_id)
        {
            FirebaseDatabase database = AppDataHelper.Getdatabase();
            DatabaseReference accidentdetailRef = database.GetReference("accidentRequest/" + accident_id);
            accidentdetailRef.AddListenerForSingleValueEvent(this);
        }
    }
}