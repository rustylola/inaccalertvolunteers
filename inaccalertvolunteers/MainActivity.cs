using Android;
using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using inaccalertvolunteers.Adapter;
using inaccalertvolunteers.DataModel;
using inaccalertvolunteers.EventListeners;
using inaccalertvolunteers.Fragments;
using inaccalertvolunteers.Helper;
using Plugin.Connectivity;
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
        NewAccidentRequestListener NewAccidentRequestListener;

        //Update Map inside AvailabilityListener
        Android.Locations.Location myLastLocation;
        LatLng myLatlng;

        //flags
        bool availabilitystatus;
        bool isBackground;
        bool newAccidentAssigned;
        string status = "NORMAL"; //REQUESTFOUND , ACCEPTED , ONTRIP 
        int oneRequestatatime = 0;
        //datamodel
        AccidentDetails newAccidentDetail;

        //Media player
        MediaPlayer musicplayer;

        //helper
        MapFunctionHelper mapHelper;
        MakeReportsFragment reportfragment;

        //Alert Dialog inialize
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            profileEventListener.Create();
            connectview();
            Frontfragment();
            checkPermission();

            //Do something here if user acc_status is processing
            //if processing unable the user to online
            //pFragment.ShowVolunteerInformation(AppDataHelper.Getname(), AppDataHelper.Getemail(), AppDataHelper.Getphone(), AppDataHelper.Getaccstatus());
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
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "";
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

            //Map notification fragment Events
            mFragment.CurrentLocation += mFragment_currentlocation;
            mFragment.VolunteerAccidentArrive += MFragment_VolunteerAccidentArrive;
            mFragment.CallUser += MFragment_CallUser;
            mFragment.Makereport += MFragment_Makereport;
            mFragment.DirectionGoogle += MFragment_DirectionGoogle;
        }
        //navigation with google
        private void MFragment_DirectionGoogle(object sender, EventArgs e)
        {
            string uri = "";
            if (status == "ACCEPTED")
            {
                uri = "google.navigation:q=" + newAccidentDetail.accidentLat.ToString() + "," + newAccidentDetail.accidentLng.ToString();
            }

            Android.Net.Uri googlemapIntentUri = Android.Net.Uri.Parse(uri);
            Intent mapIntent = new Intent(Intent.ActionView, googlemapIntentUri);
            mapIntent.SetPackage("com.google.android.apps.maps");
            try
            {
                StartActivity(mapIntent);
            }
            catch
            {
                Toast.MakeText(this, "Google Map is not Installed.", ToastLength.Long).Show();
            }
        }

        //toolbar menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu, menu);

            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.sidemenufirst)
            {
                Toast.MakeText(this, "first", ToastLength.Long).Show();
            }
            else if(item.ItemId == Resource.Id.sidemenusecond)
            {
                Toast.MakeText(this, "second", ToastLength.Long).Show();
            }
            return base.OnOptionsItemSelected(item);
        }

        //Do or make report
        private void MFragment_Makereport(object sender, EventArgs e)
        {
            status = "NORMAL";
            //input first all the information for report
            //HERE
            showprogressDialog();
            string nameofuseraccident = newAccidentDetail.userName;
            string nameofvolunteer = AppDataHelper.Getname();
            string accidentaddress = newAccidentDetail.accidentAddress;
            closeprogressDialog();
            reportfragment = new MakeReportsFragment(nameofuseraccident,nameofvolunteer,accidentaddress);
            reportfragment.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            reportfragment.Show(trans, "Report Sending");
            reportfragment.ReportSend += (o, u) =>
            {
                reportfragment.Dismiss();
            };

            //reset app
            mFragment.ResetAfterReport();
            //Accident List of Information
            NewAccidentRequestListener.AccidentEnded();
            NewAccidentRequestListener = null;
            availabilityListener.ReActivate();
            onlinebtn.Enabled = true; // offline button will enable again
        }

        //Call User event
        private void MFragment_CallUser(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("tel:" + newAccidentDetail.userPhone);
            Intent intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
        }

        //Arrive event
        private void MFragment_VolunteerAccidentArrive(object sender, EventArgs e)
        {
            //notifies user
            NewAccidentRequestListener.updatestatus("arrive");
            status = "ARRIVE";
            //LatLng accidentlatlng = new LatLng(newAccidentDetail.accidentLat, newAccidentDetail.accidentLng);
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

            if (status == "ACCEPTED")
            {
                //update or animate movement
                LatLng accidentLatlng = new LatLng(newAccidentDetail.accidentLat, newAccidentDetail.accidentLng);
                mapHelper.UpdateMovement(myLatlng, accidentLatlng, "Accident Location");
                //update location from firebase 
                NewAccidentRequestListener.UpdateLocation(myLastLocation);
            }
            //if(status == "ARRIVE")
            //{
                //Do something
            //}
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
            oneRequestatatime += 1;
            //Show Assign ID
            //Toast.MakeText(this, "New accident request assigned =" + e.accidentID, ToastLength.Short).Show();
            //Take details
            accidentDetailsListener = new AccidentDetailsListener();
            accidentDetailsListener.Create(e.accidentID);
            accidentDetailsListener.AccidentDetailFound += AccidentDetailsListener_AccidentDetailFound;
            accidentDetailsListener.AccidentDetailNotFound += AccidentDetailsListener_AccidentDetailNotFound;
        }

        //Accident Found event
        private void AccidentDetailsListener_AccidentDetailFound(object sender, AccidentDetailsListener.AccidentDetailsEventArgs e)
        {
            if (status != "NORMAL")
            {
                return;
            }
            if (oneRequestatatime > 1)
            {
                return;
            }
            //Play Music Alert
            musicplayer = MediaPlayer.Create(this, Resource.Raw.AccidentAlert);
            musicplayer.Start();
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
            }

        }
        void CreateAccidentRequestDialogue()
        {
            accidentDialogueFragment = new AccidentDialogueFragment(newAccidentDetail.userName, newAccidentDetail.accidentAddress);
            accidentDialogueFragment.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            accidentDialogueFragment.Show(trans, "Request");

            //Play Music Alert
            // move up
            accidentDialogueFragment.VolunteerAccepted += AccidentDialogueFragment_VolunteerAccepted;
            accidentDialogueFragment.VolunteerRejected += AccidentDialogueFragment_VolunteerRejected;
        }

        //rejected
        private void AccidentDialogueFragment_VolunteerRejected(object sender, EventArgs e)
        {
            //stop alert
            if (musicplayer != null)
            {
                musicplayer.Stop();
                musicplayer = null;
            }
            //dismiss dialogue
            if (accidentDialogueFragment != null)
            {
                accidentDialogueFragment.Dismiss();
                accidentDialogueFragment = null;
            }
            //return driver online
            oneRequestatatime = 0; // put it to zero
            availabilityListener.ReActivate();
            
        }

        //accepted
        async void AccidentDialogueFragment_VolunteerAccepted(object sender, EventArgs e)
        {
            NewAccidentRequestListener = new NewAccidentRequestListener(newAccidentDetail.accidentID, myLastLocation);
            NewAccidentRequestListener.Create();
            status = "ACCEPTED";
            onlinebtn.Enabled = false; // in order to not suddenly offline once the user accept it
            //stop alert
            if (musicplayer != null)
            {
                musicplayer.Stop();
                musicplayer = null;
            }
            //dismiss dialogue
            if (accidentDialogueFragment != null)
            {
                accidentDialogueFragment.Dismiss();
                accidentDialogueFragment = null;
            }
            oneRequestatatime = 0;
            mFragment.accidentCreate(newAccidentDetail.userName);
            //Do someting here
            mapHelper = new MapFunctionHelper(Resources.GetString(Resource.String.mapkey),mFragment.mainMap);
            LatLng accidentLocation = new LatLng(newAccidentDetail.accidentLat, newAccidentDetail.accidentLng);
            showprogressDialog();
            string directionjson = await mapHelper.GetDirectionJsonAsync(myLatlng,accidentLocation);
            closeprogressDialog();
            mapHelper.DrawAccidentOnMap(directionjson, newAccidentDetail.accidentAddress);
            
        }

        //Accident Not Found event
        private void AccidentDetailsListener_AccidentDetailNotFound(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Accident Detail Not Found", ToastLength.Short).Show();
        }

        //Timeout accident event
        private void AvailabilityListener_accidentTimeout(object sender, EventArgs e)
        {
            //dismiss dialogue and remove alert
            if (accidentDialogueFragment != null)
            {
                accidentDialogueFragment.Dismiss();
                accidentDialogueFragment = null;
                musicplayer.Stop();
                musicplayer = null;
            }
            //return driver online
            Toast.MakeText(this, "Accident request Timeout assigned", ToastLength.Short).Show();
            oneRequestatatime = 0;
            availabilityListener.ReActivate();
        }

        //Cancelled accident event
        private void AvailabilityListener_accidentCancelled(object sender, EventArgs e)
        {
            //dismiss dialogue and remove alert
            if (accidentDialogueFragment != null)
            {
                accidentDialogueFragment.Dismiss();
                accidentDialogueFragment = null;
                musicplayer.Stop();
                musicplayer = null;
            }
            //return driver online
            oneRequestatatime = 0;
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
            //Checking if the account is Processing
            if (AppDataHelper.Getaccstatus() == "processing")
            {
                Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertDialog.SetTitle("Account Checking");
                alertDialog.SetMessage("Your account is still processing, Try again later or Email us Inaccalert@gmail.com");
                alertDialog.SetPositiveButton("Continue", (senderAlert, args) => {
                    return;
                });
                alertDialog.Show();
                return;
            }

            if (!checkPermission())
            {
                return;
            }

            if (!CheckInternet())
            {
                Toast.MakeText(this, "Please Connect to the Internet.", ToastLength.Long).Show();
                return;
            }
            if (!CheckGPS())
            {
                Toast.MakeText(this, "Please Turn on your GPS.", ToastLength.Long).Show();
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
                //toolbar.Visibility = Android.Views.ViewStates.Invisible;
                toolbar.Title = "My Profile";
                onlinebtn.Visibility = Android.Views.ViewStates.Invisible;
                pFragment.ShowVolunteerInformation();
            }
            else if (e.Item.ItemId == Resource.Id.mapnotification)
            {
                viewPager.SetCurrentItem(1, true);
                toolbar.Title = "";
                onlinebtn.Visibility = Android.Views.ViewStates.Visible;
                //toolbar.Visibility = Android.Views.ViewStates.Visible;
            }
            else if (e.Item.ItemId == Resource.Id.history)
            {
                viewPager.SetCurrentItem(2, true);
                toolbar.Title = "Report History";
                onlinebtn.Visibility = Android.Views.ViewStates.Invisible;
                //toolbar.Visibility = Android.Views.ViewStates.Invisible;
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

        void showprogressDialog()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(this);
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

        bool CheckInternet()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                //Toast.MakeText(this, "No Internet Connection", ToastLength.Long).Show();
                return false;
            }
            else
            {
                //Toast.MakeText(this, "Connected to the Internet", ToastLength.Long).Show();
                return true;
            }
        }

        bool CheckGPS()
        {
            LocationManager locationManager = (LocationManager)GetSystemService(LocationService);
            bool gpsEnable = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            return gpsEnable;
        }
    }
}