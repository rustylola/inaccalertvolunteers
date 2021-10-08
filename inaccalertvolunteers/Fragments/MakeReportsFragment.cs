using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using inaccalertvolunteers.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Fragments
{
    public class MakeReportsFragment : Android.Support.V4.App.DialogFragment
    {
        string usernamereport;
        string nameofvolunteer;
        string addressofAccident;
        string pickcateg;
        string accidentdescription;

        //initialize layouts
        TextInputLayout usersnametext;
        TextInputLayout volunteernametext;
        TextInputLayout useraddresstext;
        Spinner accidentcategorytext;
        TextInputLayout accidentdesctext;
        Button sendreportbtn;
        //public event
        public event EventHandler ReportSend;
        //Event Listener
        SendAccidentReportListener sendreportnow;


        public MakeReportsFragment(string nameofuser, string namevolunteer,string addressaccident)
        {
            usernamereport = nameofuser;
            nameofvolunteer = namevolunteer;
            addressofAccident = addressaccident;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            //initialize layouts
            View view = inflater.Inflate(Resource.Layout.SendAccidentReportLayout, container, false);
            usersnametext = (TextInputLayout)view.FindViewById(Resource.Id.nameofuser);
            volunteernametext = (TextInputLayout)view.FindViewById(Resource.Id.nameofvolunteer);
            useraddresstext = (TextInputLayout)view.FindViewById(Resource.Id.accidentaddress);
            accidentdesctext = (TextInputLayout)view.FindViewById(Resource.Id.accidentdescription);
            sendreportbtn = (Button)view.FindViewById(Resource.Id.sendreportbtn);

            //spinner
            accidentcategorytext = (Spinner)view.FindViewById(Resource.Id.accidentcateg);
            accidentcategorytext.ItemSelected += Accidentcategorytext_ItemSelected;
            var adapter = ArrayAdapter.CreateFromResource(Activity, Resource.Array.indoorAccidents, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            accidentcategorytext.Adapter = adapter;

            //Get details
            usersnametext.EditText.Text = usernamereport;
            volunteernametext.EditText.Text = nameofvolunteer;
            useraddresstext.EditText.Text = addressofAccident;

            //click event
            sendreportbtn.Click += Sendreportbtn_Click;
            return view;
        }

        private void Sendreportbtn_Click(object sender, EventArgs e)
        {
            accidentdescription = accidentdesctext.EditText.Text;
            //Do something before close the dialogue

            if (pickcateg == "Pick Accident Category")
            {
                Toast.MakeText(Activity, "Please Choose Category", ToastLength.Long).Show();
                return;
            }
            
            if(accidentdescription == null)
            {
                Toast.MakeText(Activity, "Accident Description is Empty", ToastLength.Long).Show();
                return;
            }

            Toast.MakeText(Activity, "Sending Report...", ToastLength.Short).Show();
            sendreportnow = new SendAccidentReportListener(usernamereport, nameofvolunteer,  pickcateg, accidentdescription, addressofAccident);
            sendreportnow.CreateReport();
            Toast.MakeText(Activity, "Report Successful Send.", ToastLength.Short).Show();
            //event 
            ReportSend.Invoke(this, new EventArgs());
        }

        private void Accidentcategorytext_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner selectedCateg = (Spinner) sender;
            string toast = string.Format("Selected Accident Category is {0}", selectedCateg.GetItemAtPosition(e.Position));
            Toast.MakeText(Activity, toast, ToastLength.Long).Show();
            pickcateg = selectedCateg.GetItemAtPosition(e.Position).ToString();
        }

    }
}