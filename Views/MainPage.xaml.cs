using System.Text;
using System.Text.Json;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.Auth;
using Google.Apis.Auth.OAuth2;
using Firebase.Messaging;

using Android.Util;
using MauiFirebaseDemo.ViewModels;
using System.Collections.ObjectModel;

namespace MauiFirebaseDemo.Views;

public partial class MainPage : ContentPage
{
    
    public ObservableCollection<ReceivedNotification> Notifications { get; set; } = new();
    public MainPage()
    {
        InitializeComponent();
        NotificationsListView.ItemsSource = Notifications; 
        
        GetFcmToken();

#if ANDROID
        SubscribeToTopic("news"); 
#endif
        
        CrossFirebaseCloudMessaging.Current.NotificationReceived += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
               
                var newNotif = new ReceivedNotification
                {
                    Title = e.Notification.Title,
                    Body = e.Notification.Body,
                    ReceivedTime = DateTime.Now.ToString("HH:mm")
                };

                
                Notifications.Insert(0, newNotif);

                
                EmptyLabel.IsVisible = false;
#if ANDROID
                MauiFirebaseDemo.Platforms.Android.NotificationHelper.Show(e.Notification.Title, e.Notification.Body);
#endif
                
            });
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        
        await Task.Delay(1000);

        await RequestNotificationPermission();
    }


    private void OnTabClicked(object sender, EventArgs e)
    {
        var btn = sender as Button;
        string target = btn.CommandParameter.ToString();

        
        TabSendBtn.BackgroundColor = TabInboxBtn.BackgroundColor = TabDeviceBtn.BackgroundColor = Colors.Transparent;
        TabSendBtn.TextColor = TabInboxBtn.TextColor = TabDeviceBtn.TextColor = Colors.Gray;

        
        btn.BackgroundColor = Color.FromArgb("#3D8CFF");
        btn.TextColor = Colors.White;

        
        ViewSend.IsVisible = target == "Send";
        ViewInbox.IsVisible = target == "Inbox";
        ViewDevice.IsVisible = target == "Device";
    }

    
    private void SetLoading(bool isLoading)
    {
        PushBtn.IsEnabled = !isLoading;
        PushIndicator.IsVisible = isLoading;
        PushIndicator.IsRunning = isLoading;
        PushBtn.Text = isLoading ? string.Empty : "Send Notification";
    }


    private async Task RequestNotificationPermission()
    {
        bool areNotificationsEnabled = true;

#if ANDROID
        
        var context = Android.App.Application.Context;
        areNotificationsEnabled = AndroidX.Core.App.NotificationManagerCompat.From(context).AreNotificationsEnabled();
#endif

        
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

        
        if (!areNotificationsEnabled || status != PermissionStatus.Granted)
        {
            
            if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Version.Major >= 13)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }

            
            if (!areNotificationsEnabled || status != PermissionStatus.Granted)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    bool answer = await DisplayAlert("Notification Permission",
                        "Notifications are disabled in your system settings. To receive push messages, please enable them.",
                        "Go to Settings", "Cancel");

                    if (answer)
                    {
                        AppInfo.ShowSettingsUI();
                    }
                });
            }
        }
    }

    private async void GetFcmToken()
    {
        try
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            TokenLabel.Text = token;
            TargetEntry.Text = token; 
        }
        catch (Exception ex)
        {
            TokenLabel.Text = "FCM Error: " + ex.Message;
        }
    }

#if ANDROID
    private void SubscribeToTopic(string topic)
    {
        FirebaseMessaging.Instance.SubscribeToTopic(topic)
            .AddOnCompleteListener(new OnCompleteListener(topic));
    }

    
    public class OnCompleteListener : Java.Lang.Object, Android.Gms.Tasks.IOnCompleteListener
    {
        private readonly string _topic;

        public OnCompleteListener(string topic)
        {
            _topic = topic;
        }

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful)
                Log.Debug("FCM", $"Subscribed to topic '{_topic}' successfully!");
            else
                Log.Debug("FCM", $"Failed to subscribe to topic '{_topic}'");
        }
    }
#endif

    private async void OnSendNotificationClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || string.IsNullOrWhiteSpace(TargetEntry.Text))
        {
            await DisplayAlert("Validation", "Title and Target are required.", "OK");
            return;
        }

        SetLoading(true); 

        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("service-account.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var credential = GoogleCredential.FromJson(json).CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
            var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            var doc = JsonDocument.Parse(json);
            string projectId = doc.RootElement.GetProperty("project_id").GetString();

            string target = TargetEntry.Text.Trim();
            bool isTopic = !target.Contains(":") && target.Length < 50;

            var payload = new
            {
                message = new
                {
                    topic = isTopic ? target : null,
                    token = !isTopic ? target : null,
                    notification = new { title = TitleEntry.Text, body = BodyEntry.Text },
                    android = new { priority = "high" }
                }
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send", content);

            if (response.IsSuccessStatusCode)
                await DisplayAlert("Success", "Notification sent successfully!", "OK");
            else
                await DisplayAlert("FCM Error", "Check your token or topic name.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("System Error", ex.Message, "OK");
        }
        finally
        {
            SetLoading(false); 
        }
    }
    

    private async void OnCopyTokenClicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(TokenLabel.Text);
        await DisplayAlert("Copied", "Token copied to clipboard.", "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure?", "Yes", "No");
        if (confirm)
        {
            await CrossFirebaseAuth.Current.SignOutAsync();
            Preferences.Set("is_logged_in", false);
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}

