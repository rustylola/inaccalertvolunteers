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
    public class CheckEmailListener : Java.Lang.Object,IValueEventListener
    {
        FirebaseDatabase database;
        DatabaseReference reference;
        string emailchecking;

        public event EventHandler Existed;

        public CheckEmailListener(string email)
        {
            emailchecking = email;
        }
        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach (DataSnapshot data in child)
                {
                    if (data.Child("email").Value != null)
                    {
                        if (data.Child("email").Value.ToString() == emailchecking)
                        {
                            Existed?.Invoke(this, new EventArgs());
                        }
                    }
                }
            }
        }

        public void CheckEmail()
        {
            database = AppDataHelper.Getdatabase();
            reference = database.GetReference("volunteers");
            reference.AddListenerForSingleValueEvent(this);
        }
    }
}