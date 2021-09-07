using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Helper
{
    public static class AppDataHelper
    {
        static ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        public static FirebaseDatabase Getdatabase()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseDatabase database;
            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetProjectId("inaccalert-database")
                    .SetApplicationId("inaccalert-database")
                    .SetApiKey("AIzaSyCDcTY55MlwDzx2r_zAij1uGu0QOMzdzVQ")
                    .SetDatabaseUrl("https://inaccalert-database-default-rtdb.firebaseio.com")
                    .SetStorageBucket("inaccalert-database.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, option);
                database = FirebaseDatabase.GetInstance(app);
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }

            return database;
        }

        public static FirebaseAuth GetfirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;
            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetProjectId("inaccalert-database")
                    .SetApplicationId("inaccalert-database")
                    .SetApiKey("AIzaSyCDcTY55MlwDzx2r_zAij1uGu0QOMzdzVQ")
                    .SetDatabaseUrl("https://inaccalert-database-default-rtdb.firebaseio.com")
                    .SetStorageBucket("inaccalert-database.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, option);
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }

            return mAuth;
        }
        public static FirebaseUser Getcurrentuser()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;
            FirebaseUser mUser;
            if (app == null)
            {
                var option = new FirebaseOptions.Builder()
                    .SetProjectId("inaccalert-database")
                    .SetApplicationId("inaccalert-database")
                    .SetApiKey("AIzaSyCDcTY55MlwDzx2r_zAij1uGu0QOMzdzVQ")
                    .SetDatabaseUrl("https://inaccalert-database-default-rtdb.firebaseio.com")
                    .SetStorageBucket("inaccalert-database.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, option);
                mAuth = FirebaseAuth.Instance;
                mUser = mAuth.CurrentUser;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
                mUser = mAuth.CurrentUser;
            }

            return mUser;
        }

        public static string Getfullname()
        {
            string fullname = pref.GetString("fullname", "");
            return fullname;
        }
        public static string Getemail()
        {
            string email = pref.GetString("email", "");
            return email;
        }
        public static string Getphone()
        {
            string phone = pref.GetString("phone", "");
            return phone;
        }
    }
}