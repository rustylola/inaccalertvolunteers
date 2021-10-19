using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Maps.Android;
using inaccalert.Helpers;
using inaccalertvolunteers.DataModel;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace inaccalertvolunteers.Helper
{
    public class MapFunctionHelper
    {

        string mapkey;
        GoogleMap mainmap;
        public Marker accidentLocMarker;
        public Marker currentLocMarker;
        bool isrequestingDirection;

        //check
        public string durationcheck { get; set; }
        public string distancecheck { get; set; }
        public MapFunctionHelper(string mMapkey, GoogleMap mmap)
        {
            mapkey = mMapkey;
            mainmap = mmap;
        }

        //setting url geocode
        public string GetGeoCodeUrl(double lat, double lng)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lng + "&key=" + mapkey;
            return url;
        }

        //creating task httphandler to enable return url
        public async Task<string> GetGeojasonAsync(string url)
        {
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

        //creating task to enable return json

        public async Task<string> GetDirectionJsonAsync(LatLng userlocation, LatLng volunteerlocation)
        {
            //user location as origin of route
            string str_orig = "origin=" + userlocation.Latitude + "," + userlocation.Longitude; //user location here is the volunteer location

            //volunteer location as destination of route
            string str_destination = "destination=" + volunteerlocation.Latitude + "," + volunteerlocation.Longitude; //volunteer location is the accident location

            //mode
            string mode = "mode=walking";//driving // walking // biking

            //Building Parameters for url webservice
            string parameters = str_orig + "&" + str_destination + "&" + mode + "&";

            //output format
            string output = "json";

            //set map key
            string key = mapkey;

            //final url string
            string url = "https://maps.googleapis.com/maps/api/directions/" + output + "?" + parameters + "key=" + key;

            string json = "";
            json = await GetGeojasonAsync(url);
            return json;
        }

        public void DrawAccidentOnMap(string json, string addressuser)
        {

            Android.Gms.Maps.Model.Polyline mpolyline;
            Marker takeoff;

            var directiondata = JsonConvert.DeserializeObject<DirectionParser>(json);

            var pointcode = directiondata.routes[0].overview_polyline.points;

            var line = PolyUtil.Decode(pointcode);

            LatLng firstpoint = line[0];
            LatLng lastpoint = line[line.Count - 1];

            //takeoffposition
            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(firstpoint);
            markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue));
            takeoff = mainmap.AddMarker(markerOptions);

            //mypossition
            MarkerOptions positionMarker = new MarkerOptions();
            positionMarker.SetPosition(firstpoint);
            positionMarker.SetTitle("Current Location");
            positionMarker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_me));//BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_pinme)
            currentLocMarker = mainmap.AddMarker(positionMarker);

            //accidentlocation
            MarkerOptions accidentMarker = new MarkerOptions();
            accidentMarker.SetPosition(lastpoint);
            accidentMarker.SetTitle("Accident Location");
            accidentMarker.SetSnippet("Address : " + addressuser);
            accidentMarker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));//BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_pinme)
            accidentLocMarker = mainmap.AddMarker(accidentMarker);

            ArrayList routeList = new ArrayList();
            //int locationpoints = 0;
            foreach (LatLng item in line)
            {
                routeList.Add(item);
                //locationpoints++;
                //Console.WriteLine("Position : " + locationpoints.ToString() + " " + item.Latitude.ToString() + " , " + item.Longitude.ToString());
            }

            PolylineOptions polylineOptions = new PolylineOptions()
                .AddAll(routeList)
                .InvokeWidth(20)
                .InvokeColor(Color.Teal)
                .InvokeStartCap(new SquareCap())
                .InvokeEndCap(new SquareCap())
                .InvokeJointType(JointType.Round)
                .Geodesic(true);

            mpolyline = mainmap.AddPolyline(polylineOptions);
            mainmap.UiSettings.ZoomControlsEnabled = true;
            mainmap.TrafficEnabled = true;
            mainmap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(firstpoint, 18));
            accidentLocMarker.ShowInfoWindow();
        }
        //positionmarker = currentLocMarker
        public async void UpdateMovement(LatLng myLastLocation, LatLng accidentLocation, string fromto)
        {
            currentLocMarker.Visible = true;
            currentLocMarker.Position = myLastLocation;

            if (!isrequestingDirection)
            {
                isrequestingDirection = true;
                string json = await GetDirectionJsonAsync(myLastLocation, accidentLocation);
                var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);
                string duration = directionData.routes[0].legs[0].duration.text;
                string distance = directionData.routes[0].legs[0].distance.text;
                currentLocMarker.Title = "Current Location";
                currentLocMarker.Snippet = duration + " / " + distance + " Away from " + fromto;
                currentLocMarker.ShowInfoWindow();
                isrequestingDirection = false;
            }
        }
        public async void CheckDistance(LatLng myLastLocation, LatLng accidentLocation)
        {

            if (!isrequestingDirection)
            {
                isrequestingDirection = true;
                string json = await GetDirectionJsonAsync(myLastLocation, accidentLocation);
                var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);
                durationcheck = directionData.routes[0].legs[0].duration.text;
                distancecheck = directionData.routes[0].legs[0].distance.text;
                isrequestingDirection = false;
            }

        }
    }
}