using Android.OS;
using AndroidX.Core.App;

namespace MauiFirebaseDemo.Platforms.Android
{
    public static class NotificationHelper
    {
        public static void Show(string title, string body)
        {
            var context = global::Android.App.Application.Context;
            var manager = NotificationManagerCompat.From(context);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new global::Android.App.NotificationChannel(
                    "default",
                    "General Notifications",
                    global::Android.App.NotificationImportance.High);

                manager.CreateNotificationChannel(channel);
            }

            var builder = new NotificationCompat.Builder(context, "default")
                .SetContentTitle(title)
                .SetContentText(body)
                .SetSmallIcon(global::Android.Resource.Drawable.IcDialogInfo) // Geçici ikon
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetAutoCancel(true);

            var notification = builder.Build();
            var id = (int)(System.DateTime.Now.Ticks % int.MaxValue);

            manager.Notify(id, notification);
        }
    }
}
