using Android;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using inaccalertvolunteers.EventListeners;
using inaccalertvolunteers.Helper;
using Java.Util;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RegistrationActivity : AppCompatActivity, IOnSuccessListener, IOnFailureListener
    {
        //string
        string downloadURL;
        //Task success/failure
        TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
        //define layouts
        TextInputLayout fullnametext;
        TextInputLayout useremailtext;
        TextInputLayout userphonetext;
        TextInputLayout userpasswordtext;
        TextInputLayout confirmpasstext;
        TextView backtolog;
        Button createacc;
        CoordinatorLayout rootView;

        ImageView imageview;
        Button capimg;
        Button uploadimg;

        private byte[] imagearray;

        //define firebase related
        StorageReference storageReference;
        FirebaseDatabase database;
        FirebaseAuth mAuth;
        FirebaseUser currentuser;

        //permission
        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };

        //Alert Dialog inialize
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.register);
            connectlayout();
            setupFirebase();
            RequestPermissions(permissionGroup, 0);
        }

        void setupFirebase()
        {
            database = AppDataHelper.Getdatabase();
            mAuth = AppDataHelper.GetfirebaseAuth();
            currentuser = AppDataHelper.Getcurrentuser();
        }

        void connectlayout()
        {
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);

            fullnametext = (TextInputLayout)FindViewById(Resource.Id.registername);
            useremailtext = (TextInputLayout)FindViewById(Resource.Id.registeremail);
            userphonetext = (TextInputLayout)FindViewById(Resource.Id.registerphone);
            userpasswordtext = (TextInputLayout)FindViewById(Resource.Id.registerpassword);
            confirmpasstext = (TextInputLayout)FindViewById(Resource.Id.registerconfirmpassword);

            createacc = (Button)FindViewById(Resource.Id.registerbtn);
            createacc.Click += Createacc_Click;
            backtolog = (TextView)FindViewById(Resource.Id.backtologin);
            backtolog.Click += Backtolog_Click;

            imageview = (ImageView)FindViewById(Resource.Id.uploadimage);

            capimg = (Button)FindViewById(Resource.Id.captureimg);
            capimg.Click += Capimg_Click;
            uploadimg = (Button)FindViewById(Resource.Id.uploadimg);
            uploadimg.Click += Uploadimg_Click;
        }

        private void Backtolog_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(loginActivity));
        }

        private void Uploadimg_Click(object sender, EventArgs e)
        {
            uploadphoto();
        }

        private void Capimg_Click(object sender, EventArgs e)
        {
            takephoto();
        }

        async void takephoto()
        {
            await CrossMedia.Current.Initialize();
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions 
            { 
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 90,
                Name = "captureimg.jpg",
                Directory = "sample"

            });

            if (file == null)
            {
                return;
            }
            // Convert file to byte array and set the resulting bitmap to imageview
            imagearray = System.IO.File.ReadAllBytes(file.Path);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imagearray, 0, imagearray.Length);
            imageview.SetImageBitmap(bitmap);
        }

        async void uploadphoto()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                Toast.MakeText(this, "Upload image is not supported on this device", ToastLength.Short).Show();
                return;
            }
            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                CompressionQuality = 40,

            });
            // Convert file to byte array and set the resulting bitmap to imageview
            imagearray = System.IO.File.ReadAllBytes(file.Path);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imagearray, 0, imagearray.Length);
            imageview.SetImageBitmap(bitmap);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Createacc_Click(object sender, EventArgs e)
        {
            string fullname, useremail, userphone, userpassword, confirmpass;
            fullname = fullnametext.EditText.Text;
            useremail = useremailtext.EditText.Text;
            userphone = userphonetext.EditText.Text;
            userpassword = userpasswordtext.EditText.Text;
            confirmpass = confirmpasstext.EditText.Text;

            if (fullname.Length < 4)
            {
                Snackbar.Make(rootView, "Please enter a valid Full name", Snackbar.LengthShort).Show();
                return;
            }
            else if (fullname.Length > 35)
            {
                Snackbar.Make(rootView, "Name must be 35 Characters only", Snackbar.LengthShort).Show();
                return;
            }
            else if (!useremail.Contains("@") || useremail.Length < 8 || useremail.Contains(" "))
            {
                Snackbar.Make(rootView, "Please enter a valid Email", Snackbar.LengthShort).Show();
                return;
            }
            else if (userphone.Length < 10 || userphone.Length > 15)
            {
                Snackbar.Make(rootView, "Please enter a valid Phone number", Snackbar.LengthShort).Show();
                return;
            }
            else if (userpassword.Length < 8)
            {
                Snackbar.Make(rootView, "Please enter a passwrod up to 8 characters", Snackbar.LengthShort).Show();
                return;
            }
            else if (userpassword.Length > 30)
            {
                Snackbar.Make(rootView, "Password must be 30 Characters only", Snackbar.LengthShort).Show();
                return;
            }
            else if (userpassword != confirmpass)
            {
                Snackbar.Make(rootView, "Password and Confirm password not the same", Snackbar.LengthShort).Show();
                return;
            }
            else if (imagearray == null)
            {
                Snackbar.Make(rootView, "Please upload image for validation", Snackbar.LengthShort).Show();
                return;
            }

            registeruser(fullname, useremail, userphone, userpassword, imagearray);

        }

        private void registeruser(string fullname, string useremail, string userphone, string userpassword, byte[] imagearray)
        {
            mAuth.CreateUserWithEmailAndPassword(useremail, userpassword)
                .AddOnSuccessListener(this, taskCompletionListener)
                .AddOnFailureListener(this, taskCompletionListener);

            taskCompletionListener.Success += (o, g) =>
            {
                string imgID = Guid.NewGuid().ToString();
                DatabaseReference newvolunteer = database.GetReference("volunteers/" + mAuth.CurrentUser.Uid);
                HashMap map = new HashMap();
                map.Put("name", fullname);
                map.Put("email", useremail);
                map.Put("phone", userphone);
                map.Put("upload_img", "volunteerimg/" + imgID);
                map.Put("acc_status", "processing");
                map.Put("created_time", DateTime.Now.ToString());

                storageReference = FirebaseStorage.Instance.GetReference("volunteerimg/" + imgID);
                storageReference.PutBytes(imagearray)
                    .AddOnSuccessListener(this)
                    .AddOnFailureListener(this);


                newvolunteer.SetValue(map);

                Toast.MakeText(this, "Registration Successfully, Re-open the app or login now.", ToastLength.Short).Show();
                showprogressDialog();
                StartActivity(typeof(loginActivity));
                closeprogressDialog();
            };

            taskCompletionListener.Failure += (s, z) =>
            {
                Snackbar.Make(rootView, "Fail to Register", Snackbar.LengthShort).Show();
            };
        }

        
        public void OnSuccess(Java.Lang.Object result)
        {
            if (storageReference != null)
            {
                storageReference.GetDownloadUrl().AddOnSuccessListener(this);

            }
            if (!string.IsNullOrEmpty(result.ToString()))
            {
                downloadURL = result.ToString();
                DatabaseReference newvolunteer = database.GetReference("volunteers/" + mAuth.CurrentUser.Uid);
                newvolunteer.Child("upload_img").SetValue(downloadURL);
                return;
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            
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
    }
}