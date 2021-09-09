using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RegistrationActivity : AppCompatActivity
    {
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

        //permission
        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.register);
            connectlayout();
            RequestPermissions(permissionGroup, 0);
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

            imageview = (ImageView)FindViewById(Resource.Id.uploadimage);
            capimg = (Button)FindViewById(Resource.Id.captureimg);
            capimg.Click += Capimg_Click;
            uploadimg = (Button)FindViewById(Resource.Id.uploadimg);
            uploadimg.Click += Uploadimg_Click;
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
            byte[] imagearray = System.IO.File.ReadAllBytes(file.Path);
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
            byte[] imagearray = System.IO.File.ReadAllBytes(file.Path);
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
            else if (!useremail.Contains("@") || useremail.Length < 8)
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
        }
    }
}