using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Fragments
{
    public class AccidentDialogueFragment : Android.Support.V4.App.DialogFragment
    {
        //define layouts
        TextView nameuser;
        TextView addressuser;
        Button acceptbtn;
        Button rejectbtn;

        string nameofuser;
        string addressofuser;

        //event
        public event EventHandler VolunteerAccepted;
        public event EventHandler VolunteerRejected;
        public AccidentDialogueFragment(string name, string address)
        {
            nameofuser = name;
            addressofuser = address;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.AccidentDialogue, container, false);
            //initialize layouts
            nameuser = (TextView)view.FindViewById(Resource.Id.requestname);
            addressuser = (TextView)view.FindViewById(Resource.Id.locationaddress);
            rejectbtn = (Button)view.FindViewById(Resource.Id.rejecyBtn);
            acceptbtn = (Button)view.FindViewById(Resource.Id.acceptBtn);

            //display value
            nameuser.Text = nameofuser;
            addressuser.Text = addressofuser;

            //button events
            rejectbtn.Click += Rejectbtn_Click;
            acceptbtn.Click += Acceptbtn_Click;
            return view;
        }
        //accept request event
        private void Acceptbtn_Click(object sender, EventArgs e)
        {
            VolunteerAccepted?.Invoke(this, new EventArgs());
        }
        //reject request event
        private void Rejectbtn_Click(object sender, EventArgs e)
        {
            VolunteerRejected?.Invoke(this, new EventArgs());
        }
    }
}