using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using Firebase.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsAzure.Messaging;
using Xamarin.Forms;

namespace NotificationDank.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseService :FirebaseMessagingService
    {
        public static string FCMTemplateBody { get; set; } = "{\"data\":{\"message\":\"$(messageParam)\"}}";
        public static string NotificationHubName { get; set; } = "crfar-notification-center";
        public static string ListenConnectionString { get; set; } = "Endpoint=sb://crfar-notification.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=MMkPMPn6s6hIMZdzCpw0RJlh4BfjSqmHP1lNvmyxbo0=";
        public static string[] SubscriptionTags { get; set; } = { "default" };
        public static string CHANNEL_ID = "my_channel_01";
        public static string name = "Notification";
        public static string NotificationName = "SmartCampus Anchor";

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            Console.WriteLine("NEW_TOKEN", token);
            SendRegistrationToServer(token);
        }
        void SendRegistrationToServer(string token)
        {
            NotificationHub hub = new NotificationHub(NotificationHubName, ListenConnectionString, this);
            // register device with Azure Notification Hub using the token from FCM
            Registration reg = hub.Register(token, SubscriptionTags);
            // subscribe to the SubscriptionTags list with a simple template.
            string pnsHandle = reg.PNSHandle;
            hub.RegisterTemplate(pnsHandle, "defaultTemplate", FCMTemplateBody, SubscriptionTags);
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            string messageBody = string.Empty;
            if (message.GetNotification() != null)
            {
                messageBody = message.GetNotification().Body;
            }
            else
            {
                messageBody = message.Data.Values.First();
            }
            try
            {
                MessagingCenter.Send(messageBody, "Update");
            }
            catch (Exception e)
            { }
            SendLocalNotification(messageBody);
        }
        void SendLocalNotification(string body)
        {

            var intent = new Intent(this, typeof(MainActivity));
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
            //var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }
            var notificationBuilder = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle(NotificationName)
                .SetSmallIcon(Resource.Drawable.ic_launcher)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetShowWhen(false)
                .SetContentIntent(pendingIntent);
            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

    }
}