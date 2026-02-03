using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;


namespace MauiFirebaseDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif


            
            builder.RegisterFirebaseServices();
            return builder.Build();
        }

        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            
            builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);

            
            builder.Services.AddSingleton(_ => CrossFirebaseFirestore.Current);

            return builder;
        }
    }
}
