using MauiFirebaseDemo.Views;

namespace MauiFirebaseDemo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("MainPage", typeof(Views.MainPage));
        }

       
    }
}
