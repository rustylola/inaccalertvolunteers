using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using inaccalertvolunteers.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static inaccalertvolunteers.Helper.LocationCallbackHelper;

namespace inaccalertvolunteers.Fragments
{
    public class mapnotificationfragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback
    {
        public EventHandler<OnLocationCapturedEventArgs> CurrentLocation;
        //public static mapnotificationfragment instance;
        //initialize google map
        public GoogleMap mainMap;

        //Location Request
        LocationRequest myLocationRequest;
        FusedLocationProviderClient locationProviderClient;
        Android.Locations.Location mylastLocation;

        LocationCallbackHelper mylocationCallback = new LocationCallbackHelper();

        static int UPDATE_INTERVAL = 3; //which means 3 sec per location update
        static int FASTEST_INTERVAL = 3;
        static int DISPLACEMENT = 1; // which means 1 meter per location update

        //layouts
        ImageView marker;

        //layout for accepting request
        LinearLayout callandarrivelayout;
        RelativeLayout callbtn;
        Button arrivebtn;
        TextView usersName;

        //flags
        bool isCreated = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //instance = this;
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.mapnotification, container, false);
            //initialize map
            marker = (ImageView)view.FindViewById(Resource.Id.centerMarker);
            //initialize accept layout
            callandarrivelayout = (LinearLayout)view.FindViewById(Resource.Id.callandarrivelayout);
            callbtn = (RelativeLayout)view.FindViewById(Resource.Id.calluserbtn);
            arrivebtn = (Button)view.FindViewById(Resource.Id.arrivebtn);
            usersName = (TextView)view.FindViewById(Resource.Id.usernameacc);

            //map configure
            SupportMapFragment mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);
            createLocationRequest();
            continuelocation();
            return view;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mainMap = googleMap;
        }

        void createLocationRequest()
        {
            myLocationRequest = new LocationRequest();
            myLocationRequest.SetInterval(UPDATE_INTERVAL);
            myLocationRequest.SetFastestInterval(FASTEST_INTERVAL);
            myLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            myLocationRequest.SetSmallestDisplacement(DISPLACEMENT);

            mylocationCallback.Mylocation += mylocationcallback_mylocation;

            locationProviderClient = LocationServices.GetFusedLocationProviderClient(Activity); // since this is a fragment, we use (Activity) instead of (this)
            
        }

        void mylocationcallback_mylocation(object sender, LocationCallbackHelper.OnLocationCapturedEventArgs e)
        {
            mylastLocation = e.Location;
            //update latest location on the map
            LatLng myposition = new LatLng(mylastLocation.Latitude, mylastLocation.Longitude);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 18));
            //marker.Visibility = ViewStates.Visible;
            //Sending Location to Main Activity
            CurrentLocation?.Invoke(this, new OnLocationCapturedEventArgs { Location = e.Location});
        }

        void startLocationUpdate()
        {
            if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted)
            {
                locationProviderClient.RequestLocationUpdates(myLocationRequest, mylocationCallback, null);
            }
            else
            {
                return;
            }
        }

        void stopLocationUpdate()
        {
            locationProviderClient.RemoveLocationUpdates(mylocationCallback);
        }

        async void continuelocation()
        {
            if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }
            else
            {
                mylastLocation = await locationProviderClient.GetLastLocationAsync();
                if (mylastLocation != null)
                {
                    LatLng myposition = new LatLng(mylastLocation.Latitude, mylastLocation.Longitude);
                    mainMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 18)); //set the zoom
                }
            }
        }

        public void GoOnline()
        {
            startLocationUpdate();
            marker.Visibility = ViewStates.Visible;
        }
        public void GoOffline()
        {
            stopLocationUpdate();
            marker.Visibility = ViewStates.Invisible;
        }

        public void accidentCreate(string usersname)
        {
            marker.Visibility = ViewStates.Invisible;
            usersName.Text = usersname;
            callandarrivelayout.Visibility = ViewStates.Visible;
            isCreated = true;
        }
    }
}