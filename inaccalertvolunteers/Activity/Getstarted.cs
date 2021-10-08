using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Activity
{
    [Activity(Label = "Getstarted")]
    public class Getstarted : AppCompatActivity 
    {
        Button getstartbtn;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.AppInformation);
            getstartbtn = (Button)FindViewById(Resource.Id.getstarted);
            getstartbtn.Click += Getstartbtn_Click;
        }

        private void Getstartbtn_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(loginActivity)));
            OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.FadeOut);
        }
    }
}