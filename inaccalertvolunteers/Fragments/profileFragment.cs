using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using inaccalertvolunteers.Activity;
using inaccalertvolunteers.EventListeners;
using inaccalertvolunteers.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Fragments
{
    public class profileFragment : Android.Support.V4.App.Fragment
    {
        //define layouts
        TextView namevolun;
        TextView emailvolun;
        TextView phonevolun;
        TextView statusvolun;
        Button logoutbtn;

        //Alert Dialog inialize
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;

        //initialize firebase
        FirebaseAuth auth;

        //listener
        ProfileEventListener profile = new ProfileEventListener();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            View view = inflater.Inflate(Resource.Layout.profile, container, false);

            //initialize layouts
            namevolun = (TextView)view.FindViewById(Resource.Id.volunteernamehere);
            emailvolun = (TextView)view.FindViewById(Resource.Id.volunteeremailhere);
            phonevolun = (TextView)view.FindViewById(Resource.Id.volunteerphonehere);
            statusvolun = (TextView)view.FindViewById(Resource.Id.accountstathere);
            logoutbtn = (Button)view.FindViewById(Resource.Id.logoutbtn);
            //event
            logoutbtn.Click += Logoutbtn_Click;
            ShowVolunteerInformation();
            return view;
        }

        private void Logoutbtn_Click(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            alertDialog.SetTitle("Log out");
            alertDialog.SetMessage("Are you sure?");
            alertDialog.SetPositiveButton("Yes", (logout, args) => {
                showprogressDialog();
                auth = AppDataHelper.GetfirebaseAuth();
                profile.Logout();
                auth.SignOut();
                Intent intent = new Intent(Activity, typeof(loginActivity));
                StartActivity(intent);
                closeprogressDialog();
            }).SetNegativeButton("No", (logout, args) =>{
                return;
            });
            alertDialog.Show();
        }

        public void ShowVolunteerInformation()
        {
            string name = AppDataHelper.Getname();
            string email = AppDataHelper.Getemail();
            string phone = AppDataHelper.Getphone();
            string status = AppDataHelper.Getaccstatus();

            namevolun.Text = name;
            emailvolun.Text = email;
            statusvolun.Text = status;
            phonevolun.Text = phone;

        }
        void showprogressDialog()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            alert.SetView(Resource.Layout.progressdialogue);
            alert.SetCancelable(false);
            alertDialog = alert.Show();
        }

        void closeprogressDialog()
        {
            if (alert != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }
    }
}