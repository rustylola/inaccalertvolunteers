using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using inaccalertvolunteers.EventListeners;
using inaccalertvolunteers.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Activity
{
    [Activity(Label = "loginActivity", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class loginActivity : AppCompatActivity
    {
        //define layouts
        Button loginbtn;
        Button registerbtn;
        EditText emailtext;
        EditText passwordtext;
        CoordinatorLayout rootview;

        //define firebase auth and database
        FirebaseDatabase database;
        FirebaseAuth mAuth;
        FirebaseUser currentuser;

        //define dialog builder
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.login);
            connectlayout();
            initializefirebase();
        }

        // connect layout define ID
        void connectlayout()
        {
            loginbtn = (Button)FindViewById(Resource.Id.loginem);
            registerbtn = (Button)FindViewById(Resource.Id.btnregister);
            emailtext = (EditText)FindViewById(Resource.Id.emailinput);
            passwordtext = (EditText)FindViewById(Resource.Id.passwordinput);
            rootview = (CoordinatorLayout)FindViewById(Resource.Id.rootView);

            loginbtn.Click += Loginbtn_Click;
            registerbtn.Click += Registerbtn_Click;
        }

        // initialize fibase using Appdatahelper 
        void initializefirebase()
        {
            database = AppDataHelper.Getdatabase();
            mAuth = AppDataHelper.GetfirebaseAuth();
            currentuser = AppDataHelper.Getcurrentuser();
        }

        //click event register button
        private void Registerbtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
        }

        //click event login button
        private void Loginbtn_Click(object sender, EventArgs e)
        {
            //define email and password
            string email, password;
            email = emailtext.Text;
            password = passwordtext.Text;

            //validation of email and password
            if (!email.Contains("@") || email.Length < 7 || email.Contains(" "))
            {
                Snackbar.Make(rootview, "Please enter a valid Email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 8)
            {
                Snackbar.Make(rootview, "Please enter a valid Password", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length > 35)
            {
                Snackbar.Make(rootview, "Password is too long, Try again.", Snackbar.LengthShort).Show();
                return;
            }

            TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;

            showprogressDialog();

            //defing firebase authentication then use success listener and failure listeners
            mAuth.SignInWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this, taskCompletionListener)
                .AddOnFailureListener(this, taskCompletionListener);
        }

        //failure event
        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            Snackbar.Make(rootview, "Login Failed, Check your Email and password.", Snackbar.LengthShort).Show();
            closeprogressDialog();
            return;
        }

        //success event
        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            closeprogressDialog();
            Toast.MakeText(this,"Login Successful", ToastLength.Short).Show();
            StartActivity(typeof(MainActivity));
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
        public override void OnBackPressed()
        {
            Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Close the application.");
            alertDialog.SetMessage("This application will close.");
            alertDialog.SetPositiveButton("Close", (close, args) =>
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }).SetNegativeButton("Stay", (stay, args) => {
                return;
            });
            alertDialog.Show();
        }
    }
}