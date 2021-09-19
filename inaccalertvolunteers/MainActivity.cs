using Android;
using Android.App;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Widget;
using inaccalertvolunteers.Adapter;
using inaccalertvolunteers.EventListeners;
using inaccalertvolunteers.Fragments;
using inaccalertvolunteers.Helper;
using System;

namespace inaccalertvolunteers
{
    [Activity(Label = "@string/app_name", Theme = "@style/logintheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        
        //define layouts in activity main xml
        ViewPager viewPager;
        BottomNavigationView bottomnavigation;
        Button onlinebtn;
        Android.Support.V7.Widget.Toolbar toolbar;

        //fragments
        profileFragment pFragment = new profileFragment();
        mapnotificationfragment mFragment = new mapnotificationfragment();
        historyFragment hFragment = new historyFragment();

        //Create permission
        const int requestID = 0;
        readonly string[] permissionGroup =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
        };

        //eventlisteners
        ProfileEventListener profileEventListener = new ProfileEventListener();
        AvailabilityListener availabilityListener;

        //Update Map inside AvailabilityListener
        Android.Locations.Location myLastLocation;
        LatLng myLatlng;

        //flags
        bool availabilitystatus;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            connectview();
            Frontfragment();
            checkPermission();
            profileEventListener.Create();
        }

        void Frontfragment()
        {
            viewPager.SetCurrentItem(1, true);
            toolbar.Visibility = Android.Views.ViewStates.Visible;
        }

        void connectview()
        {
            
            viewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
            toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.onlinetoolbar);
            //buttons and events
            onlinebtn = (Button)FindViewById(Resource.Id.onlinetbtn);
            onlinebtn.Click += Onlinebtn_Click;
            bottomnavigation = (BottomNavigationView)FindViewById(Resource.Id.bottom_nav);
            bottomnavigation.NavigationItemSelected += Bottomnavigation_NavigationItemSelected;
            //view pager limit note : 0 is included
            viewPager.OffscreenPageLimit = 2;
            viewPager.BeginFakeDrag();
            
            //set viewpager
            setupviewpager();

            //Fragment addlocation
            mFragment.CurrentLocation += mFragment_currentlocation;
        }

        void mFragment_currentlocation(object sender, LocationCallbackHelper.OnLocationCapturedEventArgs e)
        {
            myLastLocation = e.Location;
            myLatlng = new LatLng(myLastLocation.Latitude, myLastLocation.Longitude);

            if (availabilityListener != null)
            {
                availabilityListener.updateLocation(myLastLocation);
            }

            if (availabilitystatus && availabilityListener == null)
            {
                TakevolunteerOnline();
            }
        }

        private void TakevolunteerOnline()
        {
            availabilityListener = new AvailabilityListener();
            availabilityListener.Create(myLastLocation);
        }

        void TakevolunteerOffline()
        {
            availabilityListener.removeListener();
            availabilityListener = null;
        }

        private void Onlinebtn_Click(object sender, EventArgs e)
        {
            if (!checkPermission())
            {
                return;
            }

            if (availabilitystatus)
            {
                //When user wants to go offline
                //show alert dialog
                Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertDialog.SetTitle("GO OFFLINE");
                alertDialog.SetMessage("You are not Enable to Receive Accident Notification and Request");
                alertDialog.SetPositiveButton("Continue", (senderAlert, args) => {

                    availabilitystatus = false;
                    mFragment.GoOffline();
                    onlinebtn.Text = "Go Online";
                    TakevolunteerOffline();
                    onlinebtn.Background = ContextCompat.GetDrawable(this, Resource.Drawable.btnstyleplain);

                });
                alertDialog.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    alertDialog.Dispose();
                });

                alertDialog.Show();
            }
            else
            {
                //When user wants to go Online
                availabilitystatus = true;
                mFragment.GoOnline(); // mapnotificationfragment is already define so use mFragment
                onlinebtn.Text = "Go Offline";
                onlinebtn.Background = ContextCompat.GetDrawable(this, Resource.Drawable.btnstyleOnlineOffline);
            }
        }

        private void Bottomnavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            //Resource id if from Menu folder that state the navigation bottom preference id
            if (e.Item.ItemId == Resource.Id.profile)
            {
                viewPager.SetCurrentItem(0, true);
                toolbar.Visibility = Android.Views.ViewStates.Invisible;
            }
            else if (e.Item.ItemId == Resource.Id.mapnotification)
            {
                viewPager.SetCurrentItem(1, true);
                toolbar.Visibility = Android.Views.ViewStates.Visible;
            }
            else if (e.Item.ItemId == Resource.Id.history)
            {
                viewPager.SetCurrentItem(2, true);
                toolbar.Visibility = Android.Views.ViewStates.Invisible;
            }
        }

        private void setupviewpager()
        {
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.Addfragment(pFragment, "Profile");
            adapter.Addfragment(mFragment, "Map Notification");
            adapter.Addfragment(hFragment, "History");
            viewPager.Adapter = adapter;
        }
        // Check permission
        bool checkPermission()
        {
            bool permissionedGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(permissionGroup, requestID);
            }
            else
            {
                permissionedGranted = true;
            }
            return permissionedGranted;
        }

        void GoOnline()
        {
            
        }
    }
}