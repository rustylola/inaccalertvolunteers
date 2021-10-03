using Android;
using Android.App;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Widget;
using inaccalertvolunteers.Adapter;
using inaccalertvolunteers.DataModel;
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
        AccidentDialogueFragment accidentDialogueFragment;

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
        AccidentDetailsListener accidentDetailsListener;

        //Update Map inside AvailabilityListener
        Android.Locations.Location myLastLocation;
        LatLng myLatlng;

        //flags
        bool availabilitystatus;
        bool isBackground;
        bool newAccidentAssigned;

        //datamodel
        AccidentDetails newAccidentDetail;

        //Media player
        MediaPlayer musicplayer;
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
            //Toast display online
            Toast.MakeText(this, "This volunteer are now online.", ToastLength.Long).Show();
            availabilityListener.accidentAssigned += AvailabilityListener_accidentAssigned;
            availabilityListener.accidentCancelled += AvailabilityListener_accidentCancelled;
            availabilityListener.accidentTimeout += AvailabilityListener_accidentTimeout;
        }

        //Assign accident Event
        private void AvailabilityListener_accidentAssigned(object sender, AvailabilityListener.AccidentAssignedEventargs e)
        {
            Toast.MakeText(this, "New accident request assigned =" + e.accidentID, ToastLength.Short).Show();
            //Take details
            accidentDetailsListener = new AccidentDetailsListener();
            accidentDetailsListener.Create(e.accidentID);
            accidentDetailsListener.AccidentDetailFound += AccidentDetailsListener_AccidentDetailFound;
            accidentDetailsListener.AccidentDetailNotFound += AccidentDetailsListener_AccidentDetailNotFound;
        }

        //Accident Found event
        private void AccidentDetailsListener_AccidentDetailFound(object sender, AccidentDetailsListener.AccidentDetailsEventArgs e)
        {
            newAccidentDetail = e.Accidentdatail;
            if (!isBackground)
            {
                CreateAccidentRequestDialogue();
            }
            else
            {
                newAccidentAssigned = true;
                NotificationHelper notificationHelper = new NotificationHelper();
                if ((int)Build.VERSION.SdkInt >= 26)
                {
                    notificationHelper.NotifyVersion26(this, Resources, (NotificationManager)GetSystemService(NotificationService));
                }
                else
                {
                    notificationHelper.NotifyOtherVersion(this, Resources, (NotificationManager)GetSystemService(NotificationService));
                }
                
            }

        }
        void CreateAccidentRequestDialogue()
        {
            accidentDialogueFragment = new AccidentDialogueFragment(newAccidentDetail.userName, newAccidentDetail.accidentAddress);
            accidentDialogueFragment.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            accidentDialogueFragment.Show(trans, "Request");

            //Play Music Alert
            musicplayer = MediaPlayer.Create(this, Resource.Raw.AccidentAlert);
            musicplayer.Start();
        }

        //Accident Not Found event
        private void AccidentDetailsListener_AccidentDetailNotFound(object sender, EventArgs e)
        {
            
        }

        //Timeout accident event
        private void AvailabilityListener_accidentTimeout(object sender, EventArgs e)
        {
            if (accidentDialogueFragment != null)
            {
                accidentDialogueFragment.Dismiss();
                accidentDialogueFragment = null;
                musicplayer.Stop();
                musicplayer = null;
            }

            Toast.MakeText(this, "Accident request Timeout assigned", ToastLength.Short).Show();
            availabilityListener.ReActivate();
        }

        //Cancelled accident event
        private void AvailabilityListener_accidentCancelled(object sender, EventArgs e)
        {
            if (accidentDialogueFragment != null)
            {
                accidentDialogueFragment.Dismiss();
                accidentDialogueFragment = null;
                musicplayer.Stop();
                musicplayer = null;
            }

            Toast.MakeText(this, "Accident request Cancelled assigned", ToastLength.Short).Show();
            availabilityListener.ReActivate();
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
                    onlinebtn.SetTextColor(Color.Black);
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
                //Toast.MakeText(this, "Enable to Receive Accident Request", ToastLength.Short).Show();
                onlinebtn.Text = "Go Offline";
                onlinebtn.SetTextColor(Color.White);
                onlinebtn.Background = ContextCompat.GetDrawable(this, Resource.Drawable.btnOffline);
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

        protected override void OnPause()
        {
            isBackground = true;
            base.OnPause();
        }
        protected override void OnResume()
        {
            isBackground = false;
            if (newAccidentAssigned)
            {
                CreateAccidentRequestDialogue();
                newAccidentAssigned = false;
            }
            base.OnResume();
        }
    }
}