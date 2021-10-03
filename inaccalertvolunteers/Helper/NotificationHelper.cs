using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Helper
{
    public class NotificationHelper : Java.Lang.Object
    {
        public const string PRIMARY_CHANNEL = "urgent";
        public const int NOTIFYID = 100;
        
        //Local notification had different method when it comes to version
        //So we will make different version in order that any version can receive notification from Accident Request
        public void NotifyVersion26(Context context,Android.Content.Res.Resources res,Android.App.NotificationManager notifManager)
        {
            string ChannelName = "Secondary Channel";
            var importance = NotificationImportance.High;
            var channel = new NotificationChannel(PRIMARY_CHANNEL, ChannelName, importance);
            var path = Android.Net.Uri.Parse("android.resource://com.companyname.inaccalertvolunteers/" + Resource.Raw.AccidentAlert);

            var audioAttribute = new AudioAttributes.Builder()
                .SetContentType(AudioContentType.Sonification)
                .SetUsage(AudioUsageKind.Notification).Build();

            channel.EnableLights(true);
            channel.EnableVibration(true);
            channel.SetSound(path, audioAttribute);
            channel.LockscreenVisibility = NotificationVisibility.Public;

            notifManager.CreateNotificationChannel(channel);

            //when notification TAP, it will resume the app
            Intent intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);

            //create notification and replace with new one
            Notification.Builder builder = new Notification.Builder(context)
                .SetContentTitle("IN-ACC-ALERT:VOLUNTEER")
                .SetSmallIcon(Resource.Drawable.ic_logo)
                .SetLargeIcon(BitmapFactory.DecodeResource(res, Resource.Drawable.ic_logo))
                .SetContentText("You have accident Request, Click here to view.")
                .SetChannelId(PRIMARY_CHANNEL)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            //initialize notification build it
            notifManager.Notify(NOTIFYID, builder.Build());
        }
        //notify older version 
        public void NotifyOtherVersion(Context context, Android.Content.Res.Resources res, Android.App.NotificationManager notifManager)
        {
            Intent intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);
            var path = Android.Net.Uri.Parse("android.resource://com.companyname.inaccalertvolunteers/" + Resource.Raw.AccidentAlert);

            //create notification and replace with new one
            Notification.Builder builder = new Notification.Builder(context)
                .SetContentTitle("IN-ACC-ALERT:VOLUNTEER")
                .SetSmallIcon(Resource.Drawable.ic_logo)
                .SetLargeIcon(BitmapFactory.DecodeResource(res, Resource.Drawable.ic_logo))
                .SetTicker("You have accident Request, Click here to view.")
                .SetChannelId(PRIMARY_CHANNEL)
                .SetSound(path)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            //initialize notification build it
            notifManager.Notify(NOTIFYID, builder.Build());
        }

    }

}