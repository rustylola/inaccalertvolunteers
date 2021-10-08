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
    class ProfileEventListener : Java.Lang.Object, IValueEventListener
    {
        //define shared preference and create folder
        ISharedPreferences preferences = Application.Context.GetSharedPreferences("volunteerinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            //getting value from current user
            if(snapshot.Value != null)
            {
                string fullname, email, phone, accstatus;
                fullname = (snapshot.Child("name") != null) ? snapshot.Child("name").Value.ToString() : "";
                email = (snapshot.Child("email") != null) ? snapshot.Child("email").Value.ToString() : "";
                phone = (snapshot.Child("phone") != null) ? snapshot.Child("phone").Value.ToString() : "";
                accstatus = (snapshot.Child("acc_status") != null) ? snapshot.Child("acc_status").Value.ToString() : "";

                //save the value of current user in shared preference
                editor.PutString("name", fullname);
                editor.PutString("email", email);
                editor.PutString("phone", phone);
                editor.PutString("acc_status", accstatus);
                //apply to the define value - preferences 
                editor.Apply();
            }
        }

        public void Create()
        {
            editor = preferences.Edit();
            //initializing and creating reference of current user
            FirebaseDatabase database = AppDataHelper.Getdatabase();
            string volunteerID = AppDataHelper.Getcurrentuser().Uid;
            DatabaseReference volunteerRef = database.GetReference("volunteers/" + volunteerID);
            volunteerRef.AddValueEventListener(this);
        }

        public void Logout()
        {
            editor = preferences.Edit();
            editor.Clear();
        }
    }
}