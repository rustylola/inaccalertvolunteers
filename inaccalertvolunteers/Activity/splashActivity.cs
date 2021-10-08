using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using inaccalertvolunteers.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inaccalertvolunteers.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/splashtheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class splashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }

        protected override async void OnResume()
        {
            base.OnResume();

            FirebaseUser currentUser = AppDataHelper.Getcurrentuser();

            if (currentUser == null)
            {
                await SimulateStartup();
            }
            else
            {
                await SimulateMainActivity();
            }
        }

        async Task SimulateStartup()
        {
            await Task.Delay(TimeSpan.FromSeconds(8));
            StartActivity(typeof(Getstarted));
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
        }

        async Task SimulateMainActivity()
        {
            await Task.Delay(TimeSpan.FromSeconds(8));
            StartActivity(typeof(MainActivity));
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
        }
    }
}