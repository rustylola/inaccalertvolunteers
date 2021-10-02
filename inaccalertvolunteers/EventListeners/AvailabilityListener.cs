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
    public class AvailabilityListener : Java.Lang.Object, IValueEventListener
    {
        FirebaseDatabase database;
        DatabaseReference availabilityref;

        public class AccidentAssignedEventargs : EventArgs
        {
            public string accidentID { get; set; }
        }
        public event EventHandler<AccidentAssignedEventargs> accidentAssigned;
        public event EventHandler accidentCancelled;
        public event EventHandler accidentTimeout;
        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                string accident_id = snapshot.Child("accident_id").Value.ToString();

                if (accident_id != "waiting" && accident_id != "timeout" && accident_id != "cancelled")
                {
                    //assigned
                    accidentAssigned?.Invoke(this, new AccidentAssignedEventargs{ accidentID = accident_id});
                }
                else if (accident_id == "timeout")
                {
                    //ride timeout
                    accidentTimeout?.Invoke(this, new EventArgs());
                }
                else if (accident_id == "cancelled")
                {
                    //ride cancelled
                    accidentCancelled?.Invoke(this, new EventArgs());
                }
            }
        }

        public void Create(Android.Locations.Location mylocation)
        {
            //when volunteer online, it will create information and location in firebasedatabase

            database = AppDataHelper.Getdatabase();
            string volunteerID = AppDataHelper.Getcurrentuser().Uid;
            availabilityref = database.GetReference("volunteerAvailable/" + volunteerID);

            HashMap location = new HashMap();
            location.Put("latitude", mylocation.Latitude);
            location.Put("longitude", mylocation.Longitude);

            HashMap volunteerInfo = new HashMap();
            volunteerInfo.Put("location", location);
            volunteerInfo.Put("accident_id", "waiting");

            availabilityref.AddValueEventListener(this);
            availabilityref.SetValue(volunteerInfo);
        }

        public void removeListener()
        {
            // this will offline the volunteer and remove information to the firebasedatabase
            availabilityref.RemoveValue();
            availabilityref.RemoveEventListener(this);
            availabilityref = null;
        }

        public void updateLocation(Android.Locations.Location myUpdateLocation)
        {
            string volunteerID = AppDataHelper.Getcurrentuser().Uid;
            // this will check if the user is offline, it will not update the location in firebase
            // but if online, the location inside the firebasedatabase will continue to update
            if (availabilityref != null)
            {
                DatabaseReference locationref = database.GetReference("volunteerAvailable/" + volunteerID + "/location");
                HashMap locationMap = new HashMap();
                locationMap.Put("latitude", myUpdateLocation.Latitude);
                locationMap.Put("longitude", myUpdateLocation.Longitude);
                locationref.SetValue(locationMap);
            }
        }
        public void ReActivate()
        {
            availabilityref.Child("accident_id").SetValue("waiting");
        }
    }
}