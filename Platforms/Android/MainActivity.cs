using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Firebase;
using Plugin.Firebase.Core.Platforms.Android; // Xamarin.Firebase.Common paketinden gelir


namespace MauiFirebaseDemo
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                // 1. ADIM: Firebase zaten başlatılmış mı kontrol et
                if (Firebase.FirebaseApp.Instance == null)
                {
                    Firebase.FirebaseApp.InitializeApp(this);
                    Android.Util.Log.Info("FIREBASE", "Firebase yeni başlatıldı.");
                }
                else
                {
                    Android.Util.Log.Info("FIREBASE", "Firebase zaten hazırdı (Auto-init).");
                }

                
                CrossFirebase.Initialize(this);

                Android.Util.Log.Info("FIREBASE", "Plugin başarıyla bağlandı.");
            }
            catch (System.Exception ex)
            {
                // Artık "already exists" hatası buraya düşmeyecek
                Android.Util.Log.Error("FIREBASE_INIT", ex.ToString());
            }
        }
    }
}